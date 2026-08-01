namespace com.hafthor.Measurement;

[Measurement("rad/s", VariableName = "degreesPerSecond", DisplayFactor = 180 / Math.PI)]
[SiUnit("DegreesPerSecond", 0)]
[Unit("RadiansPerSecond", 180 / Math.PI)]
[Unit("RevolutionsPerSecond", 360)]
[Unit("RevolutionsPerMinute", 6)]
public readonly partial struct AngularVelocity {
    // Composite relationships
    public static Angle operator *(AngularVelocity angularVelocity, Duration duration) => Angle.FromRadians(angularVelocity.ToRadiansPerSecond() * duration.ToSeconds());
    public static Angle operator *(Duration duration, AngularVelocity angularVelocity) => Angle.FromRadians(duration.ToSeconds() * angularVelocity.ToRadiansPerSecond());
    public static AngularAcceleration operator /(AngularVelocity angularVelocity, Duration duration) => AngularAcceleration.FromRadiansPerSecondSquared(angularVelocity.ToRadiansPerSecond() / duration.ToSeconds());
    public static AngularMomentum operator *(AngularVelocity angularVelocity, MomentOfInertia momentOfInertia) => AngularMomentum.FromKilogramSquareMetersPerSecond(angularVelocity.ToRadiansPerSecond() * momentOfInertia.ToKilogramSquareMeters());
}
