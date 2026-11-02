using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Measurement.Generators;

// Emits the identical-across-every-measurement boilerplate (operators, equality, comparison,
// formatting, math utilities, and the IMeasurement<T> / System.Numerics surface) into each
// [Measurement]-annotated readonly partial struct, so the handwritten source only needs the
// unit-specific From/To methods and cross-type operators.
[Generator]
public sealed class MeasurementGenerator : IIncrementalGenerator {
    private const string AttrMetadataName = "com.hafthor.Measurement.MeasurementAttribute";

    private static readonly string[] None = ["None"];

    // One [Measurement] struct, encoded for the incremental pipeline. Units/Products are ';'-joined
    // strings (rather than collections), so the value stays cheaply equatable for caching.
    private readonly record struct MeasurementInfo(
        string Name,
        string Namespace,
        string Symbol,
        double DisplayFactor,
        string VariableName,
        List<IUnitSpec> Units,
        List<ProductSpec> Products,
        List<ReciprocalSpec> Reciprocals);

    private interface IUnitSpec {
        string Name { get; }
    }

    private static IUnitSpec FromAttribute(AttributeData a) =>
        (IUnitSpec)SiUnitSpec.FromAttribute(a) ??
        (IUnitSpec)UnitSpec.FromAttribute(a) ??
        (IUnitSpec)HookSpec.FromAttribute(a);

    // $"si|{baseName}|{tenExp}|{prefixes}|{perPrefixes}"
    private readonly record struct SiUnitSpec(string Name, int TenExp, string Prefixes, string PerPrefixes)
        : IUnitSpec {
        public string[] PrefixesSplit { get; } = Prefixes.Split(' ');
        public string[] PerPrefixesSplitOrNone { get; } = PerPrefixes == "" ? None : PerPrefixes.Split(' ');

        public static SiUnitSpec? FromAttribute(AttributeData a) {
            if (a.AttributeClass?.Name != "SiUnitAttribute") return null;
            var args = a.ConstructorArguments;
            string baseName = args.Length > 0 ? args[0].Value as string ?? "" : "";
            int tenExp = args.Length > 1 && args[1].Value is int e ? e : 0;
            string prefixes = args.Length > 2 ? args[2].Value as string ?? "None" : "None";
            string perPrefixes = "None";
            foreach (var na in a.NamedArguments)
                if (na is { Key: "PerPrefixes", Value.Value: string pp })
                    perPrefixes = pp;
            return new SiUnitSpec(baseName, tenExp, prefixes, perPrefixes);
        }
    }

    // $"u|{unitName}|{factor.ToString("R", inv)}|{offset.ToString("R", inv)}"
    private readonly record struct UnitSpec(string Name, double Factor, double Offset) : IUnitSpec {
        public static UnitSpec? FromAttribute(AttributeData a) {
            if (a.AttributeClass?.Name != "UnitAttribute") return null;
            var args = a.ConstructorArguments;
            string unitName = args.Length > 0 ? args[0].Value as string ?? "" : "";
            double factor = args.Length > 1 && args[1].Value is double f ? f : 1;
            double offset = 0;
            foreach (var na in a.NamedArguments)
                if (na is { Key: "Offset", Value.Value: double o })
                    offset = o;
            return new UnitSpec(unitName, factor, offset);
        }
    }

    // $"h|{hookName}"
    private readonly record struct HookSpec(string Name) : IUnitSpec {
        public static HookSpec? FromAttribute(AttributeData a) {
            if (a.AttributeClass?.Name != "UnitHookAttribute") return null;
            var args = a.ConstructorArguments;
            string hookName = args.Length > 0 ? args[0].Value as string ?? "" : "";
            return new HookSpec(hookName);
        }
    }

    private readonly record struct ProductSpec(string LeftName, string RightName, bool IsPrimary) {
        public static ProductSpec? FromAttribute(AttributeData a) {
            if (a.AttributeClass is not { Name: "ProductAttribute", TypeArguments.Length: 2 }
                || a.AttributeClass.TypeArguments[0] is not INamedTypeSymbol l
                || a.AttributeClass.TypeArguments[1] is not INamedTypeSymbol r) return null;
            bool primary = false;
            foreach (var na in a.NamedArguments)
                if (na is { Key: "Primary", Value.Value: bool p })
                    primary = p;
            return new ProductSpec(l.Name, r.Name, primary);
        }
    }

    private readonly record struct ReciprocalSpec(string Partner, string ReadName) {
        public static ReciprocalSpec? FromAttribute(AttributeData a) {
            if (a.AttributeClass is not { Name: "ReciprocalAttribute", TypeArguments.Length: 1 }
                || a.AttributeClass.TypeArguments[0] is not INamedTypeSymbol partner) return null;
            string readName = partner.Name;
            foreach (var na in a.NamedArguments)
                if (na is { Key: "Name", Value.Value: string { Length: > 0 } s })
                    readName = s;
            return new ReciprocalSpec(partner.Name, readName);
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var types = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttrMetadataName,
            predicate: static (node, _) => node is StructDeclarationSyntax,
            transform: static (ctx, _) => {
                var type = (INamedTypeSymbol)ctx.TargetSymbol;
                var a0 = ctx.Attributes[0];
                var symbol = a0.ConstructorArguments.Length > 0
                    ? a0.ConstructorArguments[0].Value as string ?? ""
                    : "";

                double displayFactor = 1;
                string variableName = "value";
                foreach (var na in a0.NamedArguments)
                    if (na is { Key: "DisplayFactor", Value.Value: double d })
                        displayFactor = d;
                    else if (na is { Key: "VariableName", Value.Value: string s })
                        variableName = s;

                List<IUnitSpec> unitSpecs = type.GetAttributes()
                    .Select(a => FromAttribute(a))
                    .Where(spec => spec is not null)
                    .ToList();

                // [Product<TLeft, TRight>] relations: Name = Left × Right (Primary marks the result a
                // selector implicitly converts to when the product is shared by several types).
                List<ProductSpec> products = type.GetAttributes()
                    .Select(a => ProductSpec.FromAttribute(a))
                    .Where(spec => spec.HasValue)
                    .Select(spec => spec.Value)
                    .ToList();

                // [Reciprocal<TPartner>(Name=…)]: this = 1 / TPartner in coherent SI, encoded as
                // "partnerType|readName".
                List<ReciprocalSpec> reciprocals = type.GetAttributes()
                    .Select(a => ReciprocalSpec.FromAttribute(a))
                    .Where(spec => spec.HasValue)
                    .Select(spec => spec.Value)
                    .ToList();

                return new MeasurementInfo(type.Name, type.ContainingNamespace.ToDisplayString(), symbol,
                    displayFactor, variableName, unitSpecs, products, reciprocals);
            });

        // Struct body: identical-per-type boilerplate plus the unit From/To methods.
        context.RegisterSourceOutput(types, static (ctx, info) =>
            ctx.AddSource(info.Name + ".g.cs", SourceText.From(
                Emit(info.Name, info.Namespace, info.Symbol, info.DisplayFactor, info.VariableName, info.Units),
                Encoding.UTF8)));

        // Fluent sugar: needs every type at once so input-hook name collisions across
        // dimensionally equivalent types can be detected and dropped.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitFluentSet(ctx, all));

        // Cross-type operators from [Product] relations: needs every type's symbol + DisplayFactor.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitProducts(ctx, all));

        // `.Per` denominator-walk builders hung off numerator measurement types (input side).
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitPerBuilders(ctx, all));

        // Read-out mirror: `reader.To.<Numerator>.Per.<denominator…>` word-walk returning a double.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitPerReadBuilders(ctx, all));

        // Product-unit token algebra: `.Ampere.Hours`, `.Joule.Minutes`, `.Light.Seconds`,
        // `.Newton.Meters` (→ Torque|Energy) — tokens compose via the [Product] relations.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitTokenAlgebra(ctx, all));

