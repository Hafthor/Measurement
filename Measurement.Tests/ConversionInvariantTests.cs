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

    // SI prefixes (name → value of one prefixed unit in base units, e.g. Kilo → 1000).
    private static readonly (string Name, double Factor)[] Prefixes = [
        ("Quetta", 1e30), ("Ronna", 1e27), ("Yotta", 1e24), ("Zetta", 1e21), ("Exa", 1e18),
        ("Peta", 1e15), ("Tera", 1e12), ("Giga", 1e9), ("Mega", 1e6), ("Kilo", 1e3),
        ("Hecto", 1e2), ("Deca", 1e1), ("Deci", 1e-1), ("Centi", 1e-2), ("Milli", 1e-3),
        ("Micro", 1e-6), ("Nano", 1e-9), ("Pico", 1e-12), ("Femto", 1e-15), ("Atto", 1e-18),
        ("Zepto", 1e-21), ("Yocto", 1e-24), ("Ronto", 1e-27), ("Quecto", 1e-30),
    ];

    // A prefixed unit (FromKilograms / ToMilligrams) must agree exactly with its base unit scaled
    // by the SI prefix: FromKiloXxx(1) == FromXxx(1000), and FromXxx(v).ToMilliXxx() == v * 1000.
    // A pair is any FromPrefixBase whose FromBase (and ToBase) also exist. This catches a mis-typed
    // prefix factor or an asymmetric From/To on a prefixed unit.
    [TestMethod]
    public void SiPrefixedUnitsAgreeWithScaledBaseUnit() {
        var problems = new List<string>();
        int pairs = 0;
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            var froms = MeasurementReflection.Froms(t);
            var tos = MeasurementReflection.Tos(t);
            foreach (var (full, fromFull) in froms) {
                if (!TryMatchPrefix(full, froms.Keys, out string baseUnit, out double factor)) continue;
                if (!tos.TryGetValue(full, out var toFull)) continue;         // covered by pairing test
                var fromBase = froms[baseUnit];
                var toBase = tos[baseUnit];
                pairs++;
                foreach (var v in Samples) {
                    // Construction: FromPrefix(v) builds the same canonical as FromBase(v * factor).
                    double canonPrefix = MeasurementReflection.Canonical(fromFull.Invoke(null, [v]));
                    double canonBase = MeasurementReflection.Canonical(fromBase.Invoke(null, [v * factor]));
                    if (!Close(canonPrefix, canonBase)) {
                        problems.Add($"{t.Name}: From{full}({v}) canonical {canonPrefix} != From{baseUnit}({v * factor}) canonical {canonBase}");
                        break;
                    }
                    // Read-out: base value read in the prefixed unit is scaled by 1/factor
                    // (e.g. FromXxx(v).ToMilliXxx() == v * 1000), and symmetrically the other way.
                    object baseInst = fromBase.Invoke(null, [v]);
                    double readPrefixed = (double)toFull.Invoke(baseInst, null);
                    if (!Close(readPrefixed, v / factor)) {
                        problems.Add($"{t.Name}: From{baseUnit}({v}).To{full}() = {readPrefixed}, expected {v / factor}");
                        break;
                    }
                    object prefixedInst = fromFull.Invoke(null, [v]);
                    double readBase = (double)toBase.Invoke(prefixedInst, null);
                    if (!Close(readBase, v * factor)) {
                        problems.Add($"{t.Name}: From{full}({v}).To{baseUnit}() = {readBase}, expected {v * factor}");
                        break;
                    }
                }
            }
        }
        if (problems.Count != 0) Assert.Fail(string.Join("\n", problems));
        if (pairs < 60) Assert.Fail($"expected many SI-prefix pairs, only found {pairs}");
    }

    // A unit name is "PrefixBase" when it starts with an SI prefix and the remainder (re-capitalised)
    // is itself an available base unit — e.g. "Kilograms" → "Kilo" + "Grams". No SI prefix name is a
    // prefix of another, so at most one prefix can match.
    private static bool TryMatchPrefix(string full, IEnumerable<string> units, out string baseUnit, out double factor) {
        baseUnit = null; factor = 0;
        foreach (var (name, f) in Prefixes) {
            if (!full.StartsWith(name, StringComparison.Ordinal) || full.Length <= name.Length) continue;
            string rest = full[name.Length..];
            rest = char.ToUpperInvariant(rest[0]) + rest[1..];
            if (units.Contains(rest)) { baseUnit = rest; factor = f; return true; }
            return false; // starts with this prefix but no matching base → not a prefix pair
        }
        return false;
    }

    private static bool Close(double a, double b) {
        if (a == b) return true;
        double scale = Math.Max(Math.Abs(a), Math.Abs(b));
        return Math.Abs(a - b) <= 1e-9 * scale;
    }

    // Metric roots whose value in coherent SI base units is irrelevant here — only their identity
    // matters, because compound units are only compared within the same dimensional signature (so
    // the shared root values cancel in the ratio). Recognising a name as fully built from these
    // (with SI prefixes, Square/Cubic powers, and Per for division) is what makes it decomposable.
    private static readonly HashSet<string> MetricRoots = [
        "meter", "gram", "second", "mole", "kelvin", "ampere", "candela", "liter",
        "radian", "steradian", "newton", "joule", "watt", "pascal", "hertz", "coulomb",
        "volt", "ohm", "siemens", "farad", "henry", "weber", "tesla", "gray", "sievert",
        "katal", "becquerel", "lumen", "lux", "poise", "stoke", "bar", "gauss",
    ];
    private static readonly Dictionary<string, string> IrregularSingular = new() { ["henries"] = "henry" };

    // Compound metric units built with Per / Square / Cubic must scale consistently: within one
    // dimensional signature, FromA(1)/FromB(1) equals the ratio of their independently-computed
    // prefix-and-power scales — e.g. FromCubicMetersPerKilogram(1) == FromCubicCentimetersPerGram(1000)
    // (metre→centimetre cubed, and the opposite direction after "Per" for kilogram→gram). Read-out
    // scales inversely. Catches a wrong prefix/power on any factor of a compound unit.
    [TestMethod]
    public void CompoundMetricUnitsScaleConsistently() {
        var problems = new List<string>();
        int groups = 0;
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            var froms = MeasurementReflection.Froms(t);
            var tos = MeasurementReflection.Tos(t);
            // group decomposable units by dimensional signature
            var bySig = new Dictionary<string, List<(string Unit, double Scale)>>();
            foreach (var unit in froms.Keys) {
                if (!tos.ContainsKey(unit)) continue;
                var atoms = Decompose(unit);
                if (atoms == null) continue;
                string sig = Signature(atoms);
                if (IsTrivial(atoms)) continue;   // single root, power +1 → the simple-prefix test covers it
                double scale = 1;
                foreach (var (_, prefix, power) in atoms) scale *= Math.Pow(prefix, power);
                (bySig.TryGetValue(sig, out var list) ? list : bySig[sig] = []).Add((unit, scale));
            }
            foreach (var group in bySig.Values) {
                if (group.Count < 2) continue;
                groups++;
                var reference = group.OrderBy(u => Math.Abs(Math.Log(Math.Abs(u.Scale)))).First(); // scale nearest 1
                foreach (var u in group) {
                    if (u.Unit == reference.Unit) continue;
                    double expected = u.Scale / reference.Scale;
                    // Construction: FromU(1) builds the same canonical as FromRef(expected).
                    double canonU = MeasurementReflection.Canonical(froms[u.Unit].Invoke(null, [1.0]));
                    double canonRef = MeasurementReflection.Canonical(froms[reference.Unit].Invoke(null, [expected]));
                    if (!Close(canonU, canonRef)) {
                        problems.Add($"{t.Name}: From{u.Unit}(1) != From{reference.Unit}({expected}) ({canonU} vs {canonRef})");
                        continue;
                    }
                    // Read-out scales inversely: a reference value read in unit U is divided by expected.
                    foreach (var v in Samples) {
                        object refInst = froms[reference.Unit].Invoke(null, [v]);
                        double readU = (double)tos[u.Unit].Invoke(refInst, null);
                        if (!Close(readU, v / expected)) {
                            problems.Add($"{t.Name}: From{reference.Unit}({v}).To{u.Unit}() = {readU}, expected {v / expected}");
                            break;
                        }
                    }
                }
            }
        }
        if (problems.Count != 0) Assert.Fail(string.Join("\n", problems));
        if (groups < 15) Assert.Fail($"expected many compound-unit groups, only found {groups}");
    }

    // Decomposes a unit-method name (e.g. "CubicMetersPerKilogram") into atoms of (root, prefixFactor,
    // signedPower). Returns null if any word is not a structural keyword or a (prefix?+known-root),
    // i.e. the unit isn't fully metric — those (Miles, WattHours, MetersOfMercury, …) are skipped.
    private static List<(string Root, double Prefix, int Power)> Decompose(string name) {
        var atoms = new List<(string Root, double Prefix, int Power)>();
        int side = 1, pendingMul = 1, lastIndex = -1;
        foreach (var word in SplitWords(name)) {
            switch (word) {
                case "Per": side = -1; break;
                case "Square": pendingMul = 2; break;
                case "Cubic": pendingMul = 3; break;
                case "Squared":
                    if (lastIndex < 0) return null;
                    atoms[lastIndex] = (atoms[lastIndex].Root, atoms[lastIndex].Prefix, Math.Sign(atoms[lastIndex].Power) * 2);
                    break;
                case "Cubed":
                    if (lastIndex < 0) return null;
                    atoms[lastIndex] = (atoms[lastIndex].Root, atoms[lastIndex].Prefix, Math.Sign(atoms[lastIndex].Power) * 3);
                    break;
                default:
                    if (!TryAtom(word, out string root, out double prefix)) return null;
                    atoms.Add((root, prefix, side * pendingMul));
                    lastIndex = atoms.Count - 1;
                    pendingMul = 1;
                    break;
            }
        }
        return atoms.Count == 0 ? null : atoms;
    }

    private static bool TryAtom(string word, out string root, out double prefix) {
        foreach (var (name, f) in Prefixes)
            if (word.StartsWith(name, StringComparison.Ordinal) && word.Length > name.Length
                && IsRoot(word[name.Length..], out root)) { prefix = f; return true; }
        prefix = 1;
        return IsRoot(word, out root);
    }

    private static bool IsRoot(string word, out string root) {
        string w = word.ToLowerInvariant();
        if (IrregularSingular.TryGetValue(w, out root)) return true;
        if (w.EndsWith('s') && MetricRoots.Contains(w[..^1])) { root = w[..^1]; return true; }
        root = w;
        return MetricRoots.Contains(w);
    }

    private static string Signature(List<(string Root, double Prefix, int Power)> atoms) {
        var net = new Dictionary<string, int>();
        foreach (var (root, _, power) in atoms) net[root] = net.GetValueOrDefault(root) + power;
        return string.Join(";", net.Where(kv => kv.Value != 0).OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}"));
    }

    private static bool IsTrivial(List<(string Root, double Prefix, int Power)> atoms) {
        var net = new Dictionary<string, int>();
        foreach (var (root, _, power) in atoms) net[root] = net.GetValueOrDefault(root) + power;
        net = net.Where(kv => kv.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value);
        return net.Count == 1 && net.Values.First() == 1;
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
