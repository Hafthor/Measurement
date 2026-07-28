namespace com.hafthor.Measurement;

[Measurement("rad/s", VariableName = "degreesPerSecond", DisplayFactor = 180 / Math.PI)]
public readonly partial struct AngularVelocity {
    // Units
    public static AngularVelocity FromRadiansPerSecond(double radiansPerSecond) => new(radiansPerSecond * 180 / Math.PI);
    public double ToRadiansPerSecond() => degreesPerSecond * Math.PI / 180;
    public static AngularVelocity FromDegreesPerSecond(double degreesPerSecond) => new(degreesPerSecond);
    public double ToDegreesPerSecond() => degreesPerSecond;
    public static AngularVelocity FromRevolutionsPerSecond(double revolutionsPerSecond) => new(revolutionsPerSecond * 360);
    public double ToRevolutionsPerSecond() => degreesPerSecond / 360;
    public static AngularVelocity FromRevolutionsPerMinute(double revolutionsPerMinute) => new(revolutionsPerMinute * 6);
    public double ToRevolutionsPerMinute() => degreesPerSecond / 6;

    // Composite relationships
    public static Angle operator *(AngularVelocity angularVelocity, Duration duration) => Angle.FromRadians(angularVelocity.ToRadiansPerSecond() * duration.ToSeconds());
    public static Angle operator *(Duration duration, AngularVelocity angularVelocity) => Angle.FromRadians(duration.ToSeconds() * angularVelocity.ToRadiansPerSecond());
    public static AngularAcceleration operator /(AngularVelocity angularVelocity, Duration duration) => AngularAcceleration.FromRadiansPerSecondSquared(angularVelocity.ToRadiansPerSecond() / duration.ToSeconds());
    public static AngularMomentum operator *(AngularVelocity angularVelocity, MomentOfInertia momentOfInertia) => AngularMomentum.FromKilogramSquareMetersPerSecond(angularVelocity.ToRadiansPerSecond() * momentOfInertia.ToKilogramSquareMeters());
}
