namespace com.hafthor.Measurement;

// Canonical unit is metres/second. Curated compound units (km/h, mph, ft/s, knots, …) pair a
// numerator and a non-SI denominator (e.g. Hour = 3600 s), so they aren't a power-of-ten prefix
// grid — each is declared as an explicit [Unit] with its factor (m/s per unit).
[Measurement("m/s", VariableName = "metersPerSecond")]
[SiUnit("MetersPerSecond", 0)]
[Unit("KilometersPerHour", 1.0 / 3.6)]
[Unit("MilesPerHour", 0.44704)]
[Unit("FeetPerSecond", 0.3048)]
[Unit("Knots", 0.514444444444)]
[Unit("Mach", 340.29)]
[Unit("SpeedOfLight", 299792458)]
public readonly partial struct Speed {
    // Composite relationships
    public static Length operator *(Speed speed, Duration duration) => Length.FromMeters(speed.ToMetersPerSecond() * duration.ToSeconds());
    public static Acceleration operator /(Speed speed, Duration duration) => Acceleration.FromMetersPerSecondSquared(speed.ToMetersPerSecond() / duration.ToSeconds());
    public static Power operator *(Speed speed, Force force) => Power.FromWatts(speed.ToMetersPerSecond() * force.ToNewtons());

    // Composite relationships (derived)
    public static Momentum operator *(Speed speed, Mass mass) => Momentum.FromKilogramMetersPerSecond(speed.ToMetersPerSecond() * mass.ToKilograms());

    // Famous relations
    public static Frequency operator /(Speed speed, Length length) => Frequency.FromHertz(speed.ToMetersPerSecond() / length.ToMeters());
    public static Length operator /(Speed speed, Frequency frequency) => Length.FromMeters(speed.ToMetersPerSecond() / frequency.ToHertz());
}
