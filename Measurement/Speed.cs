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
[Product<Acceleration, Duration>]
[Product<Length, Frequency>]
public readonly partial struct Speed { }
