using System.Text;

namespace com.hafthor.Measurement;

// Independent ground-truth check: a hand-transcribed table of atomic unit conversions (keyed
// "Base/Unit", e.g. "Gram/Pound" = 453.59237 → 1 pound = 453.59237 g), with inverses generated
// automatically. Each unit-method name is decomposed into atoms (with SI prefixes; Square/Cubic
// raise to a power; factors after "Per" invert), the expected factor to the type's coherent base
// unit is computed, and the library's From/To is checked against it — e.g.
// Mass.FromPounds(1).ToGrams() == 453.59237, Volume.FromCubicFeet(1).ToCubicMeters() == 0.3048³,
// Speed.FromMilesPerHour(1).ToMetersPerSecond() == 1609.344/3600.
[TestClass]
public sealed class GroundTruthConversionTests {
    // "BaseAtom/Unit" → number of base atoms in one unit (1 Unit = value BaseAtom). Authoritative,
    // mostly exact-by-definition values; transcribed independently of the library's own factors.
    private static readonly Dictionary<string, double> Conversions = new() {
        // length (base: Meter)
        ["Meter/Mile"] = 1609.344, ["Meter/NauticalMile"] = 1852, ["Meter/League"] = 4828.032,
        ["Meter/Furlong"] = 201.168, ["Meter/Chain"] = 20.1168, ["Meter/Rod"] = 5.0292,
        ["Meter/Fathom"] = 1.8288, ["Meter/Yard"] = 0.9144, ["Meter/Foot"] = 0.3048,
        ["Meter/Inch"] = 0.0254, ["Meter/Angstrom"] = 1e-10,
        ["Meter/AstronomicalUnit"] = 149_597_870_700, ["Meter/LightYear"] = 9_460_730_472_580_800,
        // mass (base: Gram)
        ["Gram/Tonne"] = 1e6, ["Gram/Pound"] = 453.59237, ["Gram/Ounce"] = 28.349523125,
        ["Gram/LongTon"] = 1_016_046.9088, ["Gram/ShortTon"] = 907_184.74, ["Gram/Stone"] = 6350.29318,
        ["Gram/Dram"] = 1.7718451953125, ["Gram/Grain"] = 0.06479891, ["Gram/Slug"] = 14593.9029372,
        ["Gram/TroyPound"] = 373.2417216, ["Gram/TroyOunce"] = 31.1034768,
        ["Gram/Pennyweight"] = 1.55517384, ["Gram/Carat"] = 0.2,
        // time (base: Second) — years are Julian-year based in this library
        ["Second/Minute"] = 60, ["Second/Hour"] = 3600, ["Second/Day"] = 86400,
        ["Second/Week"] = 604800, ["Second/Fortnight"] = 1_209_600, ["Second/CommonYear"] = 31_536_000,
        ["Second/JulianYear"] = 31_557_600, ["Second/TropicalYear"] = 31_556_925.216,
        ["Second/SiderealYear"] = 31_558_149.7635, ["Second/SiderealDay"] = 86_164.0905,
        ["Second/Decade"] = 315_576_000, ["Second/Century"] = 3_155_760_000,
        ["Second/Millennium"] = 31_557_600_000, ["Second/Annum"] = 31_557_600,
        // angle (base: Radian)
        ["Radian/Turn"] = 2 * Math.PI, ["Radian/Revolution"] = 2 * Math.PI,
        ["Radian/Degree"] = Math.PI / 180, ["Radian/Gradian"] = Math.PI / 200,
        ["Radian/Arcminute"] = Math.PI / 10800, ["Radian/Arcsecond"] = Math.PI / 648000,
        // volume (base pseudo-atom: CubicMeter, dimension length³)
        ["CubicMeter/Liter"] = 1e-3, ["CubicMeter/Gallon"] = 0.003785411784,
        ["CubicMeter/Quart"] = 0.000946352946, ["CubicMeter/Pint"] = 0.000473176473,
        ["CubicMeter/Cup"] = 0.0002365882365, ["CubicMeter/FluidOunce"] = 2.95735295625e-5,
        ["CubicMeter/Tablespoon"] = 1.4786764828125e-5, ["CubicMeter/Teaspoon"] = 4.92892159375e-6,
        ["CubicMeter/ImperialGallon"] = 0.00454609, ["CubicMeter/OilBarrel"] = 0.158987294928,
        // area (base pseudo-atom: SquareMeter, dimension length²)
        ["SquareMeter/Acre"] = 4046.8564224, ["SquareMeter/Hectare"] = 10000,
        ["SquareMeter/Are"] = 100, ["SquareMeter/Barn"] = 1e-28,
    };

