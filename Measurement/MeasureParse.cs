using System.Reflection;

namespace com.hafthor.Measurement;

// Dynamic, unit-driven parsing: Measure.Parse("5 m/s") inspects the trailing unit symbol, picks
// the matching measurement type, and returns it boxed as IMeasurement. A recognised SI unit
// symbol is required — a bare number is ambiguous (e.g. Ratio vs Quantity) and is rejected.
public static partial class Measure {
    private static readonly Dictionary<string, Func<string, IFormatProvider, IMeasurement>> BySymbol = BuildRegistry();

    public static IMeasurement Parse(string s, IFormatProvider provider = null) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Could not parse as any measurement: {s}");

    public static bool TryParse(string s, out IMeasurement result) => TryParse(s, null, out result);

    public static bool TryParse(string s, IFormatProvider provider, out IMeasurement result) {
        result = null;
        if (string.IsNullOrWhiteSpace(s)) return false;
        string unit = ExtractUnit(s);
        if (unit.Length == 0) return false;                       // no unit → can't choose a type
        if (!BySymbol.TryGetValue(unit, out var parse)) return false;
        result = parse(s, provider);
        return result != null;
    }

    // The unit is whatever follows the last ASCII digit (SI symbols contain none — superscripts
    // like ² are non-ASCII), so this is culture-independent for the split.
    private static string ExtractUnit(string s) {
        s = s.Trim();
        int lastDigit = -1;
        for (int i = 0; i < s.Length; i++)
            if (s[i] is >= '0' and <= '9') lastDigit = i;
        return lastDigit < 0 ? "" : s[(lastDigit + 1)..].Trim();
    }

    private static Dictionary<string, Func<string, IFormatProvider, IMeasurement>> BuildRegistry() {
        var map = new Dictionary<string, Func<string, IFormatProvider, IMeasurement>>();
        var ambiguous = new HashSet<string>();
        foreach (var t in typeof(Measure).Assembly.GetTypes()) {
            if (!t.IsValueType) continue;
            var attr = t.GetCustomAttribute<MeasurementAttribute>();
            if (attr is null || attr.Symbol.Length == 0) continue;   // skip dimensionless
            var tryParse = t.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static,
                [typeof(string), typeof(IFormatProvider), t.MakeByRefType()]);
            if (tryParse is null) continue;
            Func<string, IFormatProvider, IMeasurement> parse = (str, prov) => {
                object[] args = [str, prov, null];
                return (bool)tryParse.Invoke(null, args) ? (IMeasurement)args[2] : null;
            };
            if (!map.TryAdd(attr.Symbol, parse)) ambiguous.Add(attr.Symbol);
        }
        foreach (var a in ambiguous) map.Remove(a);
        return map;
    }
}
