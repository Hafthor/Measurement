namespace com.hafthor.Measurement;

public class DynamicViscosity {
    private readonly double pascalSeconds;

    private DynamicViscosity(double pascalSeconds) => this.pascalSeconds = pascalSeconds;

    // Arithmetic
    public static DynamicViscosity operator +(DynamicViscosity a, DynamicViscosity b) => new(a.pascalSeconds + b.pascalSeconds);
    public static DynamicViscosity operator -(DynamicViscosity a, DynamicViscosity b) => new(a.pascalSeconds - b.pascalSeconds);
    public static DynamicViscosity operator -(DynamicViscosity x) => new(-x.pascalSeconds);

    // Units
    public static DynamicViscosity FromPascalSeconds(double pascalSeconds) => new(pascalSeconds);
    public double ToPascalSeconds() => pascalSeconds;
    public static DynamicViscosity FromMillipascalSeconds(double millipascalSeconds) => new(millipascalSeconds * (1e-3));
    public double ToMillipascalSeconds() => pascalSeconds / (1e-3);
    public static DynamicViscosity FromPoise(double poise) => new(poise * (0.1));
    public double ToPoise() => pascalSeconds / (0.1);
    public static DynamicViscosity FromCentipoise(double centipoise) => new(centipoise * (1e-3));
    public double ToCentipoise() => pascalSeconds / (1e-3);

    // Composite relationships
    public static Pressure operator /(DynamicViscosity dynamicViscosity, Duration duration) => Pressure.FromPascals(dynamicViscosity.ToPascalSeconds() / duration.ToSeconds());
    public static Duration operator /(DynamicViscosity dynamicViscosity, Pressure pressure) => Duration.FromSeconds(dynamicViscosity.ToPascalSeconds() / pressure.ToPascals());

    public override string ToString() => $"{pascalSeconds} Pa·s";

    public override bool Equals(object obj) => obj is DynamicViscosity other && other.pascalSeconds == pascalSeconds;
    public override int GetHashCode() => pascalSeconds.GetHashCode();
}
