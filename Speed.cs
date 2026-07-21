namespace com.hafthor.Measurement;

public sealed class Speed : Measurement<Speed> {

    private Speed(double value) : base(value) { }

    protected override Speed Create(double value) => new(value);
    protected override string Symbol => "m/s";

    // SI units
    public static Speed FromMetersPerSecond(double value) => new(value);
    public double ToMetersPerSecond() => value;
    public static Speed FromKilometersPerHour(double kilometersPerHour) => new(kilometersPerHour / 3.6);
    public double ToKilometersPerHour() => value * 3.6;

    // Imperial / US units
    public static Speed FromMilesPerHour(double milesPerHour) => new(milesPerHour * 0.44704);
    public double ToMilesPerHour() => value / 0.44704;
    public static Speed FromFeetPerSecond(double feetPerSecond) => new(feetPerSecond * 0.3048);
    public double ToFeetPerSecond() => value / 0.3048;

    // Nautical units
    public static Speed FromKnots(double knots) => new(knots * 0.514444444444);
    public double ToKnots() => value / 0.514444444444;

    // Physical references
    public static Speed FromMach(double mach) => new(mach * 340.29);
    public double ToMach() => value / 340.29;
    public static Speed FromSpeedOfLight(double speedOfLight) => new(speedOfLight * 299792458);
    public double ToSpeedOfLight() => value / 299792458;

    // Composite relationships
    public static Length operator *(Speed speed, Duration duration) => Length.FromMeters(speed.value * duration.ToSeconds());
    public static Acceleration operator /(Speed speed, Duration duration) => Acceleration.FromMetersPerSecondSquared(speed.value / duration.ToSeconds());
    public static Power operator *(Speed speed, Force force) => Power.FromWatts(speed.value * force.ToNewtons());

    // Composite relationships (derived)
    public static Momentum operator *(Speed speed, Mass mass) => Momentum.FromKilogramMetersPerSecond(speed.ToMetersPerSecond() * mass.ToKilograms());

    // Famous relations
    public static Frequency operator /(Speed speed, Length length) => Frequency.FromHertz(speed.ToMetersPerSecond() / length.ToMeters());
    public static Length operator /(Speed speed, Frequency frequency) => Length.FromMeters(speed.ToMetersPerSecond() / frequency.ToHertz());

}
