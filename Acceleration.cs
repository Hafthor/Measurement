namespace com.hafthor.Measurement;

public class Acceleration {
    private readonly double metersPerSecondSquared;

    private Acceleration(double metersPerSecondSquared) => this.metersPerSecondSquared = metersPerSecondSquared;

    // Arithmetic
    public static Acceleration operator +(Acceleration a, Acceleration b) => new(a.metersPerSecondSquared + b.metersPerSecondSquared);
    public static Acceleration operator -(Acceleration a, Acceleration b) => new(a.metersPerSecondSquared - b.metersPerSecondSquared);
    public static Acceleration operator -(Acceleration x) => new(-x.metersPerSecondSquared);

    // SI units
    public static Acceleration FromMetersPerSecondSquared(double metersPerSecondSquared) => new(metersPerSecondSquared);
    public double ToMetersPerSecondSquared() => metersPerSecondSquared;
    public static Acceleration FromKilometersPerHourPerSecond(double kilometersPerHourPerSecond) => new(kilometersPerHourPerSecond / 3.6);
    public double ToKilometersPerHourPerSecond() => metersPerSecondSquared * 3.6;

    // Imperial / US units
    public static Acceleration FromFeetPerSecondSquared(double feetPerSecondSquared) => new(feetPerSecondSquared * 0.3048);
    public double ToFeetPerSecondSquared() => metersPerSecondSquared / 0.3048;

    // Physical references
    public static Acceleration FromStandardGravity(double standardGravity) => new(standardGravity * 9.80665);
    public double ToStandardGravity() => metersPerSecondSquared / 9.80665;
    public static Acceleration FromGals(double gals) => new(gals * 1e-2);
    public double ToGals() => metersPerSecondSquared / 1e-2;

    // Composite relationships
    public static Speed operator *(Acceleration acceleration, Duration duration) => Speed.FromMetersPerSecond(acceleration.metersPerSecondSquared * duration.ToSeconds());
    public static Force operator *(Acceleration acceleration, Mass mass) => Force.FromNewtons(acceleration.metersPerSecondSquared * mass.ToKilograms());

    // Composite relationships (derived)
    public static Jerk operator /(Acceleration acceleration, Duration duration) => Jerk.FromMetersPerSecondCubed(acceleration.ToMetersPerSecondSquared() / duration.ToSeconds());

    public override string ToString() => $"{metersPerSecondSquared} m/s²";

    public override bool Equals(object obj) => obj is Acceleration other && other.metersPerSecondSquared == metersPerSecondSquared;
    public override int GetHashCode() => metersPerSecondSquared.GetHashCode();
}
