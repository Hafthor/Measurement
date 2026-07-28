namespace com.hafthor.Measurement;

[Measurement("Pa·s", VariableName = "millipascalSeconds", DisplayFactor = 1e3)]
public readonly partial struct DynamicViscosity {
    // Units
    public static DynamicViscosity FromPascalSeconds(double pascalSeconds) => new(pascalSeconds * 1e3);
    public double ToPascalSeconds() => millipascalSeconds / 1e3;
    public static DynamicViscosity FromMillipascalSeconds(double millipascalSeconds) => new(millipascalSeconds);
    public double ToMillipascalSeconds() => millipascalSeconds;
    public static DynamicViscosity FromPoise(double poise) => new(poise * 1e2);
    public double ToPoise() => millipascalSeconds / 1e2;
    public static DynamicViscosity FromCentipoise(double centipoise) => new(centipoise);
    public double ToCentipoise() => millipascalSeconds;

    // Composite relationships
    public static Pressure operator /(DynamicViscosity dynamicViscosity, Duration duration) => Pressure.FromPascals(dynamicViscosity.ToPascalSeconds() / duration.ToSeconds());
    public static Duration operator /(DynamicViscosity dynamicViscosity, Pressure pressure) => Duration.FromSeconds(dynamicViscosity.ToPascalSeconds() / pressure.ToPascals());
}
