namespace com.hafthor.Measurement;

[Measurement("Pa·s")]
public readonly partial struct DynamicViscosity {

    // Units
    public static DynamicViscosity FromPascalSeconds(double pascalSeconds) => new(pascalSeconds);
    public double ToPascalSeconds() => value;
    public static DynamicViscosity FromMillipascalSeconds(double millipascalSeconds) => new(millipascalSeconds * (1e-3));
    public double ToMillipascalSeconds() => value / (1e-3);
    public static DynamicViscosity FromPoise(double poise) => new(poise * (0.1));
    public double ToPoise() => value / (0.1);
    public static DynamicViscosity FromCentipoise(double centipoise) => new(centipoise * (1e-3));
    public double ToCentipoise() => value / (1e-3);

    // Composite relationships
    public static Pressure operator /(DynamicViscosity dynamicViscosity, Duration duration) => Pressure.FromPascals(dynamicViscosity.ToPascalSeconds() / duration.ToSeconds());
    public static Duration operator /(DynamicViscosity dynamicViscosity, Pressure pressure) => Duration.FromSeconds(dynamicViscosity.ToPascalSeconds() / pressure.ToPascals());

}
