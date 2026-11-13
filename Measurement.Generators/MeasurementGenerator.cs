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
public sealed partial class MeasurementGenerator : IIncrementalGenerator {
    private const string DefaultNamespace = "com.hafthor.Measurement";
    private const string AttrMetadataName = $"{DefaultNamespace}.MeasurementAttribute";

    private static readonly string[] None = ["None"];

    private static string ResolveNamespace(IEnumerable<MeasurementInfo> all) =>
        all.Select(m => m.Namespace).FirstOrDefault() ?? DefaultNamespace;

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
                    .Select(FromAttribute)
                    .Where(spec => spec is not null)
                    .ToList();

                // [Product<TLeft, TRight>] relations: Name = Left × Right (Primary marks the result a
                // selector implicitly converts to when the product is shared by several types).
                List<ProductSpec> products = type.GetAttributes()
                    .Select(ProductSpec.FromAttribute)
                    .Where(spec => spec.HasValue)
                    .Select(spec => spec.Value)
                    .ToList();

                // [Reciprocal<TPartner>(Name=…)]: this = 1 / TPartner in coherent SI, encoded as
                // "partnerType|readName".
                List<ReciprocalSpec> reciprocals = type.GetAttributes()
                    .Select(ReciprocalSpec.FromAttribute)
                    .Where(spec => spec.HasValue)
                    .Select(spec => spec.Value)
                    .ToList();

                return new MeasurementInfo(type.Name, type.ContainingNamespace.ToDisplayString(), symbol,
                    displayFactor, variableName, unitSpecs, products, reciprocals);
            });

        // Struct body: identical-per-type boilerplate plus the unit From/To methods.
        context.RegisterSourceOutput(types, EmitBase);

        var collected = types.Collect();
        Action<SourceProductionContext, ImmutableArray<MeasurementInfo>>[] emitters = [
            EmitFluentSet, EmitProducts, EmitPerBuilders, EmitPerReadBuilders, EmitTokenAlgebra,
            EmitSquareCubic, EmitSquareCubicRead, EmitTypedEntry, EmitReciprocals
        ];
        foreach (var emitter in emitters)
            context.RegisterSourceOutput(collected, emitter);
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
    private static List<string> EnumerateUnitNames(List<IUnitSpec> units) =>
        ExpandUnits(units, hookMode: false).Select(kv => kv.Key).ToList();

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

    public static string Depluralize(string s) => s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s;

    // Square/Cubic denominator sub-trie shared by the input and read-out `.Per` walks. For each of
    // Area (Square) / Volume (Cubic), maps a singularized, prefix-decomposed length path to the areal/
    // cubic unit name (so `.Cubic.Centi.Meter` resolves to CubicCentimeters' exact coherent value),
    // with the child-word sets along the way.
    private static Dictionary<string, DenomInfo> BuildDenomModTrie(ImmutableArray<MeasurementInfo> all,
        Dictionary<string, (double DF, int G)> meta) {
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
                        lw.Add(Depluralize(d));

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

    private readonly record struct PerType(string Name, string Ns, List<string> Hooks);

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
                string sn = Depluralize(n);
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
                path.Add(Depluralize(baseUnit));
                opts.Add(new Opt(path, baseUnit));
            }

            unitOpts[info.Name] = opts;
        }

        return (meta, typeOfUnit, unitWordType, unitOpts);
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
}