    // dimension exponents over (Length, Mass, Time, Angle)
    private static readonly Dictionary<string, (int L, int M, int T, int A)> BaseDim = new() {
        ["meter"] = (1, 0, 0, 0), ["gram"] = (0, 1, 0, 0), ["second"] = (0, 0, 1, 0),
        ["radian"] = (0, 0, 0, 1), ["cubicmeter"] = (3, 0, 0, 0), ["squaremeter"] = (2, 0, 0, 0),
    };

    // Standalone atoms with a compound dimension that can't be keyed by a single base atom.
    private static readonly Dictionary<string, (double Factor, (int L, int M, int T, int A) Dim)> CompoundAtoms = new() {
        ["knot"] = (1852.0 / 3600, (1, 0, -1, 0)),
        ["speedoflight"] = (299_792_458, (1, 0, -1, 0)),
        ["standardgravity"] = (9.80665, (1, 0, -2, 0)),
        ["gal"] = (0.01, (1, 0, -2, 0)),
        ["stoke"] = (1e-4, (2, 0, -1, 0)),
        ["tex"] = (1e-3, (-1, 1, 0, 0)),
        ["denier"] = (1.0 / 9000, (-1, 1, 0, 0)),
    };

    private static readonly (string Name, double Factor)[] Prefixes = [
        ("Quetta", 1e30), ("Ronna", 1e27), ("Yotta", 1e24), ("Zetta", 1e21), ("Exa", 1e18),
        ("Peta", 1e15), ("Tera", 1e12), ("Giga", 1e9), ("Mega", 1e6), ("Kilo", 1e3),
        ("Hecto", 1e2), ("Deca", 1e1), ("Deci", 1e-1), ("Centi", 1e-2), ("Milli", 1e-3),
        ("Micro", 1e-6), ("Nano", 1e-9), ("Pico", 1e-12), ("Femto", 1e-15), ("Atto", 1e-18),
        ("Zepto", 1e-21), ("Yocto", 1e-24), ("Ronto", 1e-27), ("Quecto", 1e-30),
    ];

    // atom (lowercase singular) → (factor to base atoms, dimension). Built from the tables above.
    private static readonly Dictionary<string, (double Factor, (int L, int M, int T, int A) Dim)> Atoms = BuildAtoms();

    private static Dictionary<string, (double, (int, int, int, int))> BuildAtoms() {
        var atoms = new Dictionary<string, (double, (int, int, int, int))>();
        foreach (var (root, dim) in BaseDim)
            if (root is "meter" or "gram" or "second" or "radian") atoms[root] = (1.0, dim);
        foreach (var (key, value) in Conversions) {
            int slash = key.IndexOf('/');
            string baseName = key[..slash].ToLowerInvariant();
            string unit = key[(slash + 1)..].ToLowerInvariant();
            atoms[unit] = (value, BaseDim[baseName]);
        }
        foreach (var (name, def) in CompoundAtoms) atoms[name] = def;
        return atoms;
    }

    private static readonly Dictionary<string, string> Irregular = new() {
        ["feet"] = "foot", ["centuries"] = "century", ["millennia"] = "millennium",
    };

