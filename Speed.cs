namespace com.hafthor.Measurement;

public class Speed {
    private readonly double metersPerSecond;

    private Speed(double metersPerSecond) => this.metersPerSecond = metersPerSecond;

    // Arithmetic
    public static Speed operator +(Speed a, Speed b) => new Speed(a.metersPerSecond + b.metersPerSecond);
    public static Speed operator -(Speed a, Speed b) => new Speed(a.metersPerSecond - b.metersPerSecond);
    public static Speed operator -(Speed x) => new Speed(-x.metersPerSecond);

    // SI units
    public static Speed FromMetersPerSecond(double metersPerSecond) => new Speed(metersPerSecond);
    public double ToMetersPerSecond() => metersPerSecond;
    public static Speed FromKilometersPerHour(double kilometersPerHour) => new Speed(kilometersPerHour / 3.6);
    public double ToKilometersPerHour() => metersPerSecond * 3.6;

    // Imperial / US units
    public static Speed FromMilesPerHour(double milesPerHour) => new Speed(milesPerHour * 0.44704);
    public double ToMilesPerHour() => metersPerSecond / 0.44704;
    public static Speed FromFeetPerSecond(double feetPerSecond) => new Speed(feetPerSecond * 0.3048);
    public double ToFeetPerSecond() => metersPerSecond / 0.3048;

    // Nautical units
    public static Speed FromKnots(double knots) => new Speed(knots * 0.514444444444);
    public double ToKnots() => metersPerSecond / 0.514444444444;

    // Physical references
    public static Speed FromMach(double mach) => new Speed(mach * 340.29);
    public double ToMach() => metersPerSecond / 340.29;
    public static Speed FromSpeedOfLight(double speedOfLight) => new Speed(speedOfLight * 299792458);
    public double ToSpeedOfLight() => metersPerSecond / 299792458;

    // Composite relationships
    public static Length operator *(Speed speed, Duration duration) => Length.FromMeters(speed.metersPerSecond * duration.ToSeconds());
    public static Acceleration operator /(Speed speed, Duration duration) => Acceleration.FromMetersPerSecondSquared(speed.metersPerSecond / duration.ToSeconds());
    public static Power operator *(Speed speed, Force force) => Power.FromWatts(speed.metersPerSecond * force.ToNewtons());

    // Composite relationships (derived)
    public static Momentum operator *(Speed speed, Mass mass) => Momentum.FromKilogramMetersPerSecond(speed.ToMetersPerSecond() * mass.ToKilograms());

    // Famous relations
    public static Frequency operator /(Speed speed, Length length) => Frequency.FromHertz(speed.ToMetersPerSecond() / length.ToMeters());
    public static Length operator /(Speed speed, Frequency frequency) => Length.FromMeters(speed.ToMetersPerSecond() / frequency.ToHertz());
}
