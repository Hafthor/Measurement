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
                return (Name: type.Name, Namespace: type.ContainingNamespace.ToDisplayString(), Symbol: symbol, DisplayFactor: displayFactor, VariableName: variableName, Units: string.Join(";", unitSpecs));
            });

        // Struct body: identical-per-type boilerplate plus the unit From/To methods.
        context.RegisterSourceOutput(types, static (ctx, info) =>
            ctx.AddSource(info.Name + ".g.cs", SourceText.From(
                Emit(info.Name, info.Namespace, info.Symbol, info.DisplayFactor, info.VariableName, info.Units), Encoding.UTF8)));

        // Fluent sugar: needs every type at once so input-hook name collisions across
        // dimensionally-equivalent types can be detected and dropped.
        context.RegisterSourceOutput(types.Collect(), static (ctx, all) => EmitFluentSet(ctx, all));
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
    private static void EmitFluentSet(SourceProductionContext ctx, System.Collections.Immutable.ImmutableArray<(string Name, string Namespace, string Symbol, double DisplayFactor, string VariableName, string Units)> all) {
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
