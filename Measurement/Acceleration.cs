namespace com.hafthor.Measurement;

[Measurement("m/s²", VariableName = "metersPerSecondSquared")]
[SiUnit("MetersPerSecondSquared", 0)]
[Unit("KilometersPerHourPerSecond", 1.0/(3.6))]
[Unit("FeetPerSecondSquared", 0.3048)]
[Unit("StandardGravity", 9.80665)]
[SiUnit("Gals", -2)]
[Product<Duration, Jerk>]
public readonly partial struct Acceleration { }
