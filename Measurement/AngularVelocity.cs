namespace com.hafthor.Measurement;

[Measurement("rad/s", VariableName = "degreesPerSecond", DisplayFactor = 180 / Math.PI)]
[SiUnit("DegreesPerSecond", 0)]
[Unit("RadiansPerSecond", 180 / Math.PI)]
[Unit("RevolutionsPerSecond", 360)]
[Unit("RevolutionsPerMinute", 6)]
[Product<AngularAcceleration, Duration>]
public readonly partial struct AngularVelocity;
