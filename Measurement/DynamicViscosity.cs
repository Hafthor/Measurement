namespace com.hafthor.Measurement;

[Measurement("Pa·s", VariableName = "millipascalSeconds", DisplayFactor = 1e3)]
[SiUnit("PascalSeconds", 3, "None Milli")]
[SiUnit("Poise", 2, "None Centi")]
public readonly partial struct DynamicViscosity {
    // Composite relationships
    public static Pressure operator /(DynamicViscosity dynamicViscosity, Duration duration) => Pressure.FromPascals(dynamicViscosity.ToPascalSeconds() / duration.ToSeconds());
    public static Duration operator /(DynamicViscosity dynamicViscosity, Pressure pressure) => Duration.FromSeconds(dynamicViscosity.ToPascalSeconds() / pressure.ToPascals());
}
