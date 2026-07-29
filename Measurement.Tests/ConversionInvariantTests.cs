using System.Reflection;
using System.Text;

namespace com.hafthor.Measurement;

// Mechanical safety net over every [Measurement] type. These reflect over the whole assembly, so
// a new type or unit is covered automatically. They catch the classes of bug found during
// development: asymmetric From/To factors, a missing From/To counterpart, and a wrong/missing
// DisplayFactor.
[TestClass]
public sealed class ConversionInvariantTests {
    private static readonly double[] Samples = [1.0, 2.5, 7.0, 1234.0];

    [TestMethod]
    public void EveryFromHasAToAndEveryToHasAFrom() {
        var problems = new List<string>();
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            var froms = MeasurementReflection.Froms(t);
            var tos = MeasurementReflection.Tos(t);
            foreach (var u in froms.Keys)
                if (!tos.ContainsKey(u)) problems.Add($"{t.Name}.From{u} has no To{u}");
            foreach (var u in tos.Keys)
                if (!froms.ContainsKey(u)) problems.Add($"{t.Name}.To{u} has no From{u}");
        }
        if (problems.Count != 0) Assert.Fail(string.Join("\n", problems));
    }

    [TestMethod]
    public void EveryUnitRoundTrips() {
        var problems = new List<string>();
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            var froms = MeasurementReflection.Froms(t);
            var tos = MeasurementReflection.Tos(t);
            foreach (var (unit, from) in froms) {
                if (unit.Contains("Decibel")) continue; // logarithmic: inherently lossy at extremes
                if (!tos.TryGetValue(unit, out var to)) continue;
                foreach (var s in Samples) {
                    object inst = from.Invoke(null, [s]);
                    double back = (double)to.Invoke(inst, null);
                    double relErr = Math.Abs(back - s) / Math.Abs(s);
                    if (relErr > 1e-9) {
                        problems.Add($"{t.Name}: From{unit}({s}).To{unit}() = {back} (relErr {relErr:E2})");
                        break;
                    }
                }
            }
        }
        if (problems.Count != 0) Assert.Fail(string.Join("\n", problems));
    }

    // ToString shows the fundamental SI unit by dividing the stored value by DisplayFactor. So one
    // display unit corresponds to a stored (canonical) value of exactly DisplayFactor, and reading
    // that back in the display unit must give 1. This catches a wrong or missing DisplayFactor.
    [TestMethod]
    public void DisplayFactorAgreesWithTheSymbolUnit() {
        var problems = new List<string>();
        int verified = 0;
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            string symbol = MeasurementReflection.Symbol(t);
            if (symbol.Length == 0) continue; // dimensionless: nothing to match
            var symbolKey = UnitKeyFromSymbol(symbol);
            if (symbolKey is null) continue;

            var tos = MeasurementReflection.Tos(t);
            MethodInfo displayReader = null;
            foreach (var (unit, to) in tos)
                if (UnitKeyFromMethodName(unit) == symbolKey) { displayReader = to; break; }
            if (displayReader is null) continue; // no reader in the exact display unit → can't check

            double df = MeasurementReflection.DisplayFactor(t);
            object oneDisplayUnit = MeasurementReflection.FromCanonical(t, df);
            double read = (double)displayReader.Invoke(oneDisplayUnit, null);
            if (Math.Abs(read - 1.0) > 1e-9)
                problems.Add($"{t.Name}: symbol '{symbol}' but reading 1 display-unit gives {read} (check DisplayFactor {df})");
            verified++;
        }
        if (problems.Count != 0) Assert.Fail(string.Join("\n", problems));
        if (verified < 40) Assert.Fail($"expected the resolver to verify many types, only did {verified}");
    }

    // ---- symbol / unit-name normalization (bag of unit roots: "numRoots|denRoots") ----

    private static readonly Dictionary<string, string> Atom = new() {
        ["m"] = "meter", ["g"] = "gram", ["s"] = "second", ["A"] = "ampere", ["K"] = "kelvin",
        ["mol"] = "mole", ["cd"] = "candela", ["rad"] = "radian", ["sr"] = "steradian",
        ["Hz"] = "hertz", ["N"] = "newton", ["Pa"] = "pascal", ["J"] = "joule", ["W"] = "watt",
        ["C"] = "coulomb", ["V"] = "volt", ["F"] = "farad", ["Ω"] = "ohm", ["S"] = "siemens",
        ["Wb"] = "weber", ["T"] = "tesla", ["H"] = "henry", ["lm"] = "lumen", ["lx"] = "lux",
        ["Bq"] = "becquerel", ["Gy"] = "gray", ["Sv"] = "sievert", ["kat"] = "katal",
    };
    private static readonly HashSet<string> Roots = [.. Atom.Values];

    private static string UnitKeyFromSymbol(string symbol) {
        var num = new List<string>();
        var den = new List<string>();
        string[] halves = symbol.Split('/');
        if (halves.Length > 2) return null;
        for (int h = 0; h < halves.Length; h++) {
            var side = h == 0 ? num : den;
            string cleaned = halves[h].Replace("(", "").Replace(")", "");
            if (cleaned.Length == 0) continue;
            foreach (var token in cleaned.Split('·')) {
                if (!ParseSymbolToken(token, out string root, out int power)) return null;
                var target = power < 0 ? (side == num ? den : num) : side;
                for (int i = 0; i < Math.Abs(power); i++) target.Add(root);
            }
        }
        return Key(num, den);
    }

    private static bool ParseSymbolToken(string token, out string root, out int power) {
        root = null; power = 1;
        int end = token.Length;
        int sign = 1, magnitude = 0; bool hasExp = false;
        while (end > 0 && "⁰¹²³⁴⁵⁶⁷⁸⁹⁻".Contains(token[end - 1])) {
            char c = token[--end];
            if (c == '⁻') sign = -1;
            else { magnitude = magnitude * 10 + "⁰¹²³⁴⁵⁶⁷⁸⁹".IndexOf(c); hasExp = true; }
        }
        string letters = token[..end];
        if (!Atom.TryGetValue(letters, out root)) return false;
        power = hasExp ? sign * Math.Max(magnitude, 1) : sign;
        return true;
    }

    private static readonly Dictionary<string, string> Singular = new() {
        ["henries"] = "henry", ["siemens"] = "siemens", ["hertz"] = "hertz",
        ["lux"] = "lux", ["kelvin"] = "kelvin",
    };

    private static string UnitKeyFromMethodName(string name) {
        var num = new List<string>();
        var den = new List<string>();
        var side = num;
        int pendingMul = 1;
        List<string> lastSide = null; string lastRoot = null;
        foreach (var word in SplitWords(name)) {
            switch (word) {
                case "Per": side = den; break;
                case "Square": pendingMul = 2; break;
                case "Cubic": pendingMul = 3; break;
                case "Squared": Repeat(lastSide, lastRoot, 2); break;
                case "Cubed": Repeat(lastSide, lastRoot, 3); break;
                default:
                    string lower = word.ToLowerInvariant();
                    string root = Singular.TryGetValue(lower, out var sp) ? sp
                        : lower.EndsWith('s') ? lower[..^1] : lower;
                    if (!Roots.Contains(root)) return null;
                    for (int i = 0; i < pendingMul; i++) side.Add(root);
                    lastSide = side; lastRoot = root; pendingMul = 1;
                    break;
            }
        }
        return Key(num, den);
    }

    private static void Repeat(List<string> side, string root, int total) {
        if (side is null || root is null) return;
        int have = side.Count(x => x == root);
        for (int i = have; i < total; i++) side.Add(root);
    }

    private static IEnumerable<string> SplitWords(string camel) {
        var sb = new StringBuilder();
        foreach (char c in camel) {
            if (char.IsUpper(c) && sb.Length > 0) { yield return sb.ToString(); sb.Clear(); }
            sb.Append(c);
        }
        if (sb.Length > 0) yield return sb.ToString();
    }

    private static string Key(List<string> num, List<string> den) {
        num.Sort(); den.Sort();
        return string.Join(",", num) + "|" + string.Join(",", den);
    }
}
