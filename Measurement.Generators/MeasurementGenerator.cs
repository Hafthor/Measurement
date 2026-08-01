using System.Collections.Generic;
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
    }

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

    // Computes the fluent hook candidates for one struct: name → canonical factor for every
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

        EmitCollisionSelectors(ctx, perType, inputCounts);
    }

    // For each input-hook name shared by more than one measurement, emits a selector struct carrying
    // the scaled value with a property per measurement (1.0.JouleSeconds.Action), plus the single
    // shared Prefixed/double hook that returns it — resolving what would be a duplicate-member clash.
    private static void EmitCollisionSelectors(SourceProductionContext ctx, List<(string Name, string Ns, List<string> Hooks)> perType, Dictionary<string, int> inputCounts) {
        // Colliding name → owning measurements, in stable declaration order.
        var owners = new List<KeyValuePair<string, List<string>>>();
        var seen = new HashSet<string>();
        foreach (var (name, _, hooks) in perType)
            foreach (var u in hooks)
                if (inputCounts[u] > 1 && seen.Add(u))
                    owners.Add(new KeyValuePair<string, List<string>>(u, new List<string>()));
        foreach (var (name, _, hooks) in perType)
            foreach (var u in hooks)
                if (inputCounts[u] > 1)
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
