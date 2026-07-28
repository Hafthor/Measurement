namespace com.hafthor.Measurement;

[Measurement("rad/s²", VariableName = "degreePerSecondSquared", DisplayFactor = 180 / Math.PI)]
public readonly partial struct AngularAcceleration {
    // Units
    public static AngularAcceleration FromRadiansPerSecondSquared(double radiansPerSecondSquared) => new(radiansPerSecondSquared / Math.PI * 180);
    public double ToRadiansPerSecondSquared() => degreePerSecondSquared * Math.PI / 180;
    public static AngularAcceleration FromDegreesPerSecondSquared(double degreesPerSecondSquared) => new(degreesPerSecondSquared);
    public double ToDegreesPerSecondSquared() => degreePerSecondSquared;
    public static AngularAcceleration FromRevolutionsPerMinutePerSecond(double revolutionsPerMinutePerSecond) => new(revolutionsPerMinutePerSecond * 360);
    public double ToRevolutionsPerMinutePerSecond() => degreePerSecondSquared / 360;

    // Composite relationships
    public static AngularVelocity operator *(AngularAcceleration angularAcceleration, Duration duration) => AngularVelocity.FromRadiansPerSecond(angularAcceleration.ToRadiansPerSecondSquared() * duration.ToSeconds());
    public static AngularVelocity operator *(Duration duration, AngularAcceleration angularAcceleration) => AngularVelocity.FromRadiansPerSecond(duration.ToSeconds() * angularAcceleration.ToRadiansPerSecondSquared());
}