    [TestMethod]
    public void UnitFactorsMatchIndependentGroundTruth() {
        var problems = new List<string>();
        int checkedUnits = 0;
        double[] samples = [1.0, 3.0, 0.5];
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            var froms = MeasurementReflection.Froms(t);
            var tos = MeasurementReflection.Tos(t);

            // coherent base reader: a unit whose factor is exactly 1 (all base atoms, no prefix)
            string baseName = null;
            (int, int, int, int) baseDim = default;
            foreach (var (unit, _) in tos.OrderBy(u => u.Key.Length)) {
                var d = Decompose(unit);
                if (d == null || !froms.ContainsKey(unit)) continue;
                if (Math.Abs(d.Value.Factor - 1.0) < 1e-12 && d.Value.Dim != (0, 0, 0, 0)) {
                    baseName = unit; baseDim = d.Value.Dim; break;
                }
            }
            if (baseName == null) continue;   // no coherent base reader in this table's dimensions

            var baseFrom = froms[baseName];
            var baseTo = tos[baseName];
            foreach (var (unit, from) in froms) {
                var d = Decompose(unit);
                if (d == null || d.Value.Dim != baseDim) continue;   // undecomposable or wrong dimension
                if (!tos.TryGetValue(unit, out var to)) continue;
                double expected = d.Value.Factor;
                checkedUnits++;

                // 1 Unit expressed in the base unit must equal the ground-truth factor.
                double got = (double)baseTo.Invoke(from.Invoke(null, [1.0]), null);
                if (!Close(got, expected)) {
                    problems.Add($"{t.Name}: From{unit}(1).To{baseName}() = {got}, ground truth {expected}");
                    continue;
                }
                // Inverse: a base value read back in the unit is divided by the factor.
                foreach (var v in samples) {
                    double back = (double)to.Invoke(baseFrom.Invoke(null, [v]), null);
                    if (!Close(back, v / expected)) {
                        problems.Add($"{t.Name}: From{baseName}({v}).To{unit}() = {back}, expected {v / expected}");
                        break;
                    }
                }
            }
        }
        if (problems.Count != 0) Assert.Fail(string.Join("\n", problems));
        if (checkedUnits < 60) Assert.Fail($"expected to ground-truth many units, only checked {checkedUnits}");
    }

    // --- unit-name decomposition into (total factor to base atoms, net dimension) ---

    private static (double Factor, (int L, int M, int T, int A) Dim)? Decompose(string name) {
        var words = SplitWords(name);
        double factor = 1;
        (int L, int M, int T, int A) dim = (0, 0, 0, 0);
        int side = 1, pendingMul = 1;
        double lastFactorLog = 0; (int, int, int, int) lastDimUnit = default; int lastSide = 0, lastMul = 0; bool hasLast = false;
        for (int i = 0; i < words.Count; i++) {
            string w = words[i];
            switch (w) {
                case "Per": side = -1; break;
                case "Square": pendingMul = 2; break;
                case "Cubic": pendingMul = 3; break;
                case "Squared":
                case "Cubed":
                    if (!hasLast) return null;
                    int target = w == "Squared" ? 2 : 3;
                    int extra = target - 1;                       // already applied power 1
                    factor *= Math.Pow(Math.Exp(lastFactorLog), lastSide * extra);
                    dim = Add(dim, Scale(lastDimUnit, lastSide * extra));
                    break;
                default:
                    // consume one or more consecutive non-keyword words as a single atom
                    int consumed = TryConsumeAtom(words, i, out double aFactor, out var aDim);
                    if (consumed == 0) return null;
                    i += consumed - 1;
                    int power = side * pendingMul;
                    factor *= Math.Pow(aFactor, power);
                    dim = Add(dim, Scale(aDim, power));
                    lastFactorLog = Math.Log(aFactor); lastDimUnit = aDim; lastSide = side; lastMul = pendingMul; hasLast = true;
                    pendingMul = 1;
                    break;
            }
        }
        return (factor, dim);
    }

    // Try the word at index alone (with optional SI prefix); else greedily combine up to 3
    // consecutive non-keyword words into one atom (e.g. Nautical+Miles → nauticalmile).
    private static int TryConsumeAtom(List<string> words, int index, out double factor, out (int, int, int, int) dim) {
        factor = 0; dim = default;
        // single word, optional prefix
        if (TryPrefixedAtom(words[index], out factor, out dim)) return 1;
        // multi-word phrase
        var sb = new StringBuilder(words[index]);
        for (int n = 1; n < 3 && index + n < words.Count; n++) {
            string next = words[index + n];
            if (IsKeyword(next)) break;
            sb.Append(next);
            if (LookupAtom(sb.ToString(), out factor, out dim)) return n + 1;
        }
        return 0;
    }

    private static bool TryPrefixedAtom(string word, out double factor, out (int, int, int, int) dim) {
        foreach (var (name, f) in Prefixes)
            if (word.StartsWith(name, StringComparison.Ordinal) && word.Length > name.Length
                && LookupAtom(word[name.Length..], out double bf, out dim)) { factor = bf * f; return true; }
        return LookupAtom(word, out factor, out dim);
    }

    private static bool LookupAtom(string phrase, out double factor, out (int, int, int, int) dim) {
        foreach (var candidate in Singulars(phrase.ToLowerInvariant()))
            if (Atoms.TryGetValue(candidate, out var a)) { factor = a.Item1; dim = a.Item2; return true; }
        factor = 0; dim = default;
        return false;
    }

    private static IEnumerable<string> Singulars(string w) {
        if (Irregular.TryGetValue(w, out var irr)) yield return irr;
        yield return w;
        if (w.EndsWith("ies")) yield return w[..^3] + "y";
        if (w.EndsWith("es")) yield return w[..^2];
        if (w.EndsWith('s')) yield return w[..^1];
    }

    private static bool IsKeyword(string w) => w is "Per" or "Square" or "Cubic" or "Squared" or "Cubed";

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

    private static (int, int, int, int) Add((int L, int M, int T, int A) a, (int L, int M, int T, int A) b)
        => (a.L + b.L, a.M + b.M, a.T + b.T, a.A + b.A);
    private static (int, int, int, int) Scale((int L, int M, int T, int A) a, int k)
        => (a.L * k, a.M * k, a.T * k, a.A * k);

    private static bool Close(double a, double b) {
        if (a == b) return true;
        return Math.Abs(a - b) <= 1e-9 * Math.Max(Math.Abs(a), Math.Abs(b));
    }
}
