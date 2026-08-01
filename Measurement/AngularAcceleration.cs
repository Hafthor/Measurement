namespace com.hafthor.Measurement;

[Measurement("rad/s²", VariableName = "degreePerSecondSquared", DisplayFactor = 180 / Math.PI)]
[SiUnit("DegreesPerSecondSquared", 0)]
[Unit("RadiansPerSecondSquared", 180 / Math.PI)]
[Unit("RevolutionsPerMinutePerSecond", 6)]
public readonly partial struct AngularAcceleration {
    public static AngularVelocity operator *(AngularAcceleration angularAcceleration, Duration duration) => AngularVelocity.FromRadiansPerSecond(angularAcceleration.ToRadiansPerSecondSquared() * duration.ToSeconds());
    public static AngularVelocity operator *(Duration duration, AngularAcceleration angularAcceleration) => AngularVelocity.FromRadiansPerSecond(duration.ToSeconds() * angularAcceleration.ToRadiansPerSecondSquared());
}