        // Square/Cubic area & volume modifier: `.Square.Milli.Meters`, `.Cubic.Centi.Meters`.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitSquareCubic(ctx, all));
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitSquareCubicRead(ctx, all));

        // Type-constrained fluent entry: `Area.Of(4).Square.Milli.Meters` — only T's own units.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitTypedEntry(ctx, all));

        // Reciprocal cross-type reads from [Reciprocal] relations: `frequency.To.Period` → Duration.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitReciprocals(ctx, all));
    }

    // Units excluded from the `.Per` walk because their result is dimensionally inconsistent with a
    // literal numerator÷denominator reading — e.g., psi's "Pounds" is pound-force, not mass, so
    // Mass.Per.Square.Inch would wrongly yield Pressure. These stay reachable via the flat hook only.
    private static readonly HashSet<string> PerExcludeUnits = ["PoundsPerSquareInch"];

    // SI prefix name → power-of-ten exponent (used to expand SiUnit families).
    private static readonly Dictionary<string, int> PrefixExponents = new() {
        ["Quetta"] = 30, ["Ronna"] = 27, ["Yotta"] = 24, ["Zetta"] = 21, ["Exa"] = 18,
        ["Peta"] = 15, ["Tera"] = 12, ["Giga"] = 9, ["Mega"] = 6, ["Kilo"] = 3,
        ["Hecto"] = 2, ["Deca"] = 1, ["None"] = 0, ["Deci"] = -1, ["Centi"] = -2,
        ["Milli"] = -3, ["Micro"] = -6, ["Nano"] = -9, ["Pico"] = -12, ["Femto"] = -15,
        ["Atto"] = -18, ["Zepto"] = -21, ["Yocto"] = -24, ["Ronto"] = -27, ["Quecto"] = -30,
    };

    private static readonly Dictionary<string, int> PrefixExponentsWithoutNone = MakePrefixExponentsWithoutNone();

    private static Dictionary<string, int> MakePrefixExponentsWithoutNone() {
        Dictionary<string, int> d = new(PrefixExponents);
        d.Remove("None");
        return d;
    }

    private static List<string> AllowedNext(IEnumerable<string> source, List<string> s) {
        HashSet<string> set = new(StringComparer.Ordinal);
        foreach (var key in source) {
            var parts = key.Split(',');
            if (parts.Length > s.Count && parts.Take(s.Count).SequenceEqual(s))
                set.Add(parts[s.Count]);
        }

        return [.. set.OrderBy(s => s, StringComparer.Ordinal)];
    }

    // Expands the encoded SiUnit families into a From/To method source. The base unit's factor to
    // the anchor is 10^tenExponent; a numerator prefix shifts the exponent by prefix×power, a
    // denominator (Per) prefix by −prefix×power, so factors stay exact powers of ten (identity at 0).
    private static string EmitUnits(string name, string variableName, List<IUnitSpec> units) {
        if (units.Count == 0) return "";
        var sb = new StringBuilder();
        // Removable prefix aliases (Kilometers = Kilo·Meters) are synthesized from the base unit +
        // scaling by consumers via Cons(...), so their explicit From*/To* factories are not emitted.
        var lookup = new Dictionary<string, double>();
        foreach (var kv in EnumerateUnitFactors(units))
            if (!lookup.ContainsKey(kv.Key))
                lookup[kv.Key] = kv.Value;

        foreach (var spec in units) {
            if (spec is UnitSpec u) {
                if (TryPrefixAlias(u.Name, u.Factor, lookup).HasValue) continue;
                var inv = CultureInfo.InvariantCulture;
                EmitExplicitUnit(sb, name, variableName, u.Name, u.Factor.ToString("R", inv),
                    u.Offset.ToString("R", inv));
                continue;
            }

            if (spec is not SiUnitSpec si) continue;
            // si|baseName|exp|prefixes|perPrefixes
            string baseName = si.Name;
            int baseExp = si.TenExp;
            string[] numPrefixes = si.PrefixesSplit;
            string[] denPrefixes = si.PerPrefixesSplitOrNone;

            var words = SplitWords(baseName);
            int perIdx = words.IndexOf("Per");
            (int numRoot, int numPow) = FindRoot(words, 0, perIdx < 0 ? words.Count : perIdx);
            int denRoot = -1, denPow = 1;
            if (perIdx >= 0) (denRoot, denPow) = FindRoot(words, perIdx + 1, words.Count);

            foreach (var np in numPrefixes) {
                if (np.Length == 0 || !PrefixExponents.TryGetValue(np, out int npExp)) continue;
                string[] dps = denRoot < 0 ? ["None"] : denPrefixes;
                foreach (var dp in dps) {
                    if (dp.Length == 0 || !PrefixExponents.TryGetValue(dp, out int dpExp)) continue;
                    int exp = baseExp + npExp * numPow - (denRoot < 0 ? 0 : dpExp * denPow);
                    var w = new List<string>(words);
                    if (np != "None") w[numRoot] = np + LowerFirstChar(words[numRoot]);
                    if (denRoot >= 0 && dp != "None") w[denRoot] = dp + LowerFirstChar(words[denRoot]);
                    string full = string.Concat(w);
                    if (TryPrefixAlias(full, Math.Pow(10, exp), lookup) != null) continue;
                    string param = LowerFirstChar(full);
                    if (exp == 0) {
                        sb.Append(
                            $"    internal static {name} From{full}(double {param}) => new {name}({param});\n");
                        sb.Append($"    internal double To{full}() => {variableName};\n");
                    } else {
                        string lit = "1e" + exp;
                        sb.Append(
                            $"    internal static {name} From{full}(double {param}) => new {name}({param} * {lit});\n");
                        sb.Append($"    internal double To{full}() => {variableName} / {lit};\n");
                    }
                }
            }
        }

        return sb.ToString();
    }

    // Core expansion of a type's encoded units into (name, factor) pairs in declaration order. Hook
    // units carry NaN, explicit [Unit]s their factor, and SI families a power-of-ten factor per
    // numerator×denominator prefix. When hookMode is set, only the unprefixed numerator base is
    // produced (SI-prefixed numerators are reached through the prefix chain instead).
    private static IEnumerable<KeyValuePair<string, double>> ExpandUnits(List<IUnitSpec> units, bool hookMode) {
        foreach (var unit in units) {
            if (unit is HookSpec h) yield return new(h.Name, double.NaN);
            if (unit is UnitSpec u) yield return new(u.Name, u.Factor);
            if (unit is not SiUnitSpec si) continue;
            // si|baseName|exp|prefixes|perPrefixes
            string baseName = si.Name;
            int baseExp = si.TenExp;
            string[] numPrefixes = hookMode ? ["None"] : si.PrefixesSplit;
            string[] denPrefixes = si.PerPrefixesSplitOrNone;
            var words = SplitWords(baseName);
            int perIdx = words.IndexOf("Per");
            (int numRoot, int numPow) = FindRoot(words, 0, perIdx < 0 ? words.Count : perIdx);
            int denRoot = -1, denPow = 1;
            if (perIdx >= 0) (denRoot, denPow) = FindRoot(words, perIdx + 1, words.Count);
            foreach (var np in numPrefixes) {
                if (np.Length == 0 || !PrefixExponents.TryGetValue(np, out int npExp)) continue;
                string[] dps = denRoot < 0 ? ["None"] : denPrefixes;
                foreach (var dp in dps) {
                    if (dp.Length == 0 || !PrefixExponents.TryGetValue(dp, out int dpExp)) continue;
                    int exp = baseExp + npExp * numPow - (denRoot < 0 ? 0 : dpExp * denPow);
                    var w = new List<string>(words);
                    if (np != "None") w[numRoot] = np + LowerFirstChar(words[numRoot]);
                    if (denRoot >= 0 && dp != "None") w[denRoot] = dp + LowerFirstChar(words[denRoot]);
                    yield return new(string.Concat(w), Math.Pow(10, exp));
                }
            }
        }
    }

    // Enumerates every From/To unit name the generator emits for a type (SI-prefixed family members,
    // Per-prefix cross products, and explicit [Unit] names), preserving declaration order. Used to
    // map a compound unit's numerator/denominator words back to concrete types and To-methods.
    private static List<string> EnumerateUnitNames(List<IUnitSpec> units) {
        var names = new List<string>();
        foreach (var kv in ExpandUnits(units, hookMode: false)) names.Add(kv.Key);
        return names;
    }

    // (name → power-of-ten/explicit factor) for every unit. Used for removable prefix-alias detection:
    // a unit whose name is a leading SI prefix + a base unit of the same type with a power-of-ten
    // factor (Kilometers = Kilo·Meters) is synthesized from the base + scaling rather than a factory.
    private static List<KeyValuePair<string, double>> EnumerateUnitFactors(List<IUnitSpec> units) =>
        new(ExpandUnits(units, hookMode: false));

    // If unitName is a leading-prefix alias of a base unit in lookup with a matching power-of-ten
    // factor, returns (base unit, prefix ten-exponent); otherwise null.
    private static (string Base, int Exp)? TryPrefixAlias(string unitName, double factor,
        Dictionary<string, double> lookup) {
        if (double.IsNaN(factor)) return null;

        foreach (var kv in PrefixExponentsWithoutNone) {
            if (!unitName.StartsWith(kv.Key)) continue;

            string rest = unitName.Substring(kv.Key.Length);
            if (rest.Length == 0) continue;

            string baseRest = UpperFirstChar(rest);
            if (baseRest == unitName || !lookup.TryGetValue(baseRest, out double bf) || double.IsNaN(bf)) continue;
            double expected = bf * Math.Pow(10, kv.Value);
            double scale = Math.Max(Math.Abs(factor), Math.Abs(expected));
            if (scale == 0 || Math.Abs(factor - expected) <= 1e-6 * scale)
                return (baseRest, kv.Value);
        }

        return null;
    }

    // type|unit → (base unit, prefix ten-exponent) for every removable prefix alias.
    private static Dictionary<string, (string Base, int Exp)> BuildRemovable(ImmutableArray<MeasurementInfo> all) {
        var map = new Dictionary<string, (string Base, int Exp)>();
        foreach (var info in all) {
            var factors = EnumerateUnitFactors(info.Units);
            var lookup = new Dictionary<string, double>();
            foreach (var kv in factors)
                if (!lookup.ContainsKey(kv.Key))
                    lookup[kv.Key] = kv.Value;

            foreach (var kv in factors) {
                var alias = TryPrefixAlias(kv.Key, kv.Value, lookup);
                if (alias.HasValue) map[info.Name + "|" + kv.Key] = alias.Value;
            }
        }

        return map;
    }

    // Emits an expression constructing `unit` of `lookupType` from `valueExpr` — via the base unit +
    // prefix scaling when `unit` is a removable prefix alias, else via its own factory. `emitType` is
    // the (possibly namespace-qualified) type name used in the emitted call.
    private static string Cons(string emitType, string lookupType, string unit, string valueExpr,
        Dictionary<string, (string Base, int Exp)> removable) =>
        removable.TryGetValue(lookupType + "|" + unit, out var a)
            ? $"{emitType}.From{a.Base}(({valueExpr}) * 1e{a.Exp})"
            : $"{emitType}.From{unit}({valueExpr})";

    // Directly hookable units (each [Unit] and [UnitHook], plus the unprefixed base of each [SiUnit]
    // family — SI-prefixed variants are reached through the prefix chain instead).
    private static List<KeyValuePair<string, double>> ComputeHookCandidates(List<IUnitSpec> units) =>
        new(ExpandUnits(units, hookMode: true));

    // Emits a single non-metric unit. With no offsets, it's a plain factor; otherwise an affine
    // scale where anchor = (value + offset) * factor.
    private static void EmitExplicitUnit(StringBuilder sb, string name, string variableName, string unitName,
        string factorLit, string offsetLit) {
        string param = LowerFirstChar(unitName);
        bool affine = offsetLit != "0";
        if (!affine) {
            sb.Append(
                $"    internal static {name} From{unitName}(double {param}) => new {name}({param} * {factorLit});\n");
            sb.Append($"    internal double To{unitName}() => {variableName} / {factorLit};\n");
        } else {
            string inner = $"({param} + ({offsetLit}))";
            sb.Append(
                $"    internal static {name} From{unitName}(double {param}) => new {name}({inner} * ({factorLit}));\n");
            string body = $"({variableName}) / ({factorLit})";
            if (offsetLit != "0") body += $" - ({offsetLit})";
            sb.Append($"    internal double To{unitName}() => {body};\n");
        }
    }

    // Finds the first root word in words[start..end) and its power (Square ⇒ 2, Cubic ⇒ 3, else 1).
    private static (int rootIndex, int power) FindRoot(List<string> words, int start, int end) {
        for (int pending = 1, i = start; i < end; i++)
            switch (words[i]) {
                case "Square": pending = 2; break;
                case "Cubic": pending = 3; break;
                case "Squared":
                case "Cubed":
                case "Per": break;
                default: return (i, pending);
            }

        return (-1, 1);
    }

    private static string LowerFirstChar(string s) => char.ToLowerInvariant(s[0]) + s.Substring(1);
    private static string UpperFirstChar(string s) => char.ToUpperInvariant(s[0]) + s.Substring(1);

    // Single-word unit names across all types (Meters, Grams, Newtons, …) — the roots used when
    // prefix-splitting compound spellings (Millimeters → Milli·Meters).
    private static HashSet<string> SingleWordBases(ImmutableArray<MeasurementInfo> all) {
        var bases = new HashSet<string>();
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units))
                if (SplitWords(u).Count == 1)
                    bases.Add(u);
        return bases;
    }

    private readonly record struct DenomInfo(
        string Partner,
        Dictionary<string, string> Term,
        Dictionary<string, SortedSet<string>> Children);

    // Square/Cubic denominator sub-trie shared by the input and read-out `.Per` walks. For each of
    // Area (Square) / Volume (Cubic), maps a singularized, prefix-decomposed length path to the areal/
    // cubic unit name (so `.Cubic.Centi.Meter` resolves to CubicCentimeters' exact coherent value),
    // with the child-word sets along the way.
    private static Dictionary<string, DenomInfo> BuildDenomModTrie(ImmutableArray<MeasurementInfo> all,
        Dictionary<string, (double DF, int G)> meta) {
        string StripS(string s) => s.Length > 1 && s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s;
        var bases = SingleWordBases(all);
        var mod = new Dictionary<string, DenomInfo>();
        foreach (var (kw, partner) in new[] { ("Square", "Area"), ("Cubic", "Volume") }) {
            if (!meta.ContainsKey(partner)) continue;

            var pinfo = all.First(a => a.Name == partner);
            var term = new Dictionary<string, string>();
            var childs = new Dictionary<string, SortedSet<string>>();
            foreach (var u in EnumerateUnitNames(pinfo.Units)) {
                var ws = SplitWords(u);
                if (ws.Count < 2 || ws[0] != kw) continue;
                var lw = new List<string>();
                for (int i = 1; i < ws.Count; i++)
                    foreach (var d in DecomposeDenom(ws[i], bases))
                        lw.Add(StripS(d));

                string key = string.Join("_", lw);
                if (!term.ContainsKey(key)) term[key] = u;
                for (int k = 0; k < lw.Count; k++) {
                    string p = string.Join("_", lw.GetRange(0, k));
                    if (!childs.TryGetValue(p, out var set))
                        childs[p] = set = new SortedSet<string>(StringComparer.Ordinal);
                    set.Add(lw[k]);
                }
            }

            mod[kw] = new DenomInfo(partner, term, childs);
        }

        return mod;
    }

    private readonly record struct Pref(string W, int Exp, string Child);

    private readonly record struct BaseC(string W, string Plural, string D, string BaseUnit);

    private readonly record struct Opt(List<string> Path, string BaseUnit);

    // Merged denominator sub-trie over a set of factor types (used by both `.Per` walks). Produces,
    // per accumulated path: `pref` = prefix children (carrying the SI ten-exponent) and `baseC` =
    // base children that complete one unit of a dimension (W = singular word, Plural, D = its type,
    // BaseUnit = the factory unit name). First spelling wins on collisions.
    private static void BuildDenomTrie(List<string> types,
        Dictionary<string, List<Opt>> unitOpts,
        out Dictionary<string, List<Pref>> pref,
        out Dictionary<string, List<BaseC>> baseC) {
        pref = new();
        baseC = new();
        foreach (var d in types) {
            if (!unitOpts.TryGetValue(d, out var opts)) continue;
            foreach (var (path, baseUnit) in opts) {
                string prog = "";
                for (int i = 0; i < path.Count; i++) {
                    string w = path[i];
                    if (i == path.Count - 1) {
                        if (!baseC.TryGetValue(prog, out var bl)) baseC[prog] = bl = new();
                        if (!bl.Exists(e => e.W == w)) bl.Add(new BaseC(w, baseUnit, d, baseUnit));
                    } else {
                        int exp = PrefixExponents.TryGetValue(w, out var e) ? e : 0;
                        string child = prog.Length == 0 ? w : prog + "_" + w;
                        if (!pref.TryGetValue(prog, out var pl)) pref[prog] = pl = new();
                        if (!pl.Exists(x => x.W == w)) pl.Add(new Pref(w, exp, child));
                        prog = child;
                    }
                }
            }
        }
    }

    private static List<string> SplitWords(string camel) {
        var words = new List<string>();
        var left = 0;
        for (int right = 0; right < camel.Length; right++)
            if (char.IsUpper(camel[right]) && right > left) {
                words.Add(camel.Substring(left, right - left));
                left = right;
            }

        words.Add(camel.Substring(left));
        return words;
    }

    // Emits the fluent-sugar wiring for every struct. Runs once over all types so that input-hook
    // names shared by two-dimensionally equivalent types (e.g., JouleSeconds on Action and
    // AngularMomentum) can be detected. Such names would collide on the shared Prefixed/double
    // receiver, so instead of a direct hook they resolve to a small selector struct that names each
    // measurement: 1.0.JouleSeconds.Action / .AngularMomentum. Their read-out hooks stay direct
    // (distinct Reader<T>). Prefixed aliases (Kilomoles == Kilo + Moles) are dropped from both sides —
    // reach them via the prefix chain (Measure.Of(x).Kilo.Moles). Other SI-prefixed variants likewise
    // go through the chain; only unprefixed bases and non-metric units get a direct hook.
    private static void EmitFluentSet(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        // Per struct: ordered hook candidates minus prefixed aliases.
        var perType = new List<(string Name, string Ns, List<string> Hooks)>();
        var inputCounts = new Dictionary<string, int>();
        foreach (var info in all) {
            var candidates = ComputeHookCandidates(info.Units);
            var lookup = new Dictionary<string, double>();
            foreach (var c in candidates) lookup[c.Key] = c.Value;
            var hooks = new List<string>();
            foreach (var c in candidates) {
                if (TryPrefixAlias(c.Key, c.Value, lookup) != null) continue;
                hooks.Add(c.Key);
                inputCounts.TryGetValue(c.Key, out int n);
                inputCounts[c.Key] = n + 1;
            }

            perType.Add((info.Name, info.Namespace, hooks));
        }

        foreach (var (name, ns, hooks) in perType) {
            if (hooks.Count == 0) continue;
            var from = new StringBuilder();
            var reader = new StringBuilder();
            var dbl = new StringBuilder();
            foreach (var u in hooks) {
                reader.Append($"        public double {u} => r.Value.To{u}() / r.Factor;\n");
                if (inputCounts[u] > 1) continue; // shared name → resolved via a selector (see below)
                from.Append($"        public {name} {u} => {name}.From{u}(p.Value);\n");
                dbl.Append($"        public {name} {u} => {name}.From{u}(value);\n");
            }

            string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
        extension(Prefixed p) {{
{from}        }}
        extension({name} x) {{ public Reader<{name}> To => new(x, 1.0); }}
        extension(Reader<{name}> r) {{
{reader}        }}
    }}
}}

