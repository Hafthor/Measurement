using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Measurement.Generators;

// Emits the identical-across-every-measurement boilerplate (operators, equality, comparison,
// formatting, math utilities, and the IMeasurement<T> / System.Numerics surface) into each
// [Measurement]-annotated readonly partial struct, so the hand-written source only needs the
// unit-specific From/To methods and cross-type operators.
[Generator]
public sealed class MeasurementGenerator : IIncrementalGenerator {
    private const string AttrMetadataName = "com.hafthor.Measurement.MeasurementAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var types = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttrMetadataName,
            predicate: static (node, _) => node is StructDeclarationSyntax,
            transform: static (ctx, _) => {
                var type = (INamedTypeSymbol)ctx.TargetSymbol;
                var symbol = ctx.Attributes[0].ConstructorArguments.Length > 0
                    ? ctx.Attributes[0].ConstructorArguments[0].Value as string ?? ""
                    : "";
                double displayFactor = 1;
                string variableName = "value";
                foreach (var na in ctx.Attributes[0].NamedArguments)
                    if (na.Key == "DisplayFactor" && na.Value.Value is double d)
                        displayFactor = d;
                    else if (na.Key == "VariableName" && na.Value.Value is string s)
                        variableName = s;
                // Collect [SiUnit] and [Unit] declarations, tagged and encoded as an equatable
                // string so the incremental pipeline caches correctly.
                var unitSpecs = new List<string>();
                foreach (var a in type.GetAttributes()) {
                    if (a.AttributeClass?.Name == "SiUnitAttribute") {
                        var args = a.ConstructorArguments;
                        string baseName = args.Length > 0 ? args[0].Value as string ?? "" : "";
                        int tenExp = args.Length > 1 && args[1].Value is int e ? e : 0;
                        string prefixes = args.Length > 2 ? args[2].Value as string ?? "None" : "None";
                        string perPrefixes = "None";
                        foreach (var na in a.NamedArguments)
                            if (na.Key == "PerPrefixes" && na.Value.Value is string pp) perPrefixes = pp;
                        unitSpecs.Add($"si|{baseName}|{tenExp}|{prefixes}|{perPrefixes}");
                    } else if (a.AttributeClass?.Name == "UnitAttribute") {
                        var args = a.ConstructorArguments;
                        string unitName = args.Length > 0 ? args[0].Value as string ?? "" : "";
                        double factor = args.Length > 1 && args[1].Value is double f ? f : 1;
                        double offset = 0;
                        foreach (var na in a.NamedArguments)
                            if (na.Key == "Offset" && na.Value.Value is double o) offset = o;
                        var inv = System.Globalization.CultureInfo.InvariantCulture;
                        unitSpecs.Add($"u|{unitName}|{factor.ToString("R", inv)}|{offset.ToString("R", inv)}");
                    } else if (a.AttributeClass?.Name == "UnitHookAttribute") {
                        var args = a.ConstructorArguments;
                        string hookName = args.Length > 0 ? args[0].Value as string ?? "" : "";
                        unitSpecs.Add($"h|{hookName}");
                    }
                }
                // [Product<TLeft, TRight>] relations: Name = Left × Right (Primary marks the result a
                // selector implicitly converts to when the product is shared by several types).
                var products = new List<string>();
                foreach (var a in type.GetAttributes())
                    if (a.AttributeClass?.Name == "ProductAttribute" && a.AttributeClass.TypeArguments.Length == 2
                        && a.AttributeClass.TypeArguments[0] is INamedTypeSymbol l
                        && a.AttributeClass.TypeArguments[1] is INamedTypeSymbol r) {
                        bool primary = false;
                        foreach (var na in a.NamedArguments)
                            if (na.Key == "Primary" && na.Value.Value is bool p) primary = p;
                        products.Add($"{l.Name},{r.Name},{(primary ? 1 : 0)}");
                    }
                return (Name: type.Name, Namespace: type.ContainingNamespace.ToDisplayString(), Symbol: symbol, DisplayFactor: displayFactor, VariableName: variableName, Units: string.Join(";", unitSpecs), Products: string.Join(";", products));
            });

        // Struct body: identical-per-type boilerplate plus the unit From/To methods.
        context.RegisterSourceOutput(types, static (ctx, info) =>
            ctx.AddSource(info.Name + ".g.cs", SourceText.From(
                Emit(info.Name, info.Namespace, info.Symbol, info.DisplayFactor, info.VariableName, info.Units), Encoding.UTF8)));

        // Fluent sugar: needs every type at once so input-hook name collisions across
        // dimensionally-equivalent types can be detected and dropped.
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
    }

    // Units excluded from the `.Per` walk because their result is dimensionally inconsistent with a
    // literal numerator÷denominator reading — e.g. psi's "Pounds" is pound-force, not mass, so
    // Mass.Per.Square.Inch would wrongly yield Pressure. These stay reachable via the flat hook only.
    private static readonly HashSet<string> PerExcludeUnits = new() { "PoundsPerSquareInch" };

    // SI prefix name → power-of-ten exponent (used to expand SiUnit families).
    private static readonly System.Collections.Generic.Dictionary<string, int> PrefixExponents = new() {
        ["Quetta"] = 30, ["Ronna"] = 27, ["Yotta"] = 24, ["Zetta"] = 21, ["Exa"] = 18,
        ["Peta"] = 15, ["Tera"] = 12, ["Giga"] = 9, ["Mega"] = 6, ["Kilo"] = 3,
        ["Hecto"] = 2, ["Deca"] = 1, ["None"] = 0, ["Deci"] = -1, ["Centi"] = -2,
        ["Milli"] = -3, ["Micro"] = -6, ["Nano"] = -9, ["Pico"] = -12, ["Femto"] = -15,
        ["Atto"] = -18, ["Zepto"] = -21, ["Yocto"] = -24, ["Ronto"] = -27, ["Quecto"] = -30,
    };

    // Expands the encoded SiUnit families into From/To method source. The base unit's factor to
    // the anchor is 10^tenExponent; a numerator prefix shifts the exponent by prefix×power, a
    // denominator (Per) prefix by −prefix×power, so factors stay exact powers of ten (identity at 0).
    private static string EmitUnits(string name, string variableName, string units) {
        if (units.Length == 0) return "";
        var sb = new StringBuilder();
        foreach (var spec in units.Split(';')) {
            var parts = spec.Split('|');
            if (parts[0] == "h") continue;
            if (parts[0] == "u") { EmitExplicitUnit(sb, name, variableName, parts[1], parts[2], parts[3]); continue; }
            // si|baseName|exp|prefixes|perPrefixes
            string baseName = parts[1];
            int baseExp = int.Parse(parts[2]);
            string[] numPrefixes = parts[3].Split(' ');
            string[] denPrefixes = parts.Length > 4 ? parts[4].Split(' ') : new[] { "None" };

            var words = SplitWords(baseName);
            int perIdx = words.IndexOf("Per");
            FindRoot(words, 0, perIdx < 0 ? words.Count : perIdx, out int numRoot, out int numPow);
            int denRoot = -1, denPow = 1;
            if (perIdx >= 0) FindRoot(words, perIdx + 1, words.Count, out denRoot, out denPow);

            foreach (var np in numPrefixes) {
                if (np.Length == 0 || !PrefixExponents.TryGetValue(np, out int npExp)) continue;
                string[] dps = denRoot < 0 ? new[] { "None" } : denPrefixes;
                foreach (var dp in dps) {
                    if (dp.Length == 0 || !PrefixExponents.TryGetValue(dp, out int dpExp)) continue;
                    int exp = baseExp + npExp * numPow - (denRoot < 0 ? 0 : dpExp * denPow);
                    var w = new List<string>(words);
                    if (np != "None") w[numRoot] = np + Lower(words[numRoot]);
                    if (denRoot >= 0 && dp != "None") w[denRoot] = dp + Lower(words[denRoot]);
                    string full = string.Concat(w);
                    string param = Lower(full);
                    if (exp == 0) {
                        sb.Append($"    public static {name} From{full}(double {param}) => new {name}({param});\n");
                        sb.Append($"    public double To{full}() => {variableName};\n");
                    } else {
                        string lit = "1e" + exp;
                        sb.Append($"    public static {name} From{full}(double {param}) => new {name}({param} * {lit});\n");
                        sb.Append($"    public double To{full}() => {variableName} / {lit};\n");
                    }
                }
            }
        }
        return sb.ToString();
    }

    // Enumerates every From/To unit name the generator emits for a type (SI-prefixed family members,
    // Per-prefix cross products, and explicit [Unit] names), preserving declaration order. Used to
    // map a compound unit's numerator/denominator words back to concrete types and To-methods.
    private static List<string> EnumerateUnitNames(string units) {
        var names = new List<string>();
        if (units.Length == 0) return names;
        foreach (var spec in units.Split(';')) {
            var parts = spec.Split('|');
            if (parts[0] == "h") { names.Add(parts[1]); continue; }
            if (parts[0] == "u") { names.Add(parts[1]); continue; }
            string baseName = parts[1];
            string[] numPrefixes = parts[3].Split(' ');
            string[] denPrefixes = parts.Length > 4 ? parts[4].Split(' ') : new[] { "None" };
            var words = SplitWords(baseName);
            int perIdx = words.IndexOf("Per");
            FindRoot(words, 0, perIdx < 0 ? words.Count : perIdx, out int numRoot, out _);
            int denRoot = -1;
            if (perIdx >= 0) FindRoot(words, perIdx + 1, words.Count, out denRoot, out _);
            foreach (var np in numPrefixes) {
                if (np.Length == 0 || !PrefixExponents.ContainsKey(np)) continue;
                string[] dps = denRoot < 0 ? new[] { "None" } : denPrefixes;
                foreach (var dp in dps) {
                    if (dp.Length == 0 || !PrefixExponents.ContainsKey(dp)) continue;
                    var w = new List<string>(words);
                    if (np != "None") w[numRoot] = np + Lower(words[numRoot]);
                    if (denRoot >= 0 && dp != "None") w[denRoot] = dp + Lower(words[denRoot]);
                    names.Add(string.Concat(w));
                }
            }
        }
        return names;
    }
    // directly-hookable unit (each [Unit] and [UnitHook], plus the un-prefixed base of each
    // [SiUnit] family — SI-prefixed variants are reached through the prefix chain instead).
    private static List<KeyValuePair<string, double>> ComputeHookCandidates(string units) {
        var result = new List<KeyValuePair<string, double>>();
        if (units.Length == 0) return result;
        void Add(string n, double v) => result.Add(new KeyValuePair<string, double>(n, v));
        foreach (var spec in units.Split(';')) {
            var parts = spec.Split('|');
            if (parts[0] == "h") { Add(parts[1], double.NaN); continue; }
            if (parts[0] == "u") {
                double.TryParse(parts[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double uf);
                Add(parts[1], uf);
                continue;
            }
            // si: only the base (all-None prefix) variant is a direct hook.
            string baseName = parts[1];
            int baseExp = int.Parse(parts[2]);
            string[] denPrefixes = parts.Length > 4 ? parts[4].Split(' ') : new[] { "None" };
            var words = SplitWords(baseName);
            int perIdx = words.IndexOf("Per");
            FindRoot(words, 0, perIdx < 0 ? words.Count : perIdx, out _, out int numPow);
            int denRoot = -1, denPow = 1;
            if (perIdx >= 0) FindRoot(words, perIdx + 1, words.Count, out denRoot, out denPow);
            string[] dps = denRoot < 0 ? new[] { "None" } : denPrefixes;
            foreach (var dp in dps) {
                if (dp.Length == 0 || !PrefixExponents.TryGetValue(dp, out int dpExp)) continue;
                int exp = baseExp - (denRoot < 0 ? 0 : dpExp * denPow);
                var w = new List<string>(words);
                if (denRoot >= 0 && dp != "None") w[denRoot] = dp + Lower(words[denRoot]);
                Add(string.Concat(w), System.Math.Pow(10, exp));
            }
        }
        return result;
    }

    // True if unitName == prefix + lowerFirst(other) for some SI prefix and another hook candidate
    // on the same struct, with a matching power-of-ten factor — i.e. a prefixed alias reachable via
    // the prefix chain (e.g. Kilomoles == Kilo + Moles), which is dropped from the fluent surface.
    private static bool IsPrefixedAlias(string unitName, double factor, Dictionary<string, double> lookup) {
        if (double.IsNaN(factor)) return false;
        foreach (var kv in PrefixExponents) {
            if (kv.Value == 0 || !unitName.StartsWith(kv.Key)) continue;
            string rest = unitName.Substring(kv.Key.Length);
            if (rest.Length == 0) continue;
            string base_ = char.ToUpperInvariant(rest[0]) + rest.Substring(1);
            if (base_ == unitName || !lookup.TryGetValue(base_, out double bf) || double.IsNaN(bf)) continue;
            double expected = bf * System.Math.Pow(10, kv.Value);
            double scale = System.Math.Max(System.Math.Abs(factor), System.Math.Abs(expected));
            if (scale == 0 || System.Math.Abs(factor - expected) <= 1e-6 * scale) return true;
        }
        return false;
    }

    // Emits a single non-metric unit. With no offsets it's a plain factor; otherwise an affine
    // scale where anchor = (value + offset) * factor.
    private static void EmitExplicitUnit(StringBuilder sb, string name, string variableName, string unitName, string factorLit, string offsetLit) {
        string param = Lower(unitName);
        bool affine = offsetLit != "0";
        if (!affine) {
            sb.Append($"    public static {name} From{unitName}(double {param}) => new {name}({param} * {factorLit});\n");
            sb.Append($"    public double To{unitName}() => {variableName} / {factorLit};\n");
        } else {
            string inner = $"({param} + ({offsetLit}))";
            sb.Append($"    public static {name} From{unitName}(double {param}) => new {name}({inner} * ({factorLit}));\n");
            string body = $"({variableName}) / ({factorLit})";
            if (offsetLit != "0") body += $" - ({offsetLit})";
            sb.Append($"    public double To{unitName}() => {body};\n");
        }
    }

    // Finds the first root word in words[start..end) and its power (Square ⇒ 2, Cubic ⇒ 3, else 1).
    private static void FindRoot(List<string> words, int start, int end, out int rootIndex, out int power) {
        rootIndex = -1; power = 1;
        int pending = 1;
        for (int i = start; i < end; i++) {
            if (words[i] == "Square") pending = 2;
            else if (words[i] == "Cubic") pending = 3;
            else if (words[i] is "Squared" or "Cubed" or "Per") continue;
            else { rootIndex = i; power = pending; return; }
        }
    }

    private static string Lower(string s) => char.ToLowerInvariant(s[0]) + s.Substring(1);

    private static List<string> SplitWords(string camel) {
        var words = new List<string>();
        var sb = new StringBuilder();
        foreach (char c in camel) {
            if (char.IsUpper(c) && sb.Length > 0) { words.Add(sb.ToString()); sb.Clear(); }
            sb.Append(c);
        }
        if (sb.Length > 0) words.Add(sb.ToString());
        return words;
    }

    // Emits the fluent-sugar wiring for every struct. Runs once over all types so that input-hook
    // names shared by two dimensionally-equivalent types (e.g. JouleSeconds on Action and
    // AngularMomentum) can be detected. Such names would collide on the shared Prefixed/double
    // receiver, so instead of a direct hook they resolve to a small selector struct that names each
    // measurement: 1.0.JouleSeconds.Action / .AngularMomentum. Their read-out hooks stay direct
    // (distinct Reader<T>). Prefixed aliases (Kilomoles == Kilo + Moles) are dropped from both sides —
    // reach them via the prefix chain (Measure.Of(x).Kilo.Moles). Other SI-prefixed variants likewise
    // go through the chain; only un-prefixed bases and non-metric units get a direct hook.
    private static void EmitFluentSet(SourceProductionContext ctx, System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
        // Per struct: ordered hook candidates minus prefixed aliases.
        var perType = new List<(string Name, string Ns, List<string> Hooks)>();
        var inputCounts = new Dictionary<string, int>();
        foreach (var info in all) {
            var candidates = ComputeHookCandidates(info.Units);
            var lookup = new Dictionary<string, double>();
            foreach (var c in candidates) lookup[c.Key] = c.Value;
            var hooks = new List<string>();
            foreach (var c in candidates) {
                if (IsPrefixedAlias(c.Key, c.Value, lookup)) continue;
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
    // Product-spellings (e.g. JouleSeconds = Joule·Seconds) are skipped: the compositional token walk
    // (`.Joule.Seconds.AngularMomentum`) already disambiguates them, so the fused hook is redundant.
    private static void EmitCollisionSelectors(SourceProductionContext ctx, List<(string Name, string Ns, List<string> Hooks)> perType, Dictionary<string, int> inputCounts,
        System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
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
            foreach (var w in ws) if (!productWords.Contains(w)) return false;
            return true;
        }
        // Colliding name → owning measurements, in stable declaration order.
        var owners = new List<KeyValuePair<string, List<string>>>();
        var seen = new HashSet<string>();
        foreach (var (name, _, hooks) in perType)
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
        int total = 0, sign = 1, depth = 0;
        var parenSign = new Stack<int>();
        for (int i = 0; i < symbol.Length; i++) {
            char c = symbol[i];
            if (c == '(') { parenSign.Push(sign); depth++; }
            else if (c == ')') { depth--; if (parenSign.Count > 0) sign = parenSign.Pop(); }
            else if (c == '/') sign = -sign;                 // everything after '/' at this level is a denominator
            else if (c == '·') { /* keep sign */ }
            else if (c == 'g' && (i == 0 || !char.IsLetter(symbol[i - 1])) && (i + 1 >= symbol.Length || !char.IsLetter(symbol[i + 1]))) {
                int exp = 1;
                if (i + 1 < symbol.Length) {
                    if (symbol[i + 1] == '²') exp = 2; else if (symbol[i + 1] == '³') exp = 3;
                }
                total += sign * exp;
            }
        }
        return total;
    }

    // Splits a DisplayFactor into a power-of-ten exponent and a non-power-of-ten residual
    // (e.g. 1e6 → (6, 1), 9 → (0, 9)), so operator factors keep their ten-scaling exact in a single
    // 1e{n} and carry any residual (like Temperature's 9) as an explicit divisor.
    private static void SplitTen(double v, out int tenExp, out double residual) {
        tenExp = 0; residual = v;
        if (v <= 0) return;
        int e = (int)System.Math.Round(System.Math.Log10(v));
        if (System.Math.Pow(10, e) == v) { tenExp = e; residual = 1; }
    }

    // Coherent-SI divisor for a type = DisplayFactor × 10^(3·gramPower), split into a power-of-ten
    // exponent and a non-ten residual — dividing a stored value by it yields coherent SI (kg·m·s·…).
    private static (int Ten, double Res) CoherentDivisor(double df, int gramPower) {
        SplitTen(df, out int te, out double re);
        return (te + 3 * gramPower, re);
    }
    // Divides/multiplies a value expression by a coherent-SI divisor, keeping powers of ten exact.
    private static string ToCoherentExpr(string val, (int Ten, double Res) d) {
        var s = new StringBuilder(val);
        if (d.Ten > 0) s.Append($" / 1e{d.Ten}");
        else if (d.Ten < 0) s.Append($" * 1e{-d.Ten}");
        if (d.Res != 1) s.Append($" / {d.Res.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}");
        return d.Ten != 0 || d.Res != 1 ? $"({s})" : val;
    }
    private static string FromCoherentExpr(string expr, (int Ten, double Res) d) {
        var s = new StringBuilder(expr);
        if (d.Ten > 0) s.Append($" * 1e{d.Ten}");
        else if (d.Ten < 0) s.Append($" / 1e{-d.Ten}");
        if (d.Res != 1) s.Append($" * {d.Res.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}");
        return s.ToString();
    }

    // Emits every cross-type operator implied by the [Product] relations. For C = A × B it writes
    // A*B, B*A → C and C/A → B, C/B → A, each with an exact factor derived from the symbols'
    // gram-power and DisplayFactor, placed in a partial of one of its parameter types.
    private static void EmitProducts(SourceProductionContext ctx, System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
        var meta = new Dictionary<string, (double DF, int G)>();
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        foreach (var info in all) meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));

        // container type → operator source lines.
        var byContainer = new Dictionary<string, StringBuilder>();
        StringBuilder Body(string container) {
            if (!byContainer.TryGetValue(container, out var sb)) byContainer[container] = sb = new StringBuilder();
            return sb;
        }
        // Each type's coherent-SI divisor = DisplayFactor × 10^(3·gramPower): dividing a stored
        // value by it yields the value in coherent SI units (kg·m·s·…). Split into a power-of-ten
        // exponent and a non-ten residual (e.g. Temperature's 9) so the division stays exact.
        (int Ten, double Res) Div(string t) {
            var m = meta[t];
            SplitTen(m.DF, out int te, out double re);
            return (te + 3 * m.G, re);
        }
        // Divides a value expression by a coherent-SI divisor (positive powers of ten are exact),
        // parenthesised so it binds to that operand inside a product.
        string ToCoherent(string val, (int Ten, double Res) d) {
            var s = new StringBuilder(val);
            if (d.Ten > 0) s.Append($" / 1e{d.Ten}");
            else if (d.Ten < 0) s.Append($" * 1e{-d.Ten}");
            if (d.Res != 1) s.Append($" / {d.Res.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}");
            return d.Ten != 0 || d.Res != 1 ? $"({s})" : val;
        }
        // Multiplies a value expression by a coherent-SI divisor (to convert a coherent result back
        // to the result type's stored value).
        string FromCoherent(string expr, (int Ten, double Res) d) {
            var s = new StringBuilder(expr);
            if (d.Ten > 0) s.Append($" * 1e{d.Ten}");
            else if (d.Ten < 0) s.Append($" / 1e{-d.Ten}");
            if (d.Res != 1) s.Append($" * {d.Res.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}");
            return s.ToString();
        }
        // Gather every operator "intent" keyed by its C# signature; a signature that resolves to more
        // than one result type (a dimensionally-ambiguous product like Force×Length → Energy or
        // Torque) is emitted as a selector instead of colliding.
        var intents = new Dictionary<string, (string Container, string Op, string P1, string P2, string Expr, List<(string Name, bool Primary)> Results)>();
        void Intent(string container, string op, string p1, string p2, string expr, string result, bool primary) {
            string sig = $"{container}|{op}|{p1}|{p2}";
            if (!intents.TryGetValue(sig, out var it)) intents[sig] = it = (container, op, p1, p2, expr, new List<(string, bool)>());
            if (!it.Results.Exists(x => x.Name == result)) it.Results.Add((result, primary));
        }

        foreach (var info in all) {
            if (info.Products.Length == 0) continue;
            string C = info.Name;
            if (!meta.ContainsKey(C)) continue;
            var dC = Div(C);
            foreach (var rel in info.Products.Split(';')) {
                var ab = rel.Split(',');
                if (ab.Length < 2) continue;
                string A = ab[0], B = ab[1];
                bool primary = ab.Length > 2 && ab[2] == "1";
                if (!meta.ContainsKey(A) || !meta.ContainsKey(B)) continue;
                var dA = Div(A); var dB = Div(B);
                // C = A × B and B × A — coherent product of the two operands.
                Intent(A, "*", A, B, $"{ToCoherent("a.CanonicalValue", dA)} * {ToCoherent("b.CanonicalValue", dB)}", C, primary);
                if (A != B) Intent(B, "*", B, A, $"{ToCoherent("a.CanonicalValue", dB)} * {ToCoherent("b.CanonicalValue", dA)}", C, primary);
                // A = C / B and B = C / A — coherent quotient (a is C, b is the divisor).
                Intent(C, "/", C, A, $"{ToCoherent("a.CanonicalValue", dC)} / {ToCoherent("b.CanonicalValue", dA)}", B, false);
                if (A != B) Intent(C, "/", C, B, $"{ToCoherent("a.CanonicalValue", dC)} / {ToCoherent("b.CanonicalValue", dB)}", A, false);
            }
        }

        // Emit each intent: a single result → a direct operator; multiple → a selector operator plus
        // a selector struct whose type-named properties pick the intended result (f×l).Energy / .Torque.
        var selectors = new Dictionary<string, List<(string Name, bool Primary)>>();
        string SelName(string op, string p1, string p2) {
            if (op == "*") { var a = new[] { p1, p2 }; System.Array.Sort(a, System.StringComparer.Ordinal); return a[0] + a[1] + "Product"; }
            return p1 + "Over" + p2 + "Quotient";
        }
        foreach (var it in intents.Values) {
            if (it.Results.Count == 1) {
                string r = it.Results[0].Name;
                Body(it.Container).Append($"    public static {r} operator {it.Op}({it.P1} a, {it.P2} b) => {r}.FromCanonical({FromCoherent(it.Expr, Div(r))});\n");
            } else {
                string sel = SelName(it.Op, it.P1, it.P2);
                if (!selectors.ContainsKey(sel)) selectors[sel] = it.Results;
                Body(it.Container).Append($"    public static {sel} operator {it.Op}({it.P1} a, {it.P2} b) => new({it.Expr});\n");
            }
        }
        if (byContainer.Count == 0) return;
        var src = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns + " {\n");
        foreach (var kv in selectors) {
            var results = kv.Value;
            string primary = results.Find(x => x.Primary).Name;   // null when none is marked
            var names = new List<string>();
            foreach (var r in results) names.Add(r.Name);
            src.Append(primary != null
                ? $"    // Ambiguous product — implicitly a {primary}; also (a * b).{string.Join(" / (a * b).", names)}\n"
                : $"    // Ambiguous product — pick the result: (a * b).{string.Join(" / (a * b).", names)}\n");
            src.Append($"    public readonly struct {kv.Key} {{\n");
            src.Append("        private readonly double v;\n");
            src.Append($"        internal {kv.Key}(double v) => this.v = v;\n");
            foreach (var r in names)
                src.Append($"        public {r} {r} => {r}.FromCanonical({FromCoherent("v", Div(r))});\n");
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

    // Shared model for the `.Per` walk (both input and read-out). Returns per-type metadata plus, for
    // every measurement type, the list of its non-affine single-word units expressed as a denominator
    // option = (prefix-decomposed, singularised word path; the un-prefixed base unit name). Also a map
    // from any denominator word (singular or plural, prefixed or not) to its owning type, so the type
    // sequence of a compound unit's denominator can be recovered.
    private static (Dictionary<string, (double DF, int G)> Meta,
                    Dictionary<string, string> TypeOfUnit,
                    Dictionary<string, string> UnitWordType,
                    Dictionary<string, List<(List<string> Path, string BaseUnit)>> UnitOpts)
        BuildPerCommon(System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
        var meta = new Dictionary<string, (double, int)>();
        var typeOfUnit = new Dictionary<string, string>();
        var unitWordType = new Dictionary<string, string>();
        var unitOpts = new Dictionary<string, List<(List<string>, string)>>();
        string StripS(string s) => s.Length > 1 && s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s;
        foreach (var info in all) {
            meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));
            var affine = new HashSet<string>();
            foreach (var spec in info.Units.Length == 0 ? System.Array.Empty<string>() : info.Units.Split(';')) {
                var parts = spec.Split('|');
                if (parts[0] == "u" && parts.Length >= 4 &&
                    double.TryParse(parts[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var off) && off != 0)
                    affine.Add(parts[1]);
            }
            var names = EnumerateUnitNames(info.Units);
            foreach (var n in names) if (!typeOfUnit.ContainsKey(n)) typeOfUnit[n] = info.Name;
            var single = new List<string>();
            foreach (var n in names) if (SplitWords(n).Count == 1) single.Add(n);
            foreach (var n in single) {
                if (!unitWordType.ContainsKey(n)) unitWordType[n] = info.Name;
                string sn = StripS(n);
                if (!unitWordType.ContainsKey(sn)) unitWordType[sn] = info.Name;
            }
            var rawSet = new HashSet<string>(single);
            var opts = new List<(List<string>, string)>();
            foreach (var n in single) {
                if (affine.Contains(n)) continue;
                var dec = DecomposeDenom(n, rawSet);
                string baseUnit = dec[dec.Count - 1];
                var path = new List<string>();
                for (int i = 0; i < dec.Count - 1; i++) path.Add(dec[i]);
                path.Add(StripS(baseUnit));
                opts.Add((path, baseUnit));
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
    // `.Per` (or, for a differently-typed factor, a bare unit word) appends another denominator.
    private static void EmitPerBuilders(SourceProductionContext ctx, System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var (meta, typeOfUnit, unitWordType, unitOpts) = BuildPerCommon(all);
        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        // Amount-of-substance (Quantity) stores an Avogadro count, but molar result types (g/mol, …)
        // treat the mole as the amount unit, so normalise the Quantity dimension to moles.
        bool HasMole = meta.ContainsKey("Quantity");
        string CoherentVal(string t, string valExpr) =>
            t == "Quantity" && HasMole ? $"({valExpr} / (Quantity.FromMoles(1).CanonicalValue))" : ToCoherentExpr(valExpr, Div(t));
        string FromCoherentVal(string t, string expr) =>
            t == "Quantity" && HasMole ? $"({expr} * (Quantity.FromMoles(1).CanonicalValue))" : FromCoherentExpr(expr, Div(t));
        string CUnit(string d, string baseUnit) => CoherentVal(d, $"({d}.From{baseUnit}(1).CanonicalValue)");
        string StripS(string s) => s.Length > 1 && s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s;

        // `.Square`/`.Cubic` denominator modifiers: an areal/cubic unit spelled as a length sub-walk
        // (`.Cubic.Centi.Meter`). Map singularised, prefix-decomposed length paths to the Area/Volume
        // unit name, so the factor uses that unit's exact coherent value (cm² = 1e-4 m², not (1e-2)²).
        var bases = new HashSet<string>();
        foreach (var info in all) foreach (var u in EnumerateUnitNames(info.Units)) if (SplitWords(u).Count == 1) bases.Add(u);
        var mod = new Dictionary<string, (string Partner, Dictionary<string, string> Term, Dictionary<string, SortedSet<string>> Children)>();
        foreach (var (kw, partner) in new[] { ("Square", "Area"), ("Cubic", "Volume") }) {
            if (!meta.ContainsKey(partner)) continue;
            var pinfo = all.First(a => a.Name == partner);
            var term = new Dictionary<string, string>();
            var childs = new Dictionary<string, SortedSet<string>>();
            foreach (var u in EnumerateUnitNames(pinfo.Units)) {
                var ws = SplitWords(u);
                if (ws.Count < 2 || ws[0] != kw) continue;
                var lw = new List<string>();
                for (int i = 1; i < ws.Count; i++) foreach (var d in DecomposeDenom(ws[i], bases)) lw.Add(StripS(d));
                string key = string.Join("_", lw);
                if (!term.ContainsKey(key)) term[key] = u;
                for (int k = 0; k < lw.Count; k++) {
                    string p = string.Join("_", lw.GetRange(0, k));
                    if (!childs.TryGetValue(p, out var set)) childs[p] = set = new SortedSet<string>(System.StringComparer.Ordinal);
                    set.Add(lw[k]);
                }
            }
            mod[kw] = (partner, term, childs);
        }

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
                    if (typeOfUnit.ContainsKey(string.Concat(ws.GetRange(0, k)))) { per = k; break; }
                }
                if (per < 0 || per == ws.Count - 1) continue;
                string N = typeOfUnit[string.Concat(ws.GetRange(0, per))];
                var types = new List<string>(); string lastT = ""; bool ok = true;
                for (int k = per + 1; k < ws.Count; k++) {
                    string w = ws[k];
                    if (w == "Squared") { if (lastT.Length == 0) { ok = false; break; } types.Add(lastT); }
                    else if (w == "Cubed") { if (lastT.Length == 0) { ok = false; break; } types.Add(lastT); types.Add(lastT); }
                    else if (w == "Square" || w == "Cubic") {
                        string t = w == "Square" ? "Area" : "Volume";
                        if (!meta.ContainsKey(t) || k + 1 >= ws.Count) { ok = false; break; }
                        k++; types.Add(t); lastT = t;
                    }
                    else { if (!unitWordType.TryGetValue(w, out var t)) { ok = false; break; } types.Add(t); lastT = t; }
                }
                if (!ok || types.Count == 0) continue;
                string key = string.Join(",", types);
                if (!seqMap.TryGetValue(N, out var inner)) seqMap[N] = inner = new();
                if (!inner.TryGetValue(key, out var rl)) inner[key] = rl = new List<string>();
                if (!rl.Contains(info.Name)) rl.Add(info.Name);
            }
        if (seqMap.Count == 0) return;

        string SU(List<string> s) => string.Join("_", s);
        List<string> AllowedNext(string N, List<string> S) {
            var set = new SortedSet<string>(System.StringComparer.Ordinal);
            foreach (var key in seqMap[N].Keys) {
                var parts = key.Split(',');
                if (parts.Length <= S.Count) continue;
                bool pre = true; for (int i = 0; i < S.Count; i++) if (parts[i] != S[i]) { pre = false; break; }
                if (pre) set.Add(parts[S.Count]);
            }
            return new List<string>(set);
        }
        List<string> TerminalR(string N, List<string> S) =>
            seqMap[N].TryGetValue(string.Join(",", S), out var rl) ? rl : new List<string>();

        var sb = new StringBuilder();
        var emittedCol = new HashSet<string>();
        var emittedPost = new HashSet<string>();
        var emittedMod = new HashSet<string>();

        // merged unit sub-trie over a set of denominator types: prefix children (accumulate pfx) and
        // base children (complete a factor of a given dimension).
        void BuildTrie(List<string> types,
            out Dictionary<string, List<(string W, int Exp, string Child)>> pref,
            out Dictionary<string, List<(string W, string Plural, string D, string BaseUnit)>> baseC) {
            pref = new(); baseC = new();
            foreach (var D in types) {
                if (!unitOpts.TryGetValue(D, out var opts)) continue;
                foreach (var (path, baseUnit) in opts) {
                    string prog = "";
                    for (int i = 0; i < path.Count; i++) {
                        string w = path[i];
                        if (i == path.Count - 1) {
                            if (!baseC.TryGetValue(prog, out var bl)) baseC[prog] = bl = new();
                            if (!bl.Exists(e => e.W == w)) bl.Add((w, baseUnit, D, baseUnit));
                        } else {
                            int exp = PrefixExponents.TryGetValue(w, out var e) ? e : 0;
                            string child = prog.Length == 0 ? w : prog + "_" + w;
                            if (!pref.TryGetValue(prog, out var pl)) pref[prog] = pl = new();
                            if (!pl.Exists(x => x.W == w)) pl.Add((w, exp, child));
                            prog = child;
                        }
                    }
                }
            }
        }

        string EmitMod(string N, List<string> S, string kw) {
            var (partner, term, childs) = mod[kw];
            string mid = "M_" + kw + "_" + N + (S.Count == 0 ? "" : "_" + SU(S));
            string root = mid + "__";
            if (!emittedMod.Add(mid)) return root;
            var progs = new HashSet<string> { "" };
            foreach (var kv in childs) { progs.Add(kv.Key); foreach (var w in kv.Value) progs.Add(kv.Key.Length == 0 ? w : kv.Key + "_" + w); }
            var ordered = new List<string>(progs); ordered.Sort(System.StringComparer.Ordinal);
            foreach (var prog in ordered) {
                var node = new StringBuilder();
                string nn = mid + "__" + prog;
                node.Append($"    public readonly struct {nn} {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append($"        internal {nn}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                if (childs.TryGetValue(prog, out var set))
                    foreach (var w in set) {
                        string cp = prog.Length == 0 ? w : prog + "_" + w;
                        if (term.TryGetValue(cp, out var unit)) {
                            string cu = CUnit(partner, unit);
                            string tgt = EmitPost(N, new List<string>(S) { partner });
                            node.Append($"        public {tgt} {w} => new(run / (pfx * {cu}), pfx * {cu}, 1.0);\n");
                        } else
                            node.Append($"        public {mid}__{cp} {w} => new(run, last, pfx);\n");
                    }
                node.Append("    }\n");
                sb.Append(node);
            }
            return root;
        }

        string EmitPost(string N, List<string> S) {
            string post = "P_" + N + "_" + SU(S);
            if (!emittedPost.Add(post)) return post;
            var body = new StringBuilder();
            body.Append($"    public readonly struct {post} {{\n");
            body.Append($"        private readonly double run, last, pfx;\n");
            body.Append($"        internal {post}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
            var Rl = TerminalR(N, S);
            if (Rl.Count == 1)
                body.Append($"        public static implicit operator {Rl[0]}({post} n) => {Rl[0]}.FromCanonical({FromCoherentVal(Rl[0], "n.run")});\n");
            else if (Rl.Count > 1)
                foreach (var R in Rl)
                    body.Append($"        public {R} {R} => {R}.FromCanonical({FromCoherentVal(R, "run")});\n");
            string Slast = S[S.Count - 1];
            var nxt = AllowedNext(N, S);
            if (nxt.Contains(Slast)) {
                var S1 = new List<string>(S) { Slast };
                body.Append($"        public {EmitPost(N, S1)} Squared => new(run / last, last, 1.0);\n");
                if (AllowedNext(N, S1).Contains(Slast)) {
                    var S2 = new List<string>(S1) { Slast };
                    body.Append($"        public {EmitPost(N, S2)} Cubed => new(run / last / last, last, 1.0);\n");
                }
            }
            var prodTypes = nxt.FindAll(t => t != Slast);
            if (prodTypes.Count > 0) {
                EmitCol(N, S, "prod", prodTypes);
                string colId = "C_" + N + "_prod" + (S.Count == 0 ? "" : "_" + SU(S));
                BuildTrie(prodTypes, out var pref, out var baseC);
                if (pref.TryGetValue("", out var pl))
                    foreach (var (w, exp, child) in pl)
                        body.Append($"        public {colId}__{child} {w} => new(run, last, 1.0{(exp != 0 ? $" * 1e{exp}" : "")});\n");
                if (baseC.TryGetValue("", out var bl))
                    foreach (var (w, plural, D, baseUnit) in bl) {
                        string cu = CUnit(D, baseUnit);
                        string tgt = EmitPost(N, new List<string>(S) { D });
                        string ctor = $"new(run / (1.0 * {cu}), 1.0 * {cu}, 1.0)";
                        body.Append($"        public {tgt} {w} => {ctor};\n");
                        if (plural != w) body.Append($"        public {tgt} {plural} => {ctor};\n");
                    }
                if (prodTypes.Contains("Area") && mod.ContainsKey("Square"))
                    body.Append($"        public {EmitMod(N, S, "Square")} Square => new(run, last, 1.0);\n");
                if (prodTypes.Contains("Volume") && mod.ContainsKey("Cubic"))
                    body.Append($"        public {EmitMod(N, S, "Cubic")} Cubic => new(run, last, 1.0);\n");
            }
            if (nxt.Count > 0)
                body.Append($"        public {EmitCol(N, S, "chain", nxt)} Per => new(run, last, 1.0);\n");
            body.Append("    }\n");
            sb.Append(body);
            return post;
        }

        string EmitCol(string N, List<string> S, string tag, List<string> types) {
            string colId = "C_" + N + "_" + tag + (S.Count == 0 ? "" : "_" + SU(S));
            string root = colId + "__";
            if (!emittedCol.Add(colId)) return root;
            BuildTrie(types, out var pref, out var baseC);
            var progs = new HashSet<string> { "" };
            foreach (var kv in pref) foreach (var c in kv.Value) progs.Add(c.Child);
            foreach (var kv in baseC) progs.Add(kv.Key);
            var ordered = new List<string>(progs); ordered.Sort(System.StringComparer.Ordinal);
            foreach (var prog in ordered) {
                string nn = colId + "__" + prog;
                var node = new StringBuilder();
                node.Append($"    public readonly struct {nn} {{\n");
                node.Append($"        private readonly double run, last, pfx;\n");
                node.Append($"        internal {nn}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                if (pref.TryGetValue(prog, out var pl))
                    foreach (var (w, exp, child) in pl)
                        node.Append($"        public {colId}__{child} {w} => new(run, last, pfx{(exp != 0 ? $" * 1e{exp}" : "")});\n");
                if (baseC.TryGetValue(prog, out var bl))
                    foreach (var (w, plural, D, baseUnit) in bl) {
                        string cu = CUnit(D, baseUnit);
                        string tgt = EmitPost(N, new List<string>(S) { D });
                        string ctor = $"new(run / (pfx * {cu}), pfx * {cu}, 1.0)";
                        node.Append($"        public {tgt} {w} => {ctor};\n");
                        if (plural != w) node.Append($"        public {tgt} {plural} => {ctor};\n");
                    }
                if (prog.Length == 0) {
                    if (types.Contains("Area") && mod.ContainsKey("Square"))
                        node.Append($"        public {EmitMod(N, S, "Square")} Square => new(run, last, pfx);\n");
                    if (types.Contains("Volume") && mod.ContainsKey("Cubic"))
                        node.Append($"        public {EmitMod(N, S, "Cubic")} Cubic => new(run, last, pfx);\n");
                }
                node.Append("    }\n");
                sb.Append(node);
            }
            return root;
        }

        var ext = new StringBuilder();
        foreach (var N in new SortedSet<string>(seqMap.Keys, System.StringComparer.Ordinal)) {
            string root = EmitCol(N, new List<string>(), "root", AllowedNext(N, new List<string>()));
            ext.Append($"        extension({N} x) {{ public {root} Per => new({CoherentVal(N, "x.CanonicalValue")}, 0.0, 1.0); }}\n");
        }

        var outSrc = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns + " {\n    public static partial class Units {\n");
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
    private static void EmitPerReadBuilders(SourceProductionContext ctx, System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var (meta, typeOfUnit, unitWordType, unitOpts) = BuildPerCommon(all);
        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);
        bool HasMole = meta.ContainsKey("Quantity");
        string CoherentVal(string t, string valExpr) =>
            t == "Quantity" && HasMole ? $"({valExpr} / (Quantity.FromMoles(1).CanonicalValue))" : ToCoherentExpr(valExpr, Div(t));
        string CUnit(string d, string baseUnit) => CoherentVal(d, $"({d}.From{baseUnit}(1).CanonicalValue)");
        string StripS(string s) => s.Length > 1 && s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s;

        var bases = new HashSet<string>();
        foreach (var info in all) foreach (var u in EnumerateUnitNames(info.Units)) if (SplitWords(u).Count == 1) bases.Add(u);
        var mod = new Dictionary<string, (string Partner, Dictionary<string, string> Term, Dictionary<string, SortedSet<string>> Children)>();
        foreach (var (kw, partner) in new[] { ("Square", "Area"), ("Cubic", "Volume") }) {
            if (!meta.ContainsKey(partner)) continue;
            var pinfo = all.First(a => a.Name == partner);
            var term = new Dictionary<string, string>();
            var childs = new Dictionary<string, SortedSet<string>>();
            foreach (var u in EnumerateUnitNames(pinfo.Units)) {
                var ws = SplitWords(u);
                if (ws.Count < 2 || ws[0] != kw) continue;
                var lw = new List<string>();
                for (int i = 1; i < ws.Count; i++) foreach (var d in DecomposeDenom(ws[i], bases)) lw.Add(StripS(d));
                string key = string.Join("_", lw);
                if (!term.ContainsKey(key)) term[key] = u;
                for (int k = 0; k < lw.Count; k++) {
                    string p = string.Join("_", lw.GetRange(0, k));
                    if (!childs.TryGetValue(p, out var set)) childs[p] = set = new SortedSet<string>(System.StringComparer.Ordinal);
                    set.Add(lw[k]);
                }
            }
            mod[kw] = (partner, term, childs);
        }

        string SU(List<string> s) => string.Join("_", s);
        List<string> ParseDenomTypes(List<string> ws, int per) {
            var types = new List<string>(); string lastT = ""; bool ok = true;
            for (int k = per + 1; k < ws.Count; k++) {
                string w = ws[k];
                if (w == "Per") continue;
                if (w == "Squared") { if (lastT.Length == 0) return new List<string>(); types.Add(lastT); }
                else if (w == "Cubed") { if (lastT.Length == 0) return new List<string>(); types.Add(lastT); types.Add(lastT); }
                else if (w == "Square" || w == "Cubic") {
                    string t = w == "Square" ? "Area" : "Volume";
                    if (!meta.ContainsKey(t) || k + 1 >= ws.Count) return new List<string>();
                    k++; types.Add(t); lastT = t;
                } else { if (!unitWordType.TryGetValue(w, out var t)) { ok = false; break; } types.Add(t); lastT = t; }
            }
            return ok && types.Count > 0 ? types : new List<string>();
        }

        // readMap[C][numFlat] = (numType, set of denominator type-sequences).
        var readMap = new Dictionary<string, Dictionary<string, (string NumType, HashSet<string> Seqs)>>();
        foreach (var info in all) {
            string C = info.Name;
            var names = EnumerateUnitNames(info.Units);
            var simple = new HashSet<string>();
            foreach (var u in names) if (!SplitWords(u).Contains("Per")) simple.Add(u);
            foreach (var u in names) {
                var ws = SplitWords(u);
                int per = ws.IndexOf("Per");
                if (per <= 0 || per == ws.Count - 1 || PerExcludeUnits.Contains(u)) continue;
                string numFlat = string.Concat(ws.GetRange(0, per));
                if (simple.Contains(numFlat) || !typeOfUnit.TryGetValue(numFlat, out var numType)) continue;
                var types = ParseDenomTypes(ws, per);
                if (types.Count == 0) continue;
                if (!readMap.TryGetValue(C, out var byNum)) readMap[C] = byNum = new();
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

        void BuildTrie(List<string> types,
            out Dictionary<string, List<(string W, int Exp, string Child)>> pref,
            out Dictionary<string, List<(string W, string Plural, string D, string BaseUnit)>> baseC) {
            pref = new(); baseC = new();
            foreach (var D in types) {
                if (!unitOpts.TryGetValue(D, out var opts)) continue;
                foreach (var (path, baseUnit) in opts) {
                    string prog = "";
                    for (int i = 0; i < path.Count; i++) {
                        string w = path[i];
                        if (i == path.Count - 1) {
                            if (!baseC.TryGetValue(prog, out var bl)) baseC[prog] = bl = new();
                            if (!bl.Exists(x => x.W == w)) bl.Add((w, baseUnit, D, baseUnit));
                        } else {
                            int exp = PrefixExponents.TryGetValue(w, out var e) ? e : 0;
                            string child = prog.Length == 0 ? w : prog + "_" + w;
                            if (!pref.TryGetValue(prog, out var pl)) pref[prog] = pl = new();
                            if (!pl.Exists(x => x.W == w)) pl.Add((w, exp, child));
                            prog = child;
                        }
                    }
                }
            }
        }

        // per (C, numFlat) helpers close over the current cn = C + "_" + numFlat key.
        foreach (var C in new SortedSet<string>(readMap.Keys, System.StringComparer.Ordinal)) {
            foreach (var numFlat in new SortedSet<string>(readMap[C].Keys, System.StringComparer.Ordinal)) {
                var (numType, seqs) = readMap[C][numFlat];
                string cn = C + "_" + numFlat;
                List<string> AllowedNext(List<string> S) {
                    var set = new SortedSet<string>(System.StringComparer.Ordinal);
                    foreach (var key in seqs) {
                        var parts = key.Split(',');
                        if (parts.Length <= S.Count) continue;
                        bool pre = true; for (int i = 0; i < S.Count; i++) if (parts[i] != S[i]) { pre = false; break; }
                        if (pre) set.Add(parts[S.Count]);
                    }
                    return new List<string>(set);
                }
                bool IsTerminal(List<string> S) => seqs.Contains(string.Join(",", S));

                string EmitMod(List<string> S, string kw) {
                    var (partner, term, childs) = mod[kw];
                    string mid = "RM_" + kw + "_" + cn + (S.Count == 0 ? "" : "_" + SU(S));
                    string root = mid + "__";
                    if (!emittedMod.Add(mid)) return root;
                    var progs = new HashSet<string> { "" };
                    foreach (var kv in childs) { progs.Add(kv.Key); foreach (var w in kv.Value) progs.Add(kv.Key.Length == 0 ? w : kv.Key + "_" + w); }
                    var ordered = new List<string>(progs); ordered.Sort(System.StringComparer.Ordinal);
                    foreach (var prog in ordered) {
                        var node = new StringBuilder();
                        string nn = mid + "__" + prog;
                        node.Append($"    public readonly struct {nn} {{\n");
                        node.Append($"        private readonly double run, last, pfx;\n");
                        node.Append($"        internal {nn}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                        if (childs.TryGetValue(prog, out var set))
                            foreach (var w in set) {
                                string cp = prog.Length == 0 ? w : prog + "_" + w;
                                if (term.TryGetValue(cp, out var unit)) {
                                    string cu = CUnit(partner, unit);
                                    string tgt = EmitPostRef(new List<string>(S) { partner });
                                    node.Append($"        public {tgt} {w} => new(run * (pfx * {cu}), pfx * {cu}, 1.0);\n");
                                } else
                                    node.Append($"        public {mid}__{cp} {w} => new(run, last, pfx);\n");
                            }
                        node.Append("    }\n");
                        sb.Append(node);
                    }
                    return root;
                }

                string EmitCol(List<string> S, List<string> types) {
                    string colId = "RC_" + cn + (S.Count == 0 ? "" : "_" + SU(S));
                    string root = colId + "__";
                    if (!emittedCol.Add(colId)) return root;
                    BuildTrie(types, out var pref, out var baseC);
                    var progs = new HashSet<string> { "" };
                    foreach (var kv in pref) foreach (var c in kv.Value) progs.Add(c.Child);
                    foreach (var kv in baseC) progs.Add(kv.Key);
                    var ordered = new List<string>(progs); ordered.Sort(System.StringComparer.Ordinal);
                    foreach (var prog in ordered) {
                        var node = new StringBuilder();
                        string nn = colId + "__" + prog;
                        node.Append($"    public readonly struct {nn} {{\n");
                        node.Append($"        private readonly double run, last, pfx;\n");
                        node.Append($"        internal {nn}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                        if (pref.TryGetValue(prog, out var pl))
                            foreach (var (w, exp, child) in pl)
                                node.Append($"        public {colId}__{child} {w} => new(run, last, pfx{(exp != 0 ? $" * 1e{exp}" : "")});\n");
                        if (baseC.TryGetValue(prog, out var bl))
                            foreach (var (w, plural, D, baseUnit) in bl) {
                                string cu = CUnit(D, baseUnit);
                                string tgt = EmitPostRef(new List<string>(S) { D });
                                string ctor = $"new(run * (pfx * {cu}), pfx * {cu}, 1.0)";
                                node.Append($"        public {tgt} {w} => {ctor};\n");
                                if (plural != w) node.Append($"        public {tgt} {plural} => {ctor};\n");
                            }
                        if (prog.Length == 0) {
                            if (types.Contains("Area") && mod.ContainsKey("Square"))
                                node.Append($"        public {EmitMod(S, "Square")} Square => new(run, last, pfx);\n");
                            if (types.Contains("Volume") && mod.ContainsKey("Cubic"))
                                node.Append($"        public {EmitMod(S, "Cubic")} Cubic => new(run, last, pfx);\n");
                        }
                        node.Append("    }\n");
                        sb.Append(node);
                    }
                    return root;
                }

                string EmitPostRef(List<string> S) {
                    string post = "RP_" + cn + "_" + SU(S);
                    if (!emittedPost.Add(post)) return post;
                    var body = new StringBuilder();
                    body.Append($"    public readonly struct {post} {{\n");
                    body.Append($"        private readonly double run, last, pfx;\n");
                    body.Append($"        internal {post}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                    if (IsTerminal(S))
                        body.Append($"        public static implicit operator double({post} n) => n.run;\n");
                    string Slast = S[S.Count - 1];
                    var nxt = AllowedNext(S);
                    if (nxt.Contains(Slast)) {
                        var S1 = new List<string>(S) { Slast };
                        body.Append($"        public {EmitPostRef(S1)} Squared => new(run * last, last, 1.0);\n");
                        if (AllowedNext(S1).Contains(Slast)) {
                            var S2 = new List<string>(S1) { Slast };
                            body.Append($"        public {EmitPostRef(S2)} Cubed => new(run * last * last, last, 1.0);\n");
                        }
                    }
                    if (nxt.Count > 0) {
                        EmitCol(S, nxt);
                        string colId = "RC_" + cn + "_" + SU(S);
                        BuildTrie(nxt, out var pref, out var baseC);
                        if (pref.TryGetValue("", out var pl))
                            foreach (var (w, exp, child) in pl)
                                body.Append($"        public {colId}__{child} {w} => new(run, last, 1.0{(exp != 0 ? $" * 1e{exp}" : "")});\n");
                        if (baseC.TryGetValue("", out var bl))
                            foreach (var (w, plural, D, baseUnit) in bl) {
                                string cu = CUnit(D, baseUnit);
                                string tgt = EmitPostRef(new List<string>(S) { D });
                                string ctor = $"new(run * (1.0 * {cu}), 1.0 * {cu}, 1.0)";
                                body.Append($"        public {tgt} {w} => {ctor};\n");
                                if (plural != w) body.Append($"        public {tgt} {plural} => {ctor};\n");
                            }
                        if (nxt.Contains("Area") && mod.ContainsKey("Square"))
                            body.Append($"        public {EmitMod(S, "Square")} Square => new(run, last, 1.0);\n");
                        if (nxt.Contains("Volume") && mod.ContainsKey("Cubic"))
                            body.Append($"        public {EmitMod(S, "Cubic")} Cubic => new(run, last, 1.0);\n");
                    }
                    body.Append("    }\n");
                    sb.Append(body);
                    return post;
                }

                string start = "RS_" + cn;
                string numCoh = CUnit(numType, numFlat);
                string runStart = $"(({ToCoherentExpr("r.Value.CanonicalValue", Div(C))}) / ({numCoh}) / r.Factor)";
                starts.Append($"        extension(Reader<{C}> r) {{ public {start} {numFlat} => new({runStart}, 0.0, 1.0); }}\n");
                string colRoot = EmitCol(new List<string>(), AllowedNext(new List<string>()));
                sb.Append($"    public readonly struct {start} {{\n");
                sb.Append($"        private readonly double run, last, pfx;\n");
                sb.Append($"        internal {start}(double run, double last, double pfx) {{ this.run = run; this.last = last; this.pfx = pfx; }}\n");
                sb.Append($"        public {colRoot} Per => new(run, last, pfx);\n");
                sb.Append("    }\n");
            }
        }

        var outSrc = new StringBuilder("// <auto-generated/>\n#nullable disable\n\nnamespace " + ns + " {\n    public static partial class Units {\n");
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
    private static void EmitTokenAlgebra(SourceProductionContext ctx, System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        var meta = new Dictionary<string, (double DF, int G)>();
        foreach (var info in all) meta[info.Name] = (info.DisplayFactor, GramPower(info.Symbol));
        (int, double) Div(string t) => CoherentDivisor(meta[t].DF, meta[t].G);

        // Single-word unit tokens → (type, unit). Plural forms as declared; singular base unit per
        // type is the entry token. `Light` is a synthetic Speed token (the speed of light).
        var tokenType = new Dictionary<string, (string Type, string Unit)>();       // multiplying tokens
        var entryToken = new Dictionary<string, (string Type, string Unit)>();       // Prefixed entry tokens
        var baseUnit = new Dictionary<string, string>();                             // type → its base unit
        foreach (var info in all) {
            foreach (var u in EnumerateUnitNames(info.Units)) {
                if (SplitWords(u).Count != 1) continue;
                if (!tokenType.ContainsKey(u)) tokenType[u] = (info.Name, u);
                if (!baseUnit.ContainsKey(info.Name)) baseUnit[info.Name] = u;       // first single-word unit
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
            foreach (var rel in info.Products.Length == 0 ? System.Array.Empty<string>() : info.Products.Split(';')) {
                var ab = rel.Split(',');
                if (ab.Length >= 2) AddPair(ab[0], ab[1], info.Name, ab.Length > 2 && ab[2] == "1");
            }
        List<string> Results(string r, string t) {
            string k = string.CompareOrdinal(r, t) <= 0 ? r + "|" + t : t + "|" + r;
            return pair.TryGetValue(k, out var l) ? l : new List<string>();
        }
        string PrimaryOf(string r, string t) {
            string k = string.CompareOrdinal(r, t) <= 0 ? r + "|" + t : t + "|" + r;
            return primaryResult.TryGetValue(k, out var p) ? p : "";
        }
        // Entry tokens: singular base unit of each product-participating type + Light.
        var participating = new HashSet<string>();
        foreach (var k in pair.Keys) { var ab = k.Split('|'); participating.Add(ab[0]); participating.Add(ab[1]); }
        foreach (var t in participating) {
            if (!baseUnit.TryGetValue(t, out var bu)) continue;
            string sing = bu.EndsWith("s") ? bu.Substring(0, bu.Length - 1) : bu;
            if (!tokenType.ContainsKey(sing) && !entryToken.ContainsKey(sing)) entryToken[sing] = (t, bu);
        }
        if (meta.ContainsKey("Speed")) entryToken["Light"] = ("Speed", "SpeedOfLight");

        // Reachable states via BFS over product transitions from the entry types.
        var states = new HashSet<string>();
        var queue = new Queue<string>();
        foreach (var e in entryToken.Values) if (states.Add(e.Type)) queue.Enqueue(e.Type);
        while (queue.Count > 0) {
            string r = queue.Dequeue();
            foreach (var tt in tokenType.Values)
                foreach (var c in Results(r, tt.Type))
                    if (states.Add(c)) queue.Enqueue(c);
        }

        // Value of the running state R (canonical of R, via `field`) times one unit u of type T,
        // expressed in the canonical of result C (coherent product then back to C's stored scale).
        string Combine(string r, string t, string u, string c, string field) {
            string coR = ToCoherentExpr(field, Div(r));
            string coU = ToCoherentExpr($"({t}.From{u}(1).CanonicalValue)", Div(t));
            return FromCoherentExpr($"{coR} * {coU}", Div(c));
        }

        // Square/Cubic modifier tries over the Area/Volume units, so a running state R can be scaled
        // by an areal/cubic unit via R × Area / R × Volume (e.g. Mass.Square.Meters → MomentOfInertia).
        var modTrie = new Dictionary<string, (string Partner, Dictionary<string, string> Term, Dictionary<string, SortedSet<string>> Children, List<string> Nodes)>();
        var bases = new HashSet<string>();
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units))
                if (SplitWords(u).Count == 1) bases.Add(u);
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
                        if (!childs.TryGetValue(p, out var set)) childs[p] = set = new SortedSet<string>(System.StringComparer.Ordinal);
                        set.Add(rest[k]);
                    }
                }
            }
            var ordered = new List<string>(nds); ordered.Sort(System.StringComparer.Ordinal);
            modTrie[mod] = (partner, term, childs, ordered);
        }

        var structs = new StringBuilder();
        var selectors = new StringBuilder();
        var seenSel = new HashSet<string>();
        var modStructs = new StringBuilder();
        foreach (var r in states.OrderBy(s => s, System.StringComparer.Ordinal)) {
            string nn = "ProductState_" + r;
            structs.Append($"    public readonly struct {nn} {{\n");
            structs.Append($"        internal readonly double c;\n");
            structs.Append($"        internal {nn}(double c) => this.c = c;\n");
            structs.Append($"        public static implicit operator {r}({nn} n) => {r}.FromCanonical(n.c);\n");
            // transitions, ordered by token
            foreach (var tok in tokenType.Keys.OrderBy(s => s, System.StringComparer.Ordinal)) {
                var (tt, tu) = tokenType[tok];
                var res = Results(r, tt);
                if (res.Count == 0) continue;
                if (res.Count == 1) {
                    string c = res[0];
                    structs.Append($"        public ProductState_{c} {tok} => new({Combine(r, tt, tu, c, "c")});\n");
                } else {
                    string sel = $"Sel_{r}_{tok}";
                    structs.Append($"        public {sel} {tok} => new(c);\n");
                    if (seenSel.Add(sel)) {
                        selectors.Append($"    public readonly struct {sel} {{\n        internal readonly double c;\n        internal {sel}(double c) => this.c = c;\n");
                        string prim = PrimaryOf(r, tt);
                        if (prim.Length != 0)
                            selectors.Append($"        public static implicit operator {prim}({sel} n) => global::{ns}.{prim}.FromCanonical({Combine(r, tt, tu, prim, "n.c")});\n");
                        foreach (var c in res)
                            selectors.Append($"        public ProductState_{c} {c} => new({Combine(r, tt, tu, c, "c")});\n");
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
                    modStructs.Append($"    public readonly struct {mn} {{\n        internal readonly double c;\n        internal {mn}(double c) => this.c = c;\n");
                    if (mt.Children.TryGetValue(p, out var set))
                        foreach (var w in set) {
                            string cp = p.Length == 0 ? w : p + "_" + w;
                            modStructs.Append($"        public {mod}Of_{r}_{cp} {w} => new(c);\n");
                        }
                    if (p.Length > 0 && mt.Term.TryGetValue(p, out var au)) {
                        var res = Results(r, mt.Partner);
                        if (res.Count == 1)
                            modStructs.Append($"        public static implicit operator {res[0]}({mn} n) => {res[0]}.FromCanonical({Combine(r, mt.Partner, au, res[0], "n.c")});\n");
                        else
                            foreach (var c in res)
                                modStructs.Append($"        public ProductState_{c} {c} => new({Combine(r, mt.Partner, au, c, "c")});\n");
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
        foreach (var tok in entryToken.Keys.OrderBy(s => s, System.StringComparer.Ordinal)) {
            var (ty, un) = entryToken[tok];
            prefixed.Append($"        public ProductState_{ty} {tok} => new({ty}.From{un}(p.Value).CanonicalValue);\n");
            dbl.Append($"        public ProductState_{ty} {tok} => new({ty}.From{un}(value).CanonicalValue);\n");
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

    // Emits `.Square`/`.Cubic` area & volume modifiers on Prefixed/double with a prefix-decomposed
    // length sub-walk, so `.Square.Milli.Meters` and `.Cubic.Centi.Meters` compose instead of needing
    // a flat `.SquareMillimeters` hook. Each path maps to the existing Square<Unit>/Cubic<Unit> factory
    // (and `.Square.Degrees` → SolidAngle). The value passes through unscaled; the factory does the work.
    private static void EmitSquareCubic(SourceProductionContext ctx, System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units, string Products)> all) {
        string ns = all.Length > 0 ? all[0].Namespace : "com.hafthor.Measurement";
        // Known single-word unit bases (for prefix splitting Millimeters → Milli, Meters).
        var bases = new HashSet<string>();
        foreach (var info in all)
            foreach (var u in EnumerateUnitNames(info.Units))
                if (SplitWords(u).Count == 1) bases.Add(u);

        // modifier ("Square"/"Cubic") → decomposed rest path → (type, full unit).
        foreach (var modifier in new[] { "Square", "Cubic" }) {
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
                            if (!children.TryGetValue(p, out var set)) children[p] = set = new SortedSet<string>(System.StringComparer.Ordinal);
                            set.Add(rest[k]);
                        }
                    }
                }
            if (terminal.Count == 0) continue;
            string Node(string p) => modifier + "Of" + (p.Length == 0 ? "" : "_" + p);
            var structs = new StringBuilder();
            var ordered = new List<string>(nodes); ordered.Sort(System.StringComparer.Ordinal);
            foreach (var p in ordered) {
                string nn = Node(p);
                structs.Append($"    public readonly struct {nn} {{\n        private readonly double v;\n        internal {nn}(double v) => this.v = v;\n");
                if (children.TryGetValue(p, out var set))
                    foreach (var w in set) {
                        string cp = p.Length == 0 ? w : p + "_" + w;
                        structs.Append($"        public {Node(cp)} {w} => new(v);\n");
                    }
                if (p.Length > 0 && terminal.TryGetValue(p, out var t))
                    structs.Append($"        public static implicit operator {t.Type}({nn} n) => {t.Type}.From{t.Unit}(n.v);\n");
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

    // Splits a denominator unit word into [SiPrefix, Base] when it starts with an SI prefix whose
    // un-prefixed base also occurs as a denominator word for the same numerator type (e.g. Kilogram →
    // [Kilo, Gram], Centimeter → [Centi, Meter], Milliliter → [Milli, Liter]); otherwise returns [w].
    private static List<string> DecomposeDenom(string w, HashSet<string> rawDenom) {
        string bestPrefix = "", bestBase = "";
        foreach (var kv in PrefixExponents) {
            if (kv.Value == 0 || w.Length <= kv.Key.Length || !w.StartsWith(kv.Key, System.StringComparison.Ordinal)) continue;
            string rest = w.Substring(kv.Key.Length);
            string baseW = char.ToUpperInvariant(rest[0]) + rest.Substring(1);
            if (rawDenom.Contains(baseW) && (bestPrefix.Length == 0 || kv.Key.Length > bestPrefix.Length)) {
                bestPrefix = kv.Key; bestBase = baseW;
            }
        }
        return bestPrefix.Length != 0 ? new List<string> { bestPrefix, bestBase } : new List<string> { w };
    }

    private static string Emit(string name, string ns, string symbol, double displayFactor, string variableName, string units) {
        string dfStr = displayFactor.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
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
