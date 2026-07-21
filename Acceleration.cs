namespace com.hafthor.Measurement;

public sealed class Acceleration : Measurement<Acceleration> {

    private Acceleration(double value) : base(value) { }

    protected override Acceleration Create(double value) => new(value);
    protected override string Symbol => "m/s²";

    // SI units
    public static Acceleration FromMetersPerSecondSquared(double value) => new(value);
    public double ToMetersPerSecondSquared() => value;
    public static Acceleration FromKilometersPerHourPerSecond(double kilometersPerHourPerSecond) => new(kilometersPerHourPerSecond / 3.6);
    public double ToKilometersPerHourPerSecond() => value * 3.6;

    // Imperial / US units
    public static Acceleration FromFeetPerSecondSquared(double feetPerSecondSquared) => new(feetPerSecondSquared * 0.3048);
    public double ToFeetPerSecondSquared() => value / 0.3048;

    // Physical references
    public static Acceleration FromStandardGravity(double standardGravity) => new(standardGravity * 9.80665);
    public double ToStandardGravity() => value / 9.80665;
    public static Acceleration FromGals(double gals) => new(gals * 1e-2);
    public double ToGals() => value / 1e-2;

    // Composite relationships
    public static Speed operator *(Acceleration acceleration, Duration duration) => Speed.FromMetersPerSecond(acceleration.value * duration.ToSeconds());
    public static Force operator *(Acceleration acceleration, Mass mass) => Force.FromNewtons(acceleration.value * mass.ToKilograms());

    // Composite relationships (derived)
    public static Jerk operator /(Acceleration acceleration, Duration duration) => Jerk.FromMetersPerSecondCubed(acceleration.ToMetersPerSecondSquared() / duration.ToSeconds());

}