namespace {ns}.Fluent {{
    using {ns};
    public static partial class DoubleSugar {{
        extension(double value) {{
{dbl}        }}
    }}
}}
";
            ctx.AddSource(name + ".Fluent.g.cs", SourceText.From(src, Encoding.UTF8));
        }

        EmitCollisionSelectors(ctx, perType, inputCounts, all);
    }

    // For each input-hook name shared by more than one measurement, emits a selector struct carrying
    // the scaled value with a property per measurement (1.0.RevolutionsPerMinute.Frequency), plus the
    // single shared Prefixed/double hook that returns it — resolving what would be a duplicate clash.
    // Product-spellings (e.g., JouleSeconds = Joule·Seconds) are skipped: the compositional token walk
    // (`.Joule.Seconds.AngularMomentum`) already disambiguates them, so the fused hook is redundant.
    private static void EmitCollisionSelectors(SourceProductionContext ctx,
        List<(string Name, string Ns, List<string> Hooks)> perType, Dictionary<string, int> inputCounts,
        ImmutableArray<MeasurementInfo> all) {
        // Words reachable as product tokens: single-word unit names (and their singulars) + Square/Cubic.
        var productWords = new HashSet<string> { "Square", "Cubic" };
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units))
                if (SplitWords(u).Count == 1) {
                    productWords.Add(u);
                    if (u.Length > 1 && u.EndsWith("s")) productWords.Add(u.Substring(0, u.Length - 1));
                }

        bool IsProductSpelling(string u) {
            var ws = SplitWords(u);
            if (ws.Count < 2 || ws.Contains("Per")) return false;
            foreach (var w in ws)
                if (!productWords.Contains(w))
                    return false;
            return true;
        }

        // Colliding name → owning measurements, in stable declaration order.
        var owners = new List<KeyValuePair<string, List<string>>>();
        var seen = new HashSet<string>();
        foreach (var (_, _, hooks) in perType)
            foreach (var u in hooks)
                if (inputCounts[u] > 1 && !IsProductSpelling(u) && seen.Add(u))
                    owners.Add(new KeyValuePair<string, List<string>>(u, new List<string>()));
        foreach (var (name, _, hooks) in perType)
            foreach (var u in hooks)
                if (inputCounts[u] > 1 && !IsProductSpelling(u))
                    owners.Find(o => o.Key == u).Value.Add(name);
        if (owners.Count == 0) return;

        string ns = perType[0].Ns;
        var selectors = new StringBuilder();
        var prefixed = new StringBuilder();
        var dbl = new StringBuilder();
        foreach (var kv in owners) {
            string u = kv.Key, sel = u + "Selector";
            selectors.Append($"    public readonly struct {sel} {{\n");
            selectors.Append($"        private readonly double value;\n");
            selectors.Append($"        internal {sel}(double value) => this.value = value;\n");
            foreach (var t in kv.Value)
                selectors.Append($"        public {t} {t} => {t}.From{u}(value);\n");
            selectors.Append("    }\n");
            prefixed.Append($"        public {sel} {u} => new(p.Value);\n");
            dbl.Append($"        public {sel} {u} => new(value);\n");
        }

        string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
{selectors}
    public static partial class Units {{
        extension(Prefixed p) {{
{prefixed}        }}
    }}
}}

