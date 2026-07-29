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
            if (na.MemberName == "DisplayFactor") return Convert.ToDouble(na.TypedValue.Value);
        return 1.0;
    }

    // FromUnit(double) static factories, keyed by unit name (without the "From" prefix).
    public static Dictionary<string, MethodInfo> Froms(Type t) =>
        t.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name.StartsWith("From") && m.Name != "FromCanonical"
                        && m.ReturnType == t
                        && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(double))
            .ToDictionary(m => m.Name[4..], m => m);

    // ToUnit() instance readers returning double, keyed by unit name (without the "To" prefix).
    public static Dictionary<string, MethodInfo> Tos(Type t) =>
        t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.StartsWith("To") && m.Name != "ToString"
                        && m.ReturnType == typeof(double) && m.GetParameters().Length == 0)
            .ToDictionary(m => m.Name[2..], m => m);

    public static object FromCanonical(Type t, double v) =>
        t.GetMethod("FromCanonical", BindingFlags.Public | BindingFlags.Static).Invoke(null, [v]);

    public static double Canonical(object instance) =>
        (double)instance.GetType().GetProperty("CanonicalValue").GetValue(instance);
}
