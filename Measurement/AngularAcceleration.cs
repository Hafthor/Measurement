namespace com.hafthor.Measurement;

[Measurement("rad/s²", VariableName = "degreePerSecondSquared", DisplayFactor = 180 / Math.PI)]
[SiUnit("DegreesPerSecondSquared", 0)]
[Unit("RadiansPerSecondSquared", 180 / Math.PI)]
[Unit("RevolutionsPerMinutePerSecond", 6)]
public readonly partial struct AngularAcceleration;