namespace {ns}.Fluent {{
    using {ns};
    public static partial class DoubleSugar {{
        extension(double value) {{
{dbl}        }}
    }}
}}
";
        ctx.AddSource("_FluentCollisions.g.cs", SourceText.From(src, Encoding.UTF8));
    }

    // Net power of the grams unit (`g`) in a measurement symbol, positive in the numerator and
    // negative in the denominator — used to convert between the grams-based stored value and the
    // coherent-SI (kilogram) basis when deriving operator factors. `Gy`/`Bq` etc. don't contain a
    // lowercase g atom; derived units (N, J, …) are already kilogram-based, so contribute 0.
    private static int GramPower(string symbol) {
        int total = 0, sign = 1;
        var parenSign = new Stack<int>();
        for (int i = 0; i < symbol.Length; i++) {
            char c = symbol[i];
            if (c == '(') parenSign.Push(sign);
            else if (c == ')') {
                if (parenSign.Count > 0) sign = parenSign.Pop();
            } else if (c == '/') sign = -sign; // everything after '/' at this level is a denominator
            else if (c == '·') { /* keep sign */
            } else if (c == 'g' && (i == 0 || !char.IsLetter(symbol[i - 1])) &&
                       (i + 1 >= symbol.Length || !char.IsLetter(symbol[i + 1]))) {
                int exp = 1;
                if (i + 1 < symbol.Length)
                    if (symbol[i + 1] == '²') exp = 2;
                    else if (symbol[i + 1] == '³') exp = 3;
                total += sign * exp;
            }
        }

        return total;
    }

    // Splits a DisplayFactor into a power-of-ten exponent and a non-power-of-ten residual
    // (e.g., 1e6 → (6, 1), 9 → (0, 9)), so operator factors keep their ten-scaling exact in a single
    // 1e{n} and carry any residual (like Temperature's 9) as an explicit divisor.
    private static void SplitTen(double v, out int tenExp, out double residual) {
        tenExp = 0;
        residual = v;
        if (v <= 0) return;
        int e = (int)Math.Round(Math.Log10(v));
        if (Math.Pow(10, e) == v) {
            tenExp = e;
            residual = 1;
        }
    }

    // Coherent-SI divisor for a type = DisplayFactor × 10^(3·gramPower), split into a power-of-ten
    // exponent and a non-ten residual — dividing a stored value by it yields coherent SI (kg·m·s·…).
    private static (int Ten, double Res) CoherentDivisor(double df, int gramPower) {
        SplitTen(df, out int te, out double re);
        return (te + 3 * gramPower, re);
    }

    // Divides/multiplies a value expression by a coherent-SI divisor, keeping powers of ten exact.
    private static string ToCoherentExpr(string val, (int Ten, double Res) d) {
        string s1 = d.Ten > 0 ? $" / 1e{d.Ten}" : d.Ten < 0 ? $" * 1e{-d.Ten}" : null;
        string s2 = d.Res != 1 ? $" / {d.Res.ToString("R", CultureInfo.InvariantCulture)}" : null;
        return d.Ten != 0 || d.Res != 1 ? $"({val}{s1}{s2})" : $"{val}{s1}{s2}";
    }

    private static string FromCoherentExpr(string expr, (int Ten, double Res) d) {
        string s1 = d.Ten > 0 ? $" * 1e{d.Ten}" : d.Ten < 0 ? $" / 1e{-d.Ten}" : null;
        string s2 = d.Res != 1 ? $" * {d.Res.ToString("R", CultureInfo.InvariantCulture)}" : null;
        return $"{expr}{s1}{s2}";
    }

    // Emits every cross-type operator implied by the [Product] relations. For C = A × B it writes
    // A*B, B*A → C and C/A → B, C/B → A, each with an exact factor derived from the symbols'
    // gram-power and DisplayFactor, placed in a partial of one of its parameter types.
    private static void EmitProducts(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        var meta = new Dictionary<string, (double DF, int G)>();
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        foreach (var info in all) meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));

        // container type → operator source lines.
        var byContainer = new Dictionary<string, StringBuilder>();

        StringBuilder Body(string container) {
            if (!byContainer.TryGetValue(container, out var sb)) byContainer[container] = sb = new StringBuilder();
            return sb;
        }

        // Each type's coherent-SI divisor. `Quantity` stores an Avogadro count, but in products it
        // acts as an amount of substance in moles, so normalize its dimension to moles (as the `.Per`
        // builders do) — otherwise molar results (Molality, Concentration, MolarMass, …) would be off
        // by Avogadro. All other types convert through the shared power-of-ten coherent machinery.
        (int Ten, double Res) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        bool hasMole = meta.ContainsKey("Quantity");

        string CoherentVal(string t, string val) =>
            t == "Quantity" && hasMole
                ? $"({val} / (Quantity.FromMoles(1).CanonicalValue))"
                : ToCoherentExpr(val, Div(t));

        string FromCoherentVal(string t, string expr) =>
            t == "Quantity" && hasMole
                ? $"({expr} * (Quantity.FromMoles(1).CanonicalValue))"
                : FromCoherentExpr(expr, Div(t));

        // Gather every operator "intent" keyed by its C# signature; a signature that resolves to more
        // than one result type (a dimensionally ambiguous product like Force×Length → Energy or
        // Torque) is emitted as a selector instead of colliding.
        var intents =
            new Dictionary<string, (string Container, string Op, string P1, string P2, string Expr,
                List<(string Name, bool Primary)> Results)>();

        void Intent(string container, string op, string p1, string p2, string expr, string result, bool primary) {
            string sig = $"{container}|{op}|{p1}|{p2}";
            if (!intents.TryGetValue(sig, out var it))
                intents[sig] = it = (container, op, p1, p2, expr, new List<(string, bool)>());
            if (!it.Results.Exists(x => x.Name == result)) it.Results.Add((result, primary));
        }

        foreach (var info in all) {
            if (info.Products.Count == 0) continue;
            string c = info.Name;
            if (!meta.ContainsKey(c)) continue;
            foreach (var product in info.Products) {
                string aa = product.LeftName, bb = product.RightName;
                bool primary = product.IsPrimary;
                if (!meta.ContainsKey(aa) || !meta.ContainsKey(bb)) continue;
                // C = A × B and B × A — coherent product of the two operands.
                Intent(aa, "*", aa, bb,
                    $"{CoherentVal(aa, "a.CanonicalValue")} * {CoherentVal(bb, "b.CanonicalValue")}", c, primary);
                if (aa != bb)
                    Intent(bb, "*", bb, aa,
                        $"{CoherentVal(bb, "a.CanonicalValue")} * {CoherentVal(aa, "b.CanonicalValue")}", c,
                        primary);
                // A = C / B, and B = C / A — coherent quotient (a is C, b is the divisor).
                Intent(c, "/", c, aa,
                    $"{CoherentVal(c, "a.CanonicalValue")} / {CoherentVal(aa, "b.CanonicalValue")}", bb, false);
                if (aa != bb)
                    Intent(c, "/", c, bb,
                        $"{CoherentVal(c, "a.CanonicalValue")} / {CoherentVal(bb, "b.CanonicalValue")}", aa, false);
            }
        }

        // Emit each intent: a single result → a direct operator; multiple → a selector operator plus
        // a selector struct whose type-named properties pick the intended result (f×l).Energy / .Torque.
        var selectors = new Dictionary<string, List<(string Name, bool Primary)>>();

        string SelName(string op, string p1, string p2) {
            if (op == "*") {
                var a = new[] { p1, p2 };
                Array.Sort(a, StringComparer.Ordinal);
                return a[0] + a[1] + "Product";
            }

            return p1 + "Over" + p2 + "Quotient";
        }

        foreach (var it in intents.Values) {
            if (it.Results.Count == 1) {
                string r = it.Results[0].Name;
                Body(it.Container)
                    .Append(
                        $"    public static {r} operator {it.Op}({it.P1} a, {it.P2} b) => {r}.FromCanonical({FromCoherentVal(r, it.Expr)});\n");
            } else {
                string sel = SelName(it.Op, it.P1, it.P2);
                if (!selectors.ContainsKey(sel)) selectors[sel] = it.Results;
                Body(it.Container)
                    .Append($"    public static {sel} operator {it.Op}({it.P1} a, {it.P2} b) => new({it.Expr});\n");
            }
        }

        if (byContainer.Count == 0) return;
        var src = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns + " {\n");
        foreach (var kv in selectors) {
            var results = kv.Value;
            string primary = results.Find(x => x.Primary).Name; // null when none is marked
            var names = new List<string>();
            foreach (var r in results) names.Add(r.Name);
            src.Append(primary != null
                ? $"    // Ambiguous product — implicitly a {primary}; also (a * b).{string.Join(" / (a * b).", names)}\n"
                : $"    // Ambiguous product — pick the result: (a * b).{string.Join(" / (a * b).", names)}\n");
            src.Append($"    public readonly struct {kv.Key} {{\n");
            src.Append("        private readonly double v;\n");
            src.Append($"        internal {kv.Key}(double v) => this.v = v;\n");
            foreach (var r in names)
                src.Append($"        public {r} {r} => {r}.FromCanonical({FromCoherentVal(r, "v")});\n");
            if (primary != null)
                src.Append($"        public static implicit operator {primary}({kv.Key} s) => s.{primary};\n");
            src.Append("    }\n");
        }

        foreach (var kv in byContainer) {
            src.Append($"    public readonly partial struct {kv.Key} {{\n");
            src.Append(kv.Value);
            src.Append("    }\n");
        }

        src.Append("}\n");
        ctx.AddSource("_Operators.g.cs", SourceText.From(src.ToString(), Encoding.UTF8));
    }

    // Emits the cross-type reciprocal reads declared by [Reciprocal] relations. For S = 1 / P (in
    // coherent SI) it writes `reader.To.<Name>` on Reader<S> returning P and the reverse read on
    // Reader<P> returning S, each computed as P.FromCanonical(1 / S) through the shared coherent-SI
    // conversion so DisplayFactor/anchor scaling stays exact. Replaces the handwritten reciprocal
    // From/To methods (Frequency↔Duration, Wavenumber↔Length, Conductivity↔Resistivity, …).
    private static void EmitReciprocals(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var meta = new Dictionary<string, (double DF, int G)>();
        foreach (var info in all) meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));

        (int Ten, double Res) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);

        // Value of `expr` (a canonical value of type `from`) reciprocated and expressed in the
        // canonical of type `to`: coherent(from) → 1/x → canonical(to).
        string Recip(string expr, string from, string to) =>
            FromCoherentExpr($"1 / ({ToCoherentExpr(expr, Div(from))})", Div(to));

        var body = new StringBuilder();
        foreach (var info in all) {
            if (info.Reciprocals.Count == 0) continue;
            foreach (var reciprocal in info.Reciprocals) {
                string partner = reciprocal.Partner, name = reciprocal.ReadName ?? partner;
                if (!meta.ContainsKey(info.Name) || !meta.ContainsKey(partner)) continue;
                // forward: reader.To.<name> on this type → partner; reverse: reader.To.<thisType> → this.
                body.Append(
                    $"        extension(Reader<{info.Name}> r) {{ public {partner} {name} => {partner}.FromCanonical({Recip("r.Value.CanonicalValue", info.Name, partner)}); }}\n");
                body.Append(
                    $"        extension(Reader<{partner}> r) {{ public {info.Name} {info.Name} => {info.Name}.FromCanonical({Recip("r.Value.CanonicalValue", partner, info.Name)}); }}\n");
            }
        }

        if (body.Length == 0) return;
        string source = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
{body}    }}
}}
";
        ctx.AddSource("_Reciprocal.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    // Shared model for the `.Per` walk (both input and read-out). Returns per-type metadata plus, for
    // every measurement type, the list of its non-affine single-word units is expressed as a denominator
    // option = (prefix-decomposed, singularized word path; the unprefixed base unit name). Also, a map
    // from any denominator word (singular or plural, prefixed or not) to its owning type, so the type
    // sequence of a compound unit's denominator can be recovered.
    private static (Dictionary<string, (double DF, int G)> Meta,
        Dictionary<string, string> TypeOfUnit,
        Dictionary<string, string> UnitWordType,
        Dictionary<string, List<Opt>> UnitOpts)
        BuildPerCommon(ImmutableArray<MeasurementInfo> all) {
        var meta = new Dictionary<string, (double, int)>();
        var typeOfUnit = new Dictionary<string, string>();
        var unitWordType = new Dictionary<string, string>();
        var unitOpts = new Dictionary<string, List<Opt>>();

        string StripS(string s) => s.Length > 1 && s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s;

        foreach (var info in all) {
            meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));
            var affine = new HashSet<string>();
            foreach (var unit in info.Units) {
                if (unit is UnitSpec u && u.Offset != 0)
                    affine.Add(u.Name);
            }

            var names = EnumerateUnitNames(info.Units);
            foreach (var n in names)
                if (!typeOfUnit.ContainsKey(n))
                    typeOfUnit[n] = info.Name;
            var single = new List<string>();
            foreach (var n in names)
                if (SplitWords(n).Count == 1)
                    single.Add(n);
            foreach (var n in single) {
                if (!unitWordType.ContainsKey(n)) unitWordType[n] = info.Name;
                string sn = StripS(n);
                if (!unitWordType.ContainsKey(sn)) unitWordType[sn] = info.Name;
            }

            var rawSet = new HashSet<string>(single);
            var opts = new List<Opt>();
            foreach (var n in single) {
                if (affine.Contains(n)) continue;
                var dec = DecomposeDenom(n, rawSet);
                string baseUnit = dec[dec.Count - 1];
                var path = new List<string>();
                for (int i = 0; i < dec.Count - 1; i++) path.Add(dec[i]);
                path.Add(StripS(baseUnit));
                opts.Add(new Opt(path, baseUnit));
            }

            unitOpts[info.Name] = opts;
        }

        return (meta, typeOfUnit, unitWordType, unitOpts);
    }

    // Emits the compositional `.Per` denominator walk (input side). `.Per` on a numerator measurement
    // enters a running-value trie; each denominator step divides the running coherent value by one unit
    // of the chosen dimension, so ANY (non-affine) unit of a slot's dimension composes — not only the
    // ones literally spelled in a compound unit. A completed denominator type-sequence implicitly
    // converts to the measurement that names it. `.Squared`/`.Cubed` repeat the last unit; a further
    // `.Per` (or, for a differently typed factor, a bare unit word) appends another denominator.
    private static void EmitPerBuilders(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var (meta, typeOfUnit, unitWordType, unitOpts) = BuildPerCommon(all);
        var removable = BuildRemovable(all);

        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        // Amount-of-substance (Quantity) stores an Avogadro count, but molar result types (g/mol, …)
        // treat the mole as the amount unit, so normalize the Quantity dimension to moles.
        bool hasMole = meta.ContainsKey("Quantity");

        string CoherentVal(string t, string valExpr) =>
            t == "Quantity" && hasMole
                ? $"({valExpr} / (Quantity.FromMoles(1).CanonicalValue))"
                : ToCoherentExpr(valExpr, Div(t));

        string FromCoherentVal(string t, string expr) =>
            t == "Quantity" && hasMole
                ? $"({expr} * (Quantity.FromMoles(1).CanonicalValue))"
                : FromCoherentExpr(expr, Div(t));

        string CUnit(string d, string baseUnit) =>
            CoherentVal(d, $"({Cons(d, d, baseUnit, "1", removable)}.CanonicalValue)");

        // `.Square`/`.Cubic` denominator modifiers: an areal/cubic unit spelled as a length sub-walk
        // (`.Cubic.Centi.Meter`), mapped to the Area/Volume unit for its exact coherent value.
        var mod = BuildDenomModTrie(all, meta);

        // numeratorType → (denominator type-sequence → result measurements). Squared/Cubed expand the
        // preceding factor type; the numerator split takes the longest leading unit spelling.
        var seqMap = new Dictionary<string, Dictionary<string, List<string>>>();
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units)) {
                var ws = SplitWords(u);
                if (!ws.Contains("Per") || PerExcludeUnits.Contains(u)) continue;
                int per = -1;
                for (int k = ws.Count - 1; k >= 1; k--) {
                    if (ws[k] != "Per") continue;
                    if (typeOfUnit.ContainsKey(string.Concat(ws.GetRange(0, k)))) {
                        per = k;
                        break;
                    }
                }

                if (per < 0 || per == ws.Count - 1) continue;
                string n = typeOfUnit[string.Concat(ws.GetRange(0, per))];
                var types = new List<string>();
                string lastT = "";
                bool ok = true;
                for (int k = per + 1; k < ws.Count; k++) {
                    string w = ws[k];
                    if (w == "Squared") {
                        if (lastT.Length == 0) {
                            ok = false;
                            break;
                        }

                        types.Add(lastT);
                    } else if (w == "Cubed") {
                        if (lastT.Length == 0) {
                            ok = false;
                            break;
                        }

                        types.Add(lastT);
                        types.Add(lastT);
                    } else if (w == "Square" || w == "Cubic") {
                        string t = w == "Square" ? "Area" : "Volume";
                        if (!meta.ContainsKey(t) || k + 1 >= ws.Count) {
                            ok = false;
                            break;
                        }

                        k++;
                        types.Add(t);
                        lastT = t;
                    } else {
                        if (!unitWordType.TryGetValue(w, out var t)) {
                            ok = false;
                            break;
                        }

                        types.Add(t);
                        lastT = t;
                    }
                }

                if (!ok || types.Count == 0) continue;
                string key = string.Join(",", types);
                if (!seqMap.TryGetValue(n, out var inner)) seqMap[n] = inner = new();
                if (!inner.TryGetValue(key, out var rl)) inner[key] = rl = new List<string>();
                if (!rl.Contains(info.Name)) rl.Add(info.Name);
            }

        if (seqMap.Count == 0) return;

        string Su(List<string> s) => string.Join("_", s);

        List<string> TerminalR(string n, List<string> s) =>
            seqMap[n].TryGetValue(string.Join(",", s), out var rl) ? rl : new List<string>();

        var sb = new StringBuilder();
        var emittedCol = new HashSet<string>();
        var emittedPost = new HashSet<string>();
        var emittedMod = new HashSet<string>();

        string EmitMod(string n, List<string> s, string kw) {
            var (partner, term, childs) = mod[kw];
            int power = kw == "Square" ? 2 : 3;
            string mid = "M_" + kw + "_" + n + (s.Count == 0 ? "" : "_" + Su(s));
            string root = mid + "__";
            if (!emittedMod.Add(mid)) return root;
            childs.TryGetValue("", out var rootSet);
            // areal base word(s) reached via a declared SI prefix (Meter, from SquareMillimeters →
            // Milli·Meter) → collapsed into a shared prefixed entry offering the FULL SI range, folding
            // the scaling into pfx as an exact power of ten (square ⇒ 2·e, cubic ⇒ 3·e).
            var prefixable = new SortedSet<string>(StringComparer.Ordinal);
            if (rootSet != null)
                foreach (var w in rootSet)
                    if (PrefixExponentsWithoutNone.ContainsKey(w) && childs.TryGetValue(w, out var gk))
                        foreach (var b in gk)
                            if (term.ContainsKey(b))
                                prefixable.Add(b);

            bool IsPrefixNode(string p) => p.Length > 0 && PrefixExponentsWithoutNone.ContainsKey(p.Split('_')[0]);

            string Terminal(string w, string unit) {
                string cu = CUnit(partner, unit);
                string tgt = EmitPost(n, [.. s, partner]);
                return $"        public {tgt} {w} => new(run / (pfx * {cu}), pfx * {cu}, 1.0);\n";
            }

            var progs = new HashSet<string> { "" };
            foreach (var kv in childs) {
                progs.Add(kv.Key);
                foreach (var w in kv.Value) progs.Add(kv.Key.Length == 0 ? w : kv.Key + "_" + w);
            }

            var ordered = new List<string>(progs);
            ordered.Sort(StringComparer.Ordinal);
            foreach (var prog in ordered) {
                if (IsPrefixNode(prog)) continue; // per-prefix nodes replaced by the shared entry
                var node = new StringBuilder();
                string nn = mid + "__" + prog;
                node.Append($"    public readonly struct {nn} {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append(
                    $"        internal {nn}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                if (childs.TryGetValue(prog, out var set))
                    foreach (var w in set) {
                        if (prog.Length == 0 && PrefixExponentsWithoutNone.ContainsKey(w))
                            continue; // collapsed SI-prefix child
                        string cp = prog.Length == 0 ? w : prog + "_" + w;
                        node.Append(term.TryGetValue(cp, out var unit)
                            ? Terminal(w, unit)
                            : $"        public {mid}__{cp} {w} => new(run, last, pfx);\n");
                    }

                if (prog.Length == 0 && prefixable.Count > 0)
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        node.Append(
                            $"        public {mid}__Pre {px} => new(run, last, pfx * 1e{power * PrefixExponents[px]});\n");
                node.Append("    }\n");
                sb.Append(node);
            }

            if (prefixable.Count > 0) {
                var node = new StringBuilder();
                node.Append($"    public readonly struct {mid}__Pre {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append(
                    $"        internal {mid}__Pre(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                foreach (var b in prefixable) node.Append(Terminal(b, term[b]));
                node.Append("    }\n");
                sb.Append(node);
            }

            return root;
        }

        string EmitPost(string n, List<string> s) {
            string post = "P_" + n + "_" + Su(s);
            if (!emittedPost.Add(post)) return post;
            var body = new StringBuilder();
            body.Append($"    public readonly struct {post} {{\n");
            body.Append($"        private readonly double run, last, pfx;\n");
            body.Append(
                $"        internal {post}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
            var rl = TerminalR(n, s);
            if (rl.Count == 1)
                body.Append(
                    $"        public static implicit operator {rl[0]}({post} n) => {rl[0]}.FromCanonical({FromCoherentVal(rl[0], "n.run")});\n");
            else if (rl.Count > 1)
                foreach (var r in rl)
                    body.Append($"        public {r} {r} => {r}.FromCanonical({FromCoherentVal(r, "run")});\n");
            string sLast = s[s.Count - 1];
            var nxt = AllowedNext(seqMap[n].Keys, s);
            if (nxt.Contains(sLast)) {
                var s1 = new List<string>(s) { sLast };
                body.Append($"        public {EmitPost(n, s1)} Squared => new(run / last, last, 1.0);\n");
                if (AllowedNext(seqMap[n].Keys, s1).Contains(sLast)) {
                    var s2 = new List<string>(s1) { sLast };
                    body.Append($"        public {EmitPost(n, s2)} Cubed => new(run / last / last, last, 1.0);\n");
                }
            }

            var prodTypes = nxt.FindAll(t => t != sLast);
            if (prodTypes.Count > 0) {
                EmitCol(n, s, "prod", prodTypes);
                string colId = "C_" + n + "_prod" + (s.Count == 0 ? "" : "_" + Su(s));
                BuildDenomTrie(prodTypes, unitOpts, out _, out var baseC);
                if (baseC.TryGetValue("", out var bl))
                    foreach (var (w, plural, d, baseUnit) in bl) {
                        string cu = CUnit(d, baseUnit);
                        string tgt = EmitPost(n, [.. s, d]);
                        string ctor = $"new(run / (1.0 * {cu}), 1.0 * {cu}, 1.0)";
                        body.Append($"        public {tgt} {w} => {ctor};\n");
                        if (plural != w) body.Append($"        public {tgt} {plural} => {ctor};\n");
                    }

                if (baseC.Any(kv =>
                        kv.Key.Length > 0 && kv.Value.Count > 0)) // full SI range → shared prefixed entry
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        body.Append(
                            $"        public {colId}__Pre {px} => new(run, last, 1e{PrefixExponents[px]});\n");
                if (prodTypes.Contains("Area") && mod.ContainsKey("Square"))
                    body.Append($"        public {EmitMod(n, s, "Square")} Square => new(run, last, 1.0);\n");
                if (prodTypes.Contains("Volume") && mod.ContainsKey("Cubic"))
                    body.Append($"        public {EmitMod(n, s, "Cubic")} Cubic => new(run, last, 1.0);\n");
            }

            if (nxt.Count > 0)
                body.Append($"        public {EmitCol(n, s, "chain", nxt)} Per => new(run, last, 1.0);\n");
            body.Append("    }\n");
            sb.Append(body);
            return post;
        }

        string EmitCol(string n, List<string> ss, string tag, List<string> types) {
            string colId = "C_" + n + "_" + tag + (ss.Count == 0 ? "" : "_" + Su(ss));
            string root = colId + "__";
            if (!emittedCol.Add(colId)) return root;
            BuildDenomTrie(types, unitOpts, out _, out var baseC);
            // Prefixable denominator bases = those reachable after any SI prefix (Milligrams → Grams).
            // The per-prefix nodes are collapsed into one shared `__Pre` entry offering these bases; the
            // root exposes the FULL SI range as properties that scale `pfx` and re-enter that entry.
            var prefBases = new List<BaseC>();
            var seenPb = new HashSet<string>();
            foreach (var kv in baseC)
                if (kv.Key.Length > 0)
                    foreach (var e in kv.Value)
                        if (seenPb.Add(e.W))
                            prefBases.Add(e);
            prefBases.Sort((a, b) => string.CompareOrdinal(a.W, b.W));

            string BaseChildren(IEnumerable<BaseC> bl) {
                var s = new StringBuilder();
                foreach (var (w, plural, d, baseUnit) in bl) {
                    string cu = CUnit(d, baseUnit);
                    string tgt = EmitPost(n, [.. ss, d]);
                    string ctor = $"new(run / (pfx * {cu}), pfx * {cu}, 1.0)";
                    s.Append($"        public {tgt} {w} => {ctor};\n");
                    if (plural != w) s.Append($"        public {tgt} {plural} => {ctor};\n");
                }

                return s.ToString();
            }

            var node = new StringBuilder();
            node.Append($"    public readonly struct {root} {{\n");
            node.Append($"        private readonly double run, last, pfx;\n");
            node.Append(
                $"        internal {root}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
            if (baseC.TryGetValue("", out var bl0)) node.Append(BaseChildren(bl0));
            if (prefBases.Count > 0)
                foreach (var px in PrefixExponentsWithoutNone.Keys)
                    node.Append(
                        $"        public {colId}__Pre {px} => new(run, last, pfx * 1e{PrefixExponents[px]});\n");
            if (types.Contains("Area") && mod.ContainsKey("Square"))
                node.Append($"        public {EmitMod(n, ss, "Square")} Square => new(run, last, pfx);\n");
            if (types.Contains("Volume") && mod.ContainsKey("Cubic"))
                node.Append($"        public {EmitMod(n, ss, "Cubic")} Cubic => new(run, last, pfx);\n");
            node.Append("    }\n");
            sb.Append(node);
            if (prefBases.Count > 0) {
                node = new StringBuilder();
                node.Append($"    public readonly struct {colId}__Pre {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append(
                    $"        internal {colId}__Pre(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                node.Append(BaseChildren(prefBases));
                node.Append("    }\n");
                sb.Append(node);
            }

            return root;
        }

        var ext = new StringBuilder();
        foreach (var n in new SortedSet<string>(seqMap.Keys, StringComparer.Ordinal)) {
            string root = EmitCol(n, new List<string>(), "root", AllowedNext(seqMap[n].Keys, new List<string>()));
            ext.Append(
                $"        extension({n} x) {{ public {root} Per => new({CoherentVal(n, "x.CanonicalValue")}, 0.0, 1.0); }}\n");
        }

        var outSrc = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns +
                                       " {\n    public static partial class Units {\n");
        outSrc.Append(ext);
        outSrc.Append("    }\n");
        outSrc.Append(sb);
        outSrc.Append("}\n");
        ctx.AddSource("_Per.g.cs", SourceText.From(outSrc.ToString(), Encoding.UTF8));
    }

    // Read-out mirror of EmitPerBuilders: `reader.To.<flatNumerator>.Per.<denominator…>` reads a
    // double. The numerator is a flat spelling on Reader<C> (its coherent value seeds the running
    // product); each denominator step multiplies by one coherent unit of the chosen dimension, so any
    // (non-affine) unit composes. Squared/Cubed repeat the last unit; `.Square`/`.Cubic` and bare
    // product factors (`.Per.Kilo.Gram.Kelvin`) extend the denominator. A completed type-sequence
    // returns the value in those units via implicit conversion to double.
    private static void EmitPerReadBuilders(SourceProductionContext ctx,
        ImmutableArray<MeasurementInfo> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var (meta, typeOfUnit, unitWordType, unitOpts) = BuildPerCommon(all);
        var removable = BuildRemovable(all);
        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        bool hasMole = meta.ContainsKey("Quantity");

        string CoherentVal(string t, string valExpr) =>
            t == "Quantity" && hasMole
                ? $"({valExpr} / (Quantity.FromMoles(1).CanonicalValue))"
                : ToCoherentExpr(valExpr, Div(t));

        string CUnit(string d, string baseUnit) =>
            CoherentVal(d, $"({Cons(d, d, baseUnit, "1", removable)}.CanonicalValue)");

        var mod = BuildDenomModTrie(all, meta);

        string Su(List<string> s) => string.Join("_", s);

        List<string> ParseDenomTypes(List<string> ws, int per) {
            var types = new List<string>();
            string lastT = "";
            bool ok = true;
            for (int k = per + 1; k < ws.Count; k++) {
                string w = ws[k];
                if (w == "Per") continue;
                if (w == "Squared") {
                    if (lastT.Length == 0) return new List<string>();
                    types.Add(lastT);
                } else if (w == "Cubed") {
                    if (lastT.Length == 0) return new List<string>();
                    types.Add(lastT);
                    types.Add(lastT);
                } else if (w == "Square" || w == "Cubic") {
                    string t = w == "Square" ? "Area" : "Volume";
                    if (!meta.ContainsKey(t) || k + 1 >= ws.Count) return new List<string>();
                    k++;
                    types.Add(t);
                    lastT = t;
                } else {
                    if (!unitWordType.TryGetValue(w, out var t)) {
                        ok = false;
                        break;
                    }

                    types.Add(t);
                    lastT = t;
                }
            }

            return ok && types.Count > 0 ? types : new List<string>();
        }

        // readMap[C][numFlat] = (numType, set of denominator type-sequences).
        var readMap = new Dictionary<string, Dictionary<string, (string NumType, HashSet<string> Seqs)>>();
        foreach (var info in all) {
            string c = info.Name;
            var names = EnumerateUnitNames(info.Units);
            var simple = new HashSet<string>();
            foreach (var u in names)
                if (!SplitWords(u).Contains("Per"))
                    simple.Add(u);
            foreach (var u in names) {
                var ws = SplitWords(u);
                int per = ws.IndexOf("Per");
                if (per <= 0 || per == ws.Count - 1 || PerExcludeUnits.Contains(u)) continue;
                string numFlat = string.Concat(ws.GetRange(0, per));
                if (simple.Contains(numFlat) || !typeOfUnit.TryGetValue(numFlat, out var numType)) continue;
                var types = ParseDenomTypes(ws, per);
                if (types.Count == 0) continue;
                if (!readMap.TryGetValue(c, out var byNum)) readMap[c] = byNum = new();
                if (!byNum.TryGetValue(numFlat, out var e)) byNum[numFlat] = e = (numType, new HashSet<string>());
                e.Seqs.Add(string.Join(",", types));
            }
        }

        if (readMap.Count == 0) return;

        var starts = new StringBuilder();
        var sb = new StringBuilder();
        var emittedCol = new HashSet<string>();
        var emittedPost = new HashSet<string>();
        var emittedMod = new HashSet<string>();

        // per (C, numFlat) helpers close over the current cn = C + "_" + numFlat key.
        foreach (var c in new SortedSet<string>(readMap.Keys, StringComparer.Ordinal)) {
            foreach (var numFlat in new SortedSet<string>(readMap[c].Keys, StringComparer.Ordinal)) {
                var (numType, seqs) = readMap[c][numFlat];
                string cn = c + "_" + numFlat;

                bool IsTerminal(List<string> s) => seqs.Contains(string.Join(",", s));

                string EmitMod(List<string> s, string kw) {
                    var (partner, term, childs) = mod[kw];
                    int power = kw == "Square" ? 2 : 3;
                    string mid = "RM_" + kw + "_" + cn + (s.Count == 0 ? "" : "_" + Su(s));
                    string root = mid + "__";
                    if (!emittedMod.Add(mid)) return root;
                    childs.TryGetValue("", out var rootSet);
                    var prefixable = new SortedSet<string>(StringComparer.Ordinal);
                    if (rootSet != null)
                        foreach (var w in rootSet)
                            if (PrefixExponentsWithoutNone.ContainsKey(w) && childs.TryGetValue(w, out var gk))
                                foreach (var b in gk)
                                    if (term.ContainsKey(b))
                                        prefixable.Add(b);

                    bool IsPrefixNode(string p) => p.Length > 0 && PrefixExponentsWithoutNone.ContainsKey(p.Split('_')[0]);

                    string Terminal(string w, string unit) {
                        string cu = CUnit(partner, unit);
                        string tgt = EmitPostRef([.. s, partner]);
                        return $"        public {tgt} {w} => new(run * (pfx * {cu}), pfx * {cu}, 1.0);\n";
                    }

                    var progs = new HashSet<string> { "" };
                    foreach (var kv in childs) {
                        progs.Add(kv.Key);
                        foreach (var w in kv.Value) progs.Add(kv.Key.Length == 0 ? w : kv.Key + "_" + w);
                    }

                    var ordered = new List<string>(progs);
                    ordered.Sort(StringComparer.Ordinal);
                    foreach (var prog in ordered) {
                        if (IsPrefixNode(prog)) continue; // per-prefix nodes replaced by the shared entry
                        var node = new StringBuilder();
                        string nn = mid + "__" + prog;
                        node.Append($"    public readonly struct {nn} {{\n");
                        node.Append($"        private readonly double run, last, pfx;\n");
                        node.Append(
                            $"        internal {nn}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                        if (childs.TryGetValue(prog, out var set))
                            foreach (var w in set) {
                                if (prog.Length == 0 && PrefixExponentsWithoutNone.ContainsKey(w))
                                    continue; // collapsed SI-prefix child
                                string cp = prog.Length == 0 ? w : prog + "_" + w;
                                node.Append(term.TryGetValue(cp, out var unit)
                                    ? Terminal(w, unit)
                                    : $"        public {mid}__{cp} {w} => new(run, last, pfx);\n");
                            }

                        if (prog.Length == 0 && prefixable.Count > 0)
                            foreach (var px in PrefixExponentsWithoutNone.Keys)
                                node.Append(
                                    $"        public {mid}__Pre {px} => new(run, last, pfx * 1e{power * PrefixExponents[px]});\n");
                        node.Append("    }\n");
                        sb.Append(node);
                    }

                    if (prefixable.Count > 0) {
                        var node = new StringBuilder();
                        node.Append($"    public readonly struct {mid}__Pre {{\n");
                        node.Append($"        private readonly double run, last, pfx;\n");
                        node.Append(
                            $"        internal {mid}__Pre(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                        foreach (var b in prefixable) node.Append(Terminal(b, term[b]));
                        node.Append("    }\n");
                        sb.Append(node);
                    }

                    return root;
                }

                string EmitCol(List<string> ss, List<string> types) {
                    string colId = "RC_" + cn + (ss.Count == 0 ? "" : "_" + Su(ss));
                    string root = colId + "__";
                    if (!emittedCol.Add(colId)) return root;
                    BuildDenomTrie(types, unitOpts, out _, out var baseC);
                    // SI-prefixable denominator bases collapse into one shared `__Pre`; the root exposes
                    // the full SI range as properties that scale pfx and re-enter it (read multiplies).
                    var prefBases = new List<BaseC>();
                    var seenPb = new HashSet<string>();
                    foreach (var kv in baseC)
                        if (kv.Key.Length > 0)
                            foreach (var e in kv.Value)
                                if (seenPb.Add(e.W))
                                    prefBases.Add(e);
                    prefBases.Sort((a, b) => string.CompareOrdinal(a.W, b.W));

                    string BaseChildren(IEnumerable<BaseC> bl) {
                        var s = new StringBuilder();
                        foreach (var (w, plural, d, baseUnit) in bl) {
                            string cu = CUnit(d, baseUnit);
                            string tgt = EmitPostRef([.. ss, d]);
                            string ctor = $"new(run * (pfx * {cu}), pfx * {cu}, 1.0)";
                            s.Append($"        public {tgt} {w} => {ctor};\n");
                            if (plural != w) s.Append($"        public {tgt} {plural} => {ctor};\n");
                        }

                        return s.ToString();
                    }

                    var node = new StringBuilder();
                    node.Append($"    public readonly struct {root} {{\n");
                    node.Append($"        private readonly double run, last, pfx;\n");
                    node.Append(
                        $"        internal {root}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                    if (baseC.TryGetValue("", out var bl0)) node.Append(BaseChildren(bl0));
                    if (prefBases.Count > 0)
                        foreach (var px in PrefixExponentsWithoutNone.Keys)
                            node.Append(
                                $"        public {colId}__Pre {px} => new(run, last, pfx * 1e{PrefixExponents[px]});\n");
                    if (types.Contains("Area") && mod.ContainsKey("Square"))
                        node.Append($"        public {EmitMod(ss, "Square")} Square => new(run, last, pfx);\n");
                    if (types.Contains("Volume") && mod.ContainsKey("Cubic"))
                        node.Append($"        public {EmitMod(ss, "Cubic")} Cubic => new(run, last, pfx);\n");
                    node.Append("    }\n");
                    sb.Append(node);
                    if (prefBases.Count > 0) {
                        node = new StringBuilder();
                        node.Append($"    public readonly struct {colId}__Pre {{\n");
                        node.Append($"        private readonly double run, last, pfx;\n");
                        node.Append(
                            $"        internal {colId}__Pre(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                        node.Append(BaseChildren(prefBases));
                        node.Append("    }\n");
                        sb.Append(node);
                    }

                    return root;
                }

                string EmitPostRef(List<string> s) {
                    string post = "RP_" + cn + "_" + Su(s);
                    if (!emittedPost.Add(post)) return post;
                    var body = new StringBuilder();
                    body.Append($"    public readonly struct {post} {{\n");
                    body.Append($"        private readonly double run, last, pfx;\n");
                    body.Append(
                        $"        internal {post}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                    if (IsTerminal(s))
                        body.Append($"        public static implicit operator double({post} n) => n.run;\n");
                    string sLast = s[s.Count - 1];
                    var nxt = AllowedNext(seqs, s);
                    if (nxt.Contains(sLast)) {
                        var s1 = new List<string>(s) { sLast };
                        body.Append($"        public {EmitPostRef(s1)} Squared => new(run * last, last, 1.0);\n");
                        if (AllowedNext(seqs, s1).Contains(sLast)) {
                            var s2 = new List<string>(s1) { sLast };
                            body.Append(
                                $"        public {EmitPostRef(s2)} Cubed => new(run * last * last, last, 1.0);\n");
                        }
                    }

                    if (nxt.Count > 0) {
                        EmitCol(s, nxt);
                        string colId = "RC_" + cn + "_" + Su(s);
                        BuildDenomTrie(nxt, unitOpts, out _, out var baseC);
                        if (baseC.TryGetValue("", out var bl))
                            foreach (var (w, plural, d, baseUnit) in bl) {
                                string cu = CUnit(d, baseUnit);
                                string tgt = EmitPostRef([.. s, d]);
                                string ctor = $"new(run * (1.0 * {cu}), 1.0 * {cu}, 1.0)";
                                body.Append($"        public {tgt} {w} => {ctor};\n");
                                if (plural != w) body.Append($"        public {tgt} {plural} => {ctor};\n");
                            }

                        if (baseC.Any(kv =>
                                kv.Key.Length > 0 && kv.Value.Count > 0)) // full SI range → shared prefixed entry
                            foreach (var px in PrefixExponentsWithoutNone.Keys)
                                body.Append(
                                    $"        public {colId}__Pre {px} => new(run, last, 1e{PrefixExponents[px]});\n");
                        if (nxt.Contains("Area") && mod.ContainsKey("Square"))
                            body.Append($"        public {EmitMod(s, "Square")} Square => new(run, last, 1.0);\n");
                        if (nxt.Contains("Volume") && mod.ContainsKey("Cubic"))
                            body.Append($"        public {EmitMod(s, "Cubic")} Cubic => new(run, last, 1.0);\n");
                    }

                    body.Append("    }\n");
                    sb.Append(body);
                    return post;
                }

                string start = "RS_" + cn;
                string numCoh = CUnit(numType, numFlat);
                string runStart = $"(({ToCoherentExpr("r.Value.CanonicalValue", Div(c))}) / ({numCoh}) / r.Factor)";
                starts.Append(
                    $"        extension(Reader<{c}> r) {{ public {start} {numFlat} => new({runStart}, 0.0, 1.0); }}\n");
                string colRoot = EmitCol(new List<string>(), AllowedNext(seqs, new List<string>()));
                sb.Append($"    public readonly struct {start} {{\n");
                sb.Append($"        private readonly double run, last, pfx;\n");
                sb.Append(
                    $"        internal {start}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                sb.Append($"        public {colRoot} Per => new(run, last, pfx);\n");
                sb.Append("    }\n");
            }
        }

        var outSrc = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns +
                                       " {\n    public static partial class Units {\n");
        outSrc.Append(starts);
        outSrc.Append("    }\n");
        outSrc.Append(sb);
        outSrc.Append("}\n");
        ctx.AddSource("_PerRead.g.cs", SourceText.From(outSrc.ToString(), Encoding.UTF8));
    }

    // Product-unit token algebra (input side). Each measurement type reachable as a running product
    // becomes a `ProductState_<Type>` struct carrying the running canonical value and implicitly
    // convertible to that type. A first token on Prefixed/double (the singular base unit of a
    // product-participating type, plus `Light` = the speed of light) enters the corresponding state;
    // each subsequent single-word unit token multiplies by one of that unit and transitions to the
    // product type via the [Product] relations, with the exact operator factor. Dimensionally
    // ambiguous products (Force × Length → Torque or Energy) resolve to a selector naming each result.
    // So `.Ampere.Hours`, `.Joule.Minutes`, `.Light.Seconds`, `.Newton.Meters.Torque` all compose.
    private static void EmitTokenAlgebra(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var removable = BuildRemovable(all);
        var meta = new Dictionary<string, (double DF, int G)>();
        foreach (var info in all) meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));
        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);

        // Single-word unit tokens → (type, unit). Plural forms as declared; singular base unit per
        // type is the entry token. `Light` is a synthetic Speed token (the speed of light).
        var tokenType = new Dictionary<string, (string Type, string Unit)>(); // multiplying tokens
        var entryToken = new Dictionary<string, (string Type, string Unit)>(); // Prefixed entry tokens
        var baseUnit = new Dictionary<string, string>(); // type → its base unit
        var singleWords = new Dictionary<string, HashSet<string>>(); // type → its single-word units
        foreach (var info in all) {
            foreach (var u in EnumerateUnitNames(info.Units)) {
                if (SplitWords(u).Count != 1) continue;
                if (!tokenType.ContainsKey(u)) tokenType[u] = (info.Name, u);
                if (!baseUnit.ContainsKey(info.Name)) baseUnit[info.Name] = u; // first single-word unit
                if (!singleWords.TryGetValue(info.Name, out var set))
                    singleWords[info.Name] = set = new HashSet<string>();
                set.Add(u);
            }
        }

        // product pairs (unordered) → results, plus the Primary result of each pair (implicit target).
        var pair = new Dictionary<string, List<string>>();
        var primaryResult = new Dictionary<string, string>();

        void AddPair(string a, string b, string c, bool primary) {
            string k = string.CompareOrdinal(a, b) <= 0 ? a + "|" + b : b + "|" + a;
            if (!pair.TryGetValue(k, out var l)) pair[k] = l = new List<string>();
            if (!l.Contains(c)) l.Add(c);
            if (primary) primaryResult[k] = c;
        }

        foreach (var info in all)
            foreach (var product in info.Products)
                AddPair(product.LeftName, product.RightName, info.Name, product.IsPrimary);

        List<string> Results(string r, string t) {
            string k = string.CompareOrdinal(r, t) <= 0 ? r + "|" + t : t + "|" + r;
            return pair.TryGetValue(k, out var l) ? l : new List<string>();
        }

        string PrimaryOf(string r, string t) {
            string k = string.CompareOrdinal(r, t) <= 0 ? r + "|" + t : t + "|" + r;
            return primaryResult.TryGetValue(k, out var p) ? p : "";
        }

        // Entry tokens: the singular base unit of each product-participating type + Light. If a type's
        // base unit is itself an SI-prefixed name (Mass's coherent unit is the kilogram = Kilo+Grams),
        // decompose it so the entry token is the unprefixed singular (Gram), reached via the prefix
        // chain (`.Kilo.Gram`) — matching the rest of the fluent surface rather than a fused `.Kilogram`.
        var participating = new HashSet<string>();
        foreach (var k in pair.Keys) {
            var ab = k.Split('|');
            participating.Add(ab[0]);
            participating.Add(ab[1]);
        }

        foreach (var t in participating) {
            if (!baseUnit.TryGetValue(t, out var bu)) continue;
            singleWords.TryGetValue(t, out var sw);
            var dec = DecomposeDenom(bu, sw ?? new HashSet<string>());
            string factoryUnit = dec[dec.Count - 1]; // unprefixed base (Grams)
            string sing = factoryUnit.EndsWith("s")
                ? factoryUnit.Substring(0, factoryUnit.Length - 1)
                : factoryUnit;
            if (!tokenType.ContainsKey(sing) && !entryToken.ContainsKey(sing)) entryToken[sing] = (t, factoryUnit);
        }

        if (meta.ContainsKey("Speed")) entryToken["Light"] = ("Speed", "SpeedOfLight");

        // Reachable states via BFS over product transitions from the entry types.
        var states = new HashSet<string>();
        var queue = new Queue<string>();
        foreach (var e in entryToken.Values)
            if (states.Add(e.Type))
                queue.Enqueue(e.Type);
        while (queue.Count > 0) {
            string r = queue.Dequeue();
            foreach (var tt in tokenType.Values)
                foreach (var c in Results(r, tt.Type))
                    if (states.Add(c))
                        queue.Enqueue(c);
        }

        // Value of the running state R (canonical of R, via `field`) times one unit u of type T,
        // expressed in the canonical of result C (coherent product then back to C's stored scale).
        string Combine(string r, string t, string u, string c, string field) {
            string coR = ToCoherentExpr(field, Div(r));
            string coU = ToCoherentExpr($"({Cons(t, t, u, "1", removable)}.CanonicalValue)", Div(t));
            return FromCoherentExpr($"{coR} * {coU}", Div(c));
        }

        // Square/Cubic modifier tries over the Area/Volume units, so a running state R can be scaled
        // by an areal/cubic unit via R × Area / R × Volume (e.g., Mass.Square.Meters → MomentOfInertia).
        var modTrie =
            new Dictionary<string, (string Partner, Dictionary<string, string> Term,
                Dictionary<string, SortedSet<string>> Children, List<string> Nodes)>();
        var bases = SingleWordBases(all);
        foreach (var (mod, partner) in new[] { ("Square", "Area"), ("Cubic", "Volume") }) {
            if (!meta.ContainsKey(partner)) continue;
            var term = new Dictionary<string, string>();
            var childs = new Dictionary<string, SortedSet<string>>();
            var nds = new HashSet<string> { "" };
            var pinfo = all.First(a => a.Name == partner);
            foreach (var u in EnumerateUnitNames(pinfo.Units)) {
                var ws = SplitWords(u);
                if (ws.Count < 2 || ws[0] != mod) continue;
                var rest = new List<string>();
                for (int i = 1; i < ws.Count; i++) rest.AddRange(DecomposeDenom(ws[i], bases));
                string key = string.Join("_", rest);
                if (!term.ContainsKey(key)) term[key] = u;
                for (int k = 0; k <= rest.Count; k++) {
                    string p = string.Join("_", rest.GetRange(0, k));
                    nds.Add(p);
                    if (k < rest.Count) {
                        if (!childs.TryGetValue(p, out var set))
                            childs[p] = set = new SortedSet<string>(StringComparer.Ordinal);
                        set.Add(rest[k]);
                    }
                }
            }

            var ordered = new List<string>(nds);
            ordered.Sort(StringComparer.Ordinal);
            modTrie[mod] = (partner, term, childs, ordered);
        }

        var structs = new StringBuilder();
        var selectors = new StringBuilder();
        var seenSel = new HashSet<string>();
        var modStructs = new StringBuilder();
        foreach (var r in states.OrderBy(s => s, StringComparer.Ordinal)) {
            string nn = "ProductState_" + r;
            structs.Append($"    public readonly struct {nn} {{\n");
            structs.Append($"        internal readonly double c;\n");
            structs.Append($"        internal {nn}(double c) => this.c = c;\n");
            structs.Append($"        public static implicit operator {r}({nn} n) => {r}.FromCanonical(n.c);\n");
            // transitions, ordered by token
            foreach (var tok in tokenType.Keys.OrderBy(s => s, StringComparer.Ordinal)) {
                var (tt, tu) = tokenType[tok];
                var res = Results(r, tt);
                if (res.Count == 0) continue;
                if (res.Count == 1) {
                    string c = res[0];
                    structs.Append(
                        $"        public ProductState_{c} {tok} => new({Combine(r, tt, tu, c, "c")});\n");
                } else {
                    string sel = $"Sel_{r}_{tok}";
                    structs.Append($"        public {sel} {tok} => new(c);\n");
                    if (seenSel.Add(sel)) {
                        selectors.Append(
                            $"    public readonly struct {sel} {{\n        internal readonly double c;\n        internal {sel}(double c) => this.c = c;\n");
                        string prim = PrimaryOf(r, tt);
                        if (prim.Length != 0)
                            selectors.Append(
                                $"        public static implicit operator {prim}({sel} n) => global::{ns}.{prim}.FromCanonical({Combine(r, tt, tu, prim, "n.c")});\n");
                        foreach (var c in res)
                            selectors.Append(
                                $"        public ProductState_{c} {c} => new({Combine(r, tt, tu, c, "c")});\n");
                        selectors.Append("    }\n");
                    }
                }
            }

            // Square/Cubic modifiers: R × Area / R × Volume, when such a product exists.
            foreach (var mod in new[] { "Square", "Cubic" }) {
                if (!modTrie.TryGetValue(mod, out var mt) || Results(r, mt.Partner).Count == 0) continue;
                structs.Append($"        public {mod}Of_{r} {mod} => new(c);\n");
                foreach (var p in mt.Nodes) {
                    string mn = $"{mod}Of_{r}" + (p.Length == 0 ? "" : "_" + p);
                    modStructs.Append(
                        $"    public readonly struct {mn} {{\n        internal readonly double c;\n        internal {mn}(double c) => this.c = c;\n");
                    if (mt.Children.TryGetValue(p, out var set))
                        foreach (var w in set) {
                            string cp = p.Length == 0 ? w : p + "_" + w;
                            modStructs.Append($"        public {mod}Of_{r}_{cp} {w} => new(c);\n");
                        }

                    if (p.Length > 0 && mt.Term.TryGetValue(p, out var au)) {
                        var res = Results(r, mt.Partner);
                        if (res.Count == 1)
                            modStructs.Append(
                                $"        public static implicit operator {res[0]}({mn} n) => {res[0]}.FromCanonical({Combine(r, mt.Partner, au, res[0], "n.c")});\n");
                        else
                            foreach (var c in res)
                                modStructs.Append(
                                    $"        public ProductState_{c} {c} => new({Combine(r, mt.Partner, au, c, "c")});\n");
                    }

                    modStructs.Append("    }\n");
                }
            }

            structs.Append("    }\n");
        }

        structs.Append(modStructs);

        // Entry properties on Prefixed and double.
        var prefixed = new StringBuilder();
        var dbl = new StringBuilder();
        foreach (var tok in entryToken.Keys.OrderBy(s => s, StringComparer.Ordinal)) {
            var (ty, un) = entryToken[tok];
            prefixed.Append(
                $"        public ProductState_{ty} {tok} => new({Cons(ty, ty, un, "p.Value", removable)}.CanonicalValue);\n");
            dbl.Append(
                $"        public ProductState_{ty} {tok} => new({Cons(ty, ty, un, "value", removable)}.CanonicalValue);\n");
        }

        string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
        extension(Prefixed p) {{
{prefixed}        }}
    }}
{structs}{selectors}}}

namespace {ns}.Fluent {{
    using {ns};
    public static partial class DoubleSugar {{
        extension(double value) {{
{dbl}        }}
    }}
}}
";
        ctx.AddSource("_Product.g.cs", SourceText.From(src, Encoding.UTF8));
    }

    // Emits `.Square`/`.Cubic` area & volume modifiers on Prefixed/double. The areal/cubic base (Meters)
    // accepts the FULL SI prefix range, folded into the value as an exact power of ten (a prefix of ten
    // exponent e scales an n-th-power unit by 1e{n·e}) via a shared prefixed entry, so `.Square.Milli.Meters`,
    // `.Cubic.Kilo.Meters`, etc. compose without per-prefix nodes. Each base terminal maps to the
    // Square<Unit>/Cubic<Unit> factory; non-metric spellings (`.Square.Miles`, `.Square.Degrees`) stay direct.
    private static void EmitSquareCubic(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        // Known single-word unit bases (for prefix splitting Millimeters → Milli, Meters).
        var bases = SingleWordBases(all);

        // modifier ("Square"/"Cubic") → decomposed rest path → (type, full unit).
        foreach (var modifier in new[] { "Square", "Cubic" }) {
            int power = modifier == "Square" ? 2 : 3;
            var terminal = new Dictionary<string, (string Type, string Unit)>();
            var children = new Dictionary<string, SortedSet<string>>();
            var nodes = new HashSet<string> { "" };
            foreach (var info in all)
                foreach (var u in EnumerateUnitNames(info.Units)) {
                    var ws = SplitWords(u);
                    if (ws.Count < 2 || ws[0] != modifier || ws.Contains("Per")) continue;
                    var rest = new List<string>();
                    for (int i = 1; i < ws.Count; i++) rest.AddRange(DecomposeDenom(ws[i], bases));
                    string key = string.Join("_", rest);
                    if (!terminal.ContainsKey(key)) terminal[key] = (info.Name, u);
                    for (int k = 0; k <= rest.Count; k++) {
                        string p = string.Join("_", rest.GetRange(0, k));
                        nodes.Add(p);
                        if (k < rest.Count) {
                            if (!children.TryGetValue(p, out var set))
                                children[p] = set = new SortedSet<string>(StringComparer.Ordinal);
                            set.Add(rest[k]);
                        }
                    }
                }

            if (terminal.Count == 0) continue;
            string Node(string p) => modifier + "Of" + (p.Length == 0 ? "" : "_" + p);
            string preName = modifier + "Of__Pre";
            children.TryGetValue("", out var rootKids);

            bool IsPrefixNode(string p) => p.Length > 0 && PrefixExponentsWithoutNone.ContainsKey(p.Split('_')[0]);

            var prefixable = new SortedSet<string>(StringComparer.Ordinal);
            if (rootKids != null)
                foreach (var w in rootKids)
                    if (PrefixExponentsWithoutNone.ContainsKey(w) && children.TryGetValue(w, out var gk))
                        foreach (var b in gk)
                            if (terminal.ContainsKey(b))
                                prefixable.Add(b);
            var structs = new StringBuilder();
            var ordered = new List<string>(nodes);
            ordered.Sort(StringComparer.Ordinal);
            foreach (var p in ordered) {
                if (IsPrefixNode(p)) continue; // per-prefix nodes replaced by the shared entry
                string nn = Node(p);
                structs.Append(
                    $"    public readonly struct {nn} {{\n        private readonly double v;\n        internal {nn}(double v) => this.v = v;\n");
                if (children.TryGetValue(p, out var set))
                    foreach (var w in set)
                        if (!PrefixExponentsWithoutNone.ContainsKey(w)) // skip SI-prefix children (handled below)
                            structs.Append(
                                $"        public {Node(p.Length == 0 ? w : p + "_" + w)} {w} => new(v);\n");
                if (p.Length == 0 && prefixable.Count > 0) // full SI prefix range on the modifier entry
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        structs.Append(
                            $"        public {preName} {px} => new(v * 1e{power * PrefixExponents[px]});\n");
                if (p.Length > 0 && terminal.TryGetValue(p, out var t))
                    structs.Append(
                        $"        public static implicit operator {t.Type}({nn} n) => {t.Type}.From{t.Unit}(n.v);\n");
                structs.Append("    }\n");
            }

            if (prefixable.Count > 0) { // shared prefixed entry: base units, value pre-scaled
                structs.Append(
                    $"    public readonly struct {preName} {{\n        private readonly double v;\n        internal {preName}(double v) => this.v = v;\n");
                foreach (var w in prefixable)
                    structs.Append($"        public {Node(w)} {w} => new(v);\n");
                structs.Append("    }\n");
            }

            string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
        extension(Prefixed p) {{ public {Node("")} {modifier} => new(p.Value); }}
    }}
{structs}}}

