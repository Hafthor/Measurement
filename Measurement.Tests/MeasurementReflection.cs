using System.Reflection;

namespace com.hafthor.Measurement;

// Reflection helpers shared by the reflective test suites. Discovers [Measurement] types and
// their FromXxx/ToXxx unit methods without needing InternalsVisibleTo (the attribute is matched
// by name via CustomAttributesData).
internal static class MeasurementReflection {
    public static IEnumerable<Type> AllMeasurementTypes() =>
        typeof(Length).Assembly.GetTypes()
            .Where(t => t.IsValueType && AttributeData(t) != null)
            .OrderBy(t => t.Name);

    public static CustomAttributeData AttributeData(Type t) =>
        t.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.Name == "MeasurementAttribute");

    public static string Symbol(Type t) {
        var a = AttributeData(t);
        return a?.ConstructorArguments.Count > 0 ? a.ConstructorArguments[0].Value as string ?? "" : "";
    }

    public static double DisplayFactor(Type t) {
        var a = AttributeData(t);
        foreach (var na in a.NamedArguments)
            if (na.MemberName == "DisplayFactor") return System.Convert.ToDouble(na.TypedValue.Value);
        return 1.0;
    }

    // FromUnit(double) static factories, keyed by unit name (without the "From" prefix). The unit
    // factories are internal (fluent is the public surface), reachable here via InternalsVisibleTo.
    public static Dictionary<string, MethodInfo> Froms(Type t) =>
        t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Where(m => m.Name.StartsWith("From") && m.Name != "FromCanonical"
                        && m.ReturnType == t
                        && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(double))
            .ToDictionary(m => m.Name[4..], m => m);

    // ToUnit() instance readers returning double, keyed by unit name (without the "To" prefix).
    public static Dictionary<string, MethodInfo> Tos(Type t) =>
        t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("To") && m.Name != "ToString"
                        && m.ReturnType == typeof(double) && m.GetParameters().Length == 0)
            .ToDictionary(m => m.Name[2..], m => m);

    public static object FromCanonical(Type t, double v) =>
        t.GetMethod("FromCanonical", BindingFlags.Public | BindingFlags.Static).Invoke(null, [v]);

    public static double Canonical(object instance) =>
        (double)instance.GetType().GetProperty("CanonicalValue").GetValue(instance);

    // SI prefix ten-exponents (mirrors the generator). Leading-prefix aliases (Kilometers = Kilo·
    // Meters) have no generated From*/To* method — they are synthesized from the base unit + scaling.
    private static readonly Dictionary<string, int> PrefixExponents = new() {
        ["Quetta"] = 30, ["Ronna"] = 27, ["Yotta"] = 24, ["Zetta"] = 21, ["Exa"] = 18,
        ["Peta"] = 15, ["Tera"] = 12, ["Giga"] = 9, ["Mega"] = 6, ["Kilo"] = 3,
        ["Hecto"] = 2, ["Deca"] = 1, ["Deci"] = -1, ["Centi"] = -2,
        ["Milli"] = -3, ["Micro"] = -6, ["Nano"] = -9, ["Pico"] = -12, ["Femto"] = -15,
        ["Atto"] = -18, ["Zepto"] = -21, ["Yocto"] = -24, ["Ronto"] = -27, ["Quecto"] = -30,
    };

    // (base unit, ten-exponent) if `unit` is a leading-SI-prefix alias of a base unit present in
    // `known`; otherwise null. Mirrors the generator's removable-alias synthesis.
    private static (string Base, int Exp)? PrefixAlias(string unit, ICollection<string> known) {
        foreach (var kv in PrefixExponents) {
            if (!unit.StartsWith(kv.Key)) continue;
            string rest = unit[kv.Key.Length..];
            if (rest.Length == 0) continue;
            string baseUnit = char.ToUpperInvariant(rest[0]) + rest[1..];
            if (baseUnit != unit && known.Contains(baseUnit)) return (baseUnit, kv.Value);
        }
        return null;
    }

    // Applies a From<fromUnit> … To<toUnit> conversion via reflection — for white-box checks (e.g.
    // bit-exact storage anchoring) that must read the exact stored value. Leading-prefix aliases have
    // no generated method; they are synthesized by folding the prefix into the base unit's canonical
    // factor as a single power-of-ten multiply, preserving the storage exactness the direct factories
    // used to give.
    public static double Convert(Type t, string fromUnit, double value, string toUnit) {
        var froms = Froms(t);
        var tos = Tos(t);
        double canon;
        if (froms.TryGetValue(fromUnit, out var fm)) canon = Canonical(fm.Invoke(null, [value]));
        else {
            var a = PrefixAlias(fromUnit, froms.Keys) ?? throw new KeyNotFoundException($"No From method or prefix alias for '{fromUnit}' on {t.Name}");
            double baseFactor = Canonical(froms[a.Base].Invoke(null, [1.0]));
            // Fold the prefix into the base's ten-exponent as one integer, so the scale is a single
            // literal power of ten (1e{baseExp+prefixExp}) — matching the generator bit-for-bit, rather
            // than multiplying two powers of ten (1e-9 is not exactly representable).
            canon = TryTenExponent(baseFactor, out int be)
                ? value * System.Math.Pow(10, be + a.Exp)
                : value * (baseFactor * System.Math.Pow(10, a.Exp));
        }
        if (tos.TryGetValue(toUnit, out var tm)) return (double)tm.Invoke(FromCanonical(t, canon), null);
        var b = PrefixAlias(toUnit, tos.Keys) ?? throw new KeyNotFoundException($"No To method or prefix alias for '{toUnit}' on {t.Name}");
        double baseToFactor = Canonical(froms[b.Base].Invoke(null, [1.0]));
        return TryTenExponent(baseToFactor, out int te)
            ? canon / System.Math.Pow(10, te + b.Exp)
            : canon / (baseToFactor * System.Math.Pow(10, b.Exp));
    }

    // True (with the exponent) when f is an exact power of ten — the SI base units anchor on one.
    private static bool TryTenExponent(double f, out int exp) {
        exp = 0;
        if (f <= 0) return false;
        exp = (int)System.Math.Round(System.Math.Log10(f));
        return System.Math.Pow(10, exp) == f;
    }
}