namespace {ns}.Fluent {{
    using {ns};
    public static partial class DoubleSugar {{
        extension(double value) {{ public {Node("")} {modifier} => new(value); }}
    }}
}}
";
            ctx.AddSource("_" + modifier + ".g.cs", SourceText.From(src, Encoding.UTF8));
        }
    }

    // Read-out mirror of EmitSquareCubic: `reader.To.Square.Milli.Meters` / `.To.Cubic.Centi.Meters`
    // read a double. The areal/cubic base (Meters) accepts the FULL SI prefix range, folded into the
    // reader's running factor as an exact power of ten (a prefix of ten-exponent e scales an n-th-power
    // unit by 1e{n·e}) via a shared prefixed entry, so `.Square.Yotta.Meters` etc. compose too.
    // Non-metric spellings (Square.Miles, Cubic.Feet) remain direct terminals.
    private static void EmitSquareCubicRead(SourceProductionContext ctx, ImmutableArray<MeasurementInfo> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var bases = SingleWordBases(all);

        var starts = new StringBuilder();
        var structs = new StringBuilder();
        bool any = false;
        foreach (var info in all)
            foreach (var modifier in new[] { "Square", "Cubic" }) {
                string T = info.Name;
                int power = modifier == "Square" ? 2 : 3;
                var terminal = new Dictionary<string, string>(); // path -> full unit
                var children = new Dictionary<string, SortedSet<string>>();
                var nodes = new HashSet<string> { "" };
                foreach (var u in EnumerateUnitNames(info.Units)) {
                    var ws = SplitWords(u);
                    if (ws.Count < 2 || ws[0] != modifier || ws.Contains("Per")) continue;
                    var rest = new List<string>();
                    for (int i = 1; i < ws.Count; i++) rest.AddRange(DecomposeDenom(ws[i], bases));
                    string key = string.Join("_", rest);
                    if (!terminal.ContainsKey(key)) terminal[key] = u;
                    for (int k = 0; k <= rest.Count; k++) {
                        string p = string.Join("_", rest.GetRange(0, k));
                        nodes.Add(p);
                        if (k < rest.Count) {
                            if (!children.TryGetValue(p, out var set))
                                children[p] = set = new SortedSet<string>(StringComparer.Ordinal);
                            set.Add(rest[k]);
                        }
                    }
                }

                if (terminal.Count == 0) continue;
                any = true;
                string Node(string p) => "R" + modifier + "_" + T + (p.Length == 0 ? "" : "_" + p);
                string preName = "R" + modifier + "_" + T + "__Pre";
                children.TryGetValue("", out var rootKids);

                bool IsPrefixNode(string p) => p.Length > 0 && PrefixExponentsWithoutNone.ContainsKey(p.Split('_')[0]);

                // base words reachable via a declared SI prefix (Meters) and also present unprefixed:
                // these get the full SI range via the shared prefixed entry.
                var prefixable = new SortedSet<string>(StringComparer.Ordinal);
                if (rootKids != null)
                    foreach (var w in rootKids)
                        if (PrefixExponentsWithoutNone.ContainsKey(w) && children.TryGetValue(w, out var gk))
                            foreach (var b in gk)
                                if (terminal.ContainsKey(b))
                                    prefixable.Add(b);
                starts.Append(
                    $"        extension(Reader<{T}> r) {{ public {Node("")} {modifier} => new(r.Value, r.Factor); }}\n");
                var ordered = new List<string>(nodes);
                ordered.Sort(StringComparer.Ordinal);
                foreach (var p in ordered) {
                    if (IsPrefixNode(p)) continue; // per-prefix nodes replaced by the shared entry
                    string nn = Node(p);
                    structs.Append(
                        $"    public readonly struct {nn} {{\n        private readonly {T} v; private readonly double f;\n        internal {nn}({T} v, double f) {{ this.v = v; this.f = f; }}\n");
                    if (children.TryGetValue(p, out var set))
                        foreach (var w in set)
                            if (!PrefixExponentsWithoutNone.ContainsKey(w)) // skip SI-prefix children (handled below)
                                structs.Append(
                                    $"        public {Node(p.Length == 0 ? w : p + "_" + w)} {w} => new(v, f);\n");
                    if (p.Length == 0 && prefixable.Count > 0) // full SI prefix range on the modifier entry
                        foreach (var px in PrefixExponentsWithoutNone.Keys)
                            structs.Append(
                                $"        public {preName} {px} => new(v, f * 1e{power * PrefixExponents[px]});\n");
                    if (p.Length > 0 && terminal.TryGetValue(p, out var u))
                        structs.Append(
                            $"        public static implicit operator double({nn} n) => n.v.To{u}() / n.f;\n");
                    structs.Append("    }\n");
                }

                if (prefixable.Count > 0) { // shared prefixed entry: base units, factor pre-scaled
                    structs.Append(
                        $"    public readonly struct {preName} {{\n        private readonly {T} v; private readonly double f;\n        internal {preName}({T} v, double f) {{ this.v = v; this.f = f; }}\n");
                    foreach (var w in prefixable)
                        structs.Append($"        public {Node(w)} {w} => new(v, f);\n");
                    structs.Append("    }\n");
                }
            }

        if (!any) return;
        string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
    public static partial class Units {{
{starts}    }}
{structs}}}
";
        ctx.AddSource("_SquareCubicRead.g.cs", SourceText.From(src, Encoding.UTF8));
    }

    // The numerator "root" words of a type's SIMPLE prefix-expandable SI units — single-word units
    // declaring ≥1 numerator prefix (Meters, Grams, Joules, Bars, …). The `.Of` entry offers the full
    // SI prefix range for these via a shared prefixed-entry struct that folds the scaling into the
    // value. Compound-expandable units (GramsPerSecond) are excluded here — they keep per-prefix trie
    // paths (see EnumerateTypedEntryUnits) so they merge correctly with declared prefixed compounds.
    private static HashSet<string> ExpandableRootWords(List<IUnitSpec> units, HashSet<string> bases) {
        var er = new HashSet<string>();
        if (units.Count == 0) return er;
        foreach (var si in units.OfType<SiUnitSpec>()) {
            var words = SplitWords(si.Name);
            if (words.Contains("Per")) continue; // compound → not collapsed here
            var declaredNum = si.PrefixesSplit;
            if (!Array.Exists(declaredNum, p => PrefixExponentsWithoutNone.ContainsKey(p))) continue;
            var path = new List<string>();
            foreach (var w in words) path.AddRange(DecomposeDenom(w, bases));
            if (path.Count > 0) er.Add(path[0]);
        }

        return er;
    }

    // The unit names the type-constrained `.Of` entry offers in its trie. SIMPLE prefix-expandable
    // units are yielded unprefixed (the full SI range is layered on via the shared prefixed entry —
    // see ExpandableRootWords). COMPOUND prefix-expandable units (GramsPerSecond) ARE expanded to the
    // full numerator range here so their per-prefix paths merge with declared prefixed compounds
    // (KilogramsPerHour); each such prefixed name records `type|name -> (base variant, ten-exponent)`
    // in `construct` for base-factory + scaling construction. Everything else is yielded as declared.
    private static List<string> EnumerateTypedEntryUnits(List<IUnitSpec> units, string typeName,
        Dictionary<string, (string Base, int Exp)> construct) {
        var names = new List<string>();
        if (units.Count == 0) return names;
        foreach (var unit in units) {
            if (unit is not SiUnitSpec si) {
                names.Add(unit.Name);
                continue;
            }

            // si|baseName|exp|prefixes|perPrefixes
            string baseName = si.Name;
            string[] declaredNum = si.PrefixesSplit;
            string[] denPrefixes = si.PerPrefixesSplitOrNone;
            var words = SplitWords(baseName);
            int perIdx = words.IndexOf("Per");
            (int numRoot, int numPow) = FindRoot(words, 0, perIdx < 0 ? words.Count : perIdx);
            int denRoot = -1;
            if (perIdx >= 0) (denRoot, _) = FindRoot(words, perIdx + 1, words.Count);
            bool expandable = numRoot >= 0 &&
                              Array.Exists(declaredNum, p => PrefixExponentsWithoutNone.ContainsKey(p));
            bool compound = perIdx >= 0;
            // simple expandable → unprefixed (Pre adds the range); compound expandable → full range
            // here (merges with declared compounds); otherwise as declared.
            ICollection<string> numPrefixes =
                expandable ? (compound ? PrefixExponents.Keys : new[] { "None" }) : declaredNum;
            foreach (var np in numPrefixes) {
                if (np.Length == 0 || !PrefixExponents.TryGetValue(np, out int npExp)) continue;
                string[] dps = denRoot < 0 ? ["None"] : denPrefixes;
                foreach (var dp in dps) {
                    if (dp.Length == 0 || !PrefixExponents.ContainsKey(dp)) continue;
                    var w = new List<string>(words);
                    if (np != "None") w[numRoot] = np + LowerFirstChar(words[numRoot]);
                    if (denRoot >= 0 && dp != "None") w[denRoot] = dp + LowerFirstChar(words[denRoot]);
                    string full = string.Concat(w);
                    names.Add(full);
                    if (expandable && compound && np != "None") {
                        var nv = new List<string>(words); // None-numerator base variant (same denominator)
                        if (denRoot >= 0 && dp != "None") nv[denRoot] = dp + LowerFirstChar(words[denRoot]);
                        construct[typeName + "|" + full] = (string.Concat(nv), npExp * numPow);
                    }
                }
            }
        }

        return names;
    }

    // is limited to ways of constructing T (e.g. `Area.Of(4).Square.Milli.Meters`). Each unit name is
    // decomposed into fluent path segments (SplitWords + SI-prefix splitting: `SquareMillimeters` →
    // Square·Milli·Meters, `Kilometers` → Kilo·Meters, `MetersPerSecond` → Meters·Per·Second); a
    // completed path returns `T.From<Unit>(value)`. Purely reuses the existing From factories.
    private static void EmitTypedEntry(SourceProductionContext ctx,
        ImmutableArray<MeasurementInfo> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        // Start from the removable prefix aliases (Kilometers→Meters, Hectares→Ares, …); the typed
        // enumerator extends this with the extra all-prefix numerator variants it introduces.
        var construct =
            new Dictionary<string, (string Base, int Exp)>(
                BuildRemovable(all)); // + compound-expandable entries below
        var bases = new HashSet<string>();
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units))
                if (SplitWords(u).Count == 1) {
                    bases.Add(u);
                    if (u.Length > 1 && u.EndsWith("s"))
                        bases.Add(u.Substring(0,
                            u.Length - 1)); // singular, for denominators (Kilogram → Kilo.Gram)
                }

        var structs = new StringBuilder();
        var partials = new StringBuilder();
        foreach (var info in all.OrderBy(a => a.Name, StringComparer.Ordinal)) {
            string T = info.Name;
            // Trie of unprefixed unit paths (plus full-range paths for compound expandables). SIMPLE
            // prefix-expandable units get the whole SI range layered on via a shared prefixed-entry
            // struct that folds the scaling into the value and re-enters this trie — no per-prefix nodes.
            var children = new Dictionary<string, SortedDictionary<string, string>>();
            var terminal = new Dictionary<string, string>();
            var nodes = new HashSet<string> { "" };
            foreach (var u in EnumerateTypedEntryUnits(info.Units, T, construct)) {
                var path = new List<string>();
                foreach (var w in SplitWords(u)) path.AddRange(DecomposeDenom(w, bases));
                if (path.Count == 0) continue;
                string acc = "";
                foreach (var w in path) {
                    string child = acc.Length == 0 ? w : acc + "_" + w;
                    nodes.Add(child);
                    if (!children.TryGetValue(acc, out var m))
                        children[acc] = m = new SortedDictionary<string, string>(StringComparer.Ordinal);
                    m[w] = child;
                    acc = child;
                }

                if (!terminal.ContainsKey(acc)) terminal[acc] = u; // first unit wins on collision
            }

            var erWords = ExpandableRootWords(info.Units, bases);
            string NodeName(string p) => "Of_" + T + (p.Length == 0 ? "" : "_" + p);
            bool HasChildren(string p) => children.TryGetValue(p, out var m) && m.Count > 0;
            string qt = "global::" + ns + "." + T; // a child word may equal the type name, shadowing it
            string
                preName = "Of_" + T + "__Pre"; // shared prefixed-entry struct (double underscore avoids path clash)

            // The member stepping into root child `w` from a node carrying value `val` (structural child →
            // next node; terminal → construct T). SI prefixing reuses this by passing a scaled `val`.
            string Member(string w, string val) => HasChildren(children[""][w])
                ? $"        public {NodeName(children[""][w])} {w} => new({val});\n"
                : $"        public {qt} {w} => {Cons(qt, T, terminal[children[""][w]], val, construct)};\n";

            // Member for an arbitrary node's child (unscaled) — used for the normal trie transitions.
            string ChildMember(string w, string child) => HasChildren(child)
                ? $"        public {NodeName(child)} {w} => new(v);\n"
                : $"        public {qt} {w} => {Cons(qt, T, terminal[child], "v", construct)};\n";

            // Route each SI prefix: onto the root as a `.Prefix` property re-entering the shared Pre
            // entry (value scaled), unless it collides with a declared prefix-leading unit's node (e.g.,
            // Kilocalories' `.Kilo`), in which case the scaled expandable roots are merged into that node.
            var rootPrefixProps = new StringBuilder();
            var injected = new Dictionary<string, StringBuilder>();
            bool usePre = false;
            if (erWords.Count > 0)
                foreach (var px in PrefixExponentsWithoutNone.Keys) {
                    int exp = PrefixExponents[px];
                    if (children.TryGetValue("", out var rc) && rc.ContainsKey(px)) {
                        if (!injected.TryGetValue(px, out var inj)) injected[px] = inj = new StringBuilder();
                        foreach (var w in erWords.OrderBy(s => s, StringComparer.Ordinal))
                            if (!(children.TryGetValue(px, out var nc) &&
                                  nc.ContainsKey(w))) // don't shadow an existing child
                                inj.Append(Member(w, $"v * 1e{exp}"));
                    } else {
                        rootPrefixProps.Append($"        public {preName} {px} => new(v * 1e{exp});\n");
                        usePre = true;
                    }
                }

            partials.Append(
                $"    public readonly partial struct {T} {{ public static {NodeName("")} Of(double value) => new(value); }}\n");
            // Square/Cubic subentries: collapse the declared per-prefix areal/cubic nodes
            // (Of_T_Square_Milli → …SquareMillimeters) into a shared prefixed entry offering the FULL SI
            // range, folding the scaling into the value as an exact power of ten (square ⇒ 2·e, cubic ⇒
            // 3·e) — mirroring the untyped Square/Cubic and read builders.
            var scPre = new Dictionary<string, (List<string> Bases, int Power)>();
            var scSkip = new HashSet<string>();
            foreach (var mp in new[] { "Square", "Cubic" }) {
                if (!children.TryGetValue(mp, out var kids)) continue;
                int power = mp == "Square" ? 2 : 3;
                var bs = new List<string>();
                foreach (var w in kids.Keys)
                    if (PrefixExponentsWithoutNone.ContainsKey(w)) {
                        string pnode = mp + "_" + w;
                        if (children.TryGetValue(pnode, out var gk))
                            foreach (var b in gk.Keys)
                                if (kids.ContainsKey(b) && !bs.Contains(b))
                                    bs.Add(b); // base also a direct terminal child
                        foreach (var np in nodes)
                            if (np == pnode || np.StartsWith(pnode + "_"))
                                scSkip.Add(np);
                    }

                if (bs.Count > 0) {
                    bs.Sort(StringComparer.Ordinal);
                    scPre[mp] = (bs, power);
                }
            }

            string ScPreName(string p) => NodeName(p) + "__Pre";
            var ordered = new List<string>(nodes);
            ordered.Sort(StringComparer.Ordinal);
            foreach (var p in ordered) {
                if (!HasChildren(p) || scSkip.Contains(p))
                    continue; // leaf terminals are parent properties; skip collapsed prefix nodes
                string nn = NodeName(p);
                structs.Append($"    public readonly struct {nn} {{\n");
                structs.Append($"        private readonly double v;\n");
                structs.Append($"        internal {nn}(double v) => this.v = v;\n");
                bool scHere = scPre.ContainsKey(p);
                foreach (var kv in children[p])
                    if (!(scHere && PrefixExponentsWithoutNone.ContainsKey(kv.Key))) // skip collapsed SI-prefix children
                        structs.Append(ChildMember(kv.Key, kv.Value));
                if (scHere) // full SI range on the Square/Cubic node
                    foreach (var px in PrefixExponentsWithoutNone.Keys)
                        structs.Append(
                            $"        public {ScPreName(p)} {px} => new(v * 1e{scPre[p].Power * PrefixExponents[px]});\n");
                if (injected.TryGetValue(p, out var inj)) structs.Append(inj); // merged SI-prefix expandable roots
                if (p.Length == 0) structs.Append(rootPrefixProps); // non-colliding SI-prefix properties
                if (terminal.TryGetValue(p, out var tu)) // node both internal and a unit
                    structs.Append(
                        $"        public static implicit operator {qt}({nn} n) => {Cons(qt, T, tu, "n.v", construct)};\n");
                structs.Append("    }\n");
            }

            foreach (var kv in scPre) { // shared Square/Cubic prefixed entries (value pre-scaled)
                structs.Append($"    public readonly struct {ScPreName(kv.Key)} {{\n");
                structs.Append($"        private readonly double v;\n");
                structs.Append($"        internal {ScPreName(kv.Key)}(double v) => this.v = v;\n");
                foreach (var b in kv.Value.Bases) structs.Append(ChildMember(b, children[kv.Key][b]));
                structs.Append("    }\n");
            }

            if (usePre) { // shared prefixed entry: the prefixable roots (v pre-scaled)
                structs.Append($"    public readonly struct {preName} {{\n");
                structs.Append($"        private readonly double v;\n");
                structs.Append($"        internal {preName}(double v) => this.v = v;\n");
                foreach (var w in erWords.OrderBy(s => s, StringComparer.Ordinal))
                    structs.Append(Member(w, "v"));
                structs.Append("    }\n");
            }
        }

        string src = $@"// <auto-generated/>
#nullable disable

namespace {ns} {{
{partials}{structs}}}
";
        ctx.AddSource("_Of.g.cs", SourceText.From(src, Encoding.UTF8));
    }

    // Splits a denominator unit word into [SiPrefix, Base] when it starts with an SI prefix whose
    // unprefixed base also occurs as a denominator word for the same numerator type (e.g., Kilogram →
    // [Kilo, Gram], Centimeter → [Centi, Meter], Milliliter → [Milli, Liter]); otherwise returns [w].
    private static List<string> DecomposeDenom(string w, HashSet<string> rawDenom) {
        string bestPrefix = "", bestBase = "";
        foreach (var kv in PrefixExponentsWithoutNone) {
            if (w.Length <= kv.Key.Length ||
                !w.StartsWith(kv.Key, StringComparison.Ordinal)) continue;
            string rest = w.Substring(kv.Key.Length);
            string baseW = char.ToUpperInvariant(rest[0]) + rest.Substring(1);
            if (rawDenom.Contains(baseW) && (bestPrefix.Length == 0 || kv.Key.Length > bestPrefix.Length)) {
                bestPrefix = kv.Key;
                bestBase = baseW;
            }
        }

        return bestPrefix.Length != 0 ? [bestPrefix, bestBase] : [w];
    }

    private static string Emit(string name, string ns, string symbol, double displayFactor, string variableName,
        List<IUnitSpec> units) {
        string dfStr = displayFactor.ToString("R", CultureInfo.InvariantCulture);
        string disp = displayFactor == 1d
            ? variableName
            : $"({variableName} / {dfStr})";
        string toStr = symbol.Length == 0 ? $"$\"{{{disp}}}\"" : $"$\"{{{disp}}} {symbol}\"";
        string toStrFmt = symbol.Length == 0
            ? $"({disp}).ToString(format, provider)"
            : $"({disp}).ToString(format, provider) + \" {symbol}\"";
        string unitMethods = EmitUnits(name, variableName, units);

        return $@"// <auto-generated/>
#nullable disable
using System;
using System.Numerics;

namespace {ns};

public readonly partial struct {name}
    : IMeasurement<{name}>, IComparable<{name}>, IComparable, IEquatable<{name}>, IFormattable, IParsable<{name}>, ISpanParsable<{name}> {{
    private readonly double {variableName};
    private {name}(double {variableName}) => this.{variableName} = {variableName};

    public static {name} FromCanonical(double {variableName}) => new {name}({variableName});
    public double CanonicalValue => {variableName};
    public string UnitSymbol => ""{symbol}"";
    public static {name} Zero => new {name}(0);

{unitMethods}
    public override string ToString() => {toStr};
    public string ToString(string format, IFormatProvider provider) => {toStrFmt};

    public static {name} Parse(string s, IFormatProvider provider) {{
        if (!MeasurementParsing.TryParseCanonical(s, ""{symbol}"", {dfStr}, provider, out double c))
            throw new FormatException($""Could not parse as {name}: {{s}}"");
        return new {name}(c);
    }}
    public static bool TryParse(string s, IFormatProvider provider, out {name} result) {{
        if (MeasurementParsing.TryParseCanonical(s, ""{symbol}"", {dfStr}, provider, out double c)) {{ result = new {name}(c); return true; }}
        result = default; return false;
    }}
    public static {name} Parse(ReadOnlySpan<char> s, IFormatProvider provider) {{
        if (!MeasurementParsing.TryParseCanonical(s, ""{symbol}"", {dfStr}, provider, out double c))
            throw new FormatException($""Could not parse as {name}: {{s.ToString()}}"");
        return new {name}(c);
    }}
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out {name} result) {{
        if (MeasurementParsing.TryParseCanonical(s, ""{symbol}"", {dfStr}, provider, out double c)) {{ result = new {name}(c); return true; }}
        result = default; return false;
    }}
    public static {name} Parse(string s) => Parse(s, null);
    public static bool TryParse(string s, out {name} result) => TryParse(s, null, out result);

    public override bool Equals(object obj) => obj is {name} o && o.{variableName} == {variableName};
    public bool Equals({name} other) => other.{variableName} == {variableName};
    public override int GetHashCode() => {variableName}.GetHashCode();

    public bool NearlyEquals({name} other, int ulps = 4) => MeasurementMath.NearlyEqual({variableName}, other.{variableName}, ulps);

    public int CompareTo({name} other) => {variableName}.CompareTo(other.{variableName});
    int IComparable.CompareTo(object obj)
        => obj is null ? 1
         : obj is {name} o ? {variableName}.CompareTo(o.{variableName})
         : throw new ArgumentException(""Object must be of type {name}."", nameof(obj));

    public static {name} operator +({name} a, {name} b) => new {name}(a.{variableName} + b.{variableName});
    public static {name} operator -({name} a, {name} b) => new {name}(a.{variableName} - b.{variableName});
    public static {name} operator -({name} a) => new {name}(-a.{variableName});
    public static {name} operator *({name} a, double factor) => new {name}(a.{variableName} * factor);
    public static {name} operator *(double factor, {name} a) => new {name}(factor * a.{variableName});
    public static {name} operator /({name} a, double divisor) => new {name}(a.{variableName} / divisor);
    public static double operator /({name} a, {name} b) => a.{variableName} / b.{variableName};
    public static bool operator <({name} a, {name} b) => a.{variableName} < b.{variableName};
    public static bool operator >({name} a, {name} b) => a.{variableName} > b.{variableName};
    public static bool operator <=({name} a, {name} b) => a.{variableName} <= b.{variableName};
    public static bool operator >=({name} a, {name} b) => a.{variableName} >= b.{variableName};
    public static bool operator ==({name} a, {name} b) => a.{variableName} == b.{variableName};
    public static bool operator !=({name} a, {name} b) => a.{variableName} != b.{variableName};

    public {name} Abs() => new {name}(Math.Abs({variableName}));
    public {name} Min({name} other) => {variableName} <= other.{variableName} ? this : other;
    public {name} Max({name} other) => {variableName} >= other.{variableName} ? this : other;
    public {name} Clamp({name} min, {name} max) => new {name}(Math.Clamp({variableName}, min.{variableName}, max.{variableName}));
    public static {name} Lerp({name} a, {name} b, double t) => new {name}(a.{variableName} + (b.{variableName} - a.{variableName}) * t);
}}
";
    }
}
