namespace com.hafthor.Measurement;

public sealed class AngularAcceleration : Measurement<AngularAcceleration> {

    private AngularAcceleration(double value) : base(value) { }

    protected override AngularAcceleration Create(double value) => new(value);
    protected override string Symbol => "rad/s²";

    // Units
    public static AngularAcceleration FromRadiansPerSecondSquared(double value) => new(value);
    public double ToRadiansPerSecondSquared() => value;
    public static AngularAcceleration FromDegreesPerSecondSquared(double degreesPerSecondSquared) => new(degreesPerSecondSquared * (Math.PI / 180));
    public double ToDegreesPerSecondSquared() => value / (Math.PI / 180);
    public static AngularAcceleration FromRevolutionsPerMinutePerSecond(double revolutionsPerMinutePerSecond) => new(revolutionsPerMinutePerSecond * (2 * Math.PI / 60));
    public double ToRevolutionsPerMinutePerSecond() => value / (2 * Math.PI / 60);

    // Composite relationships
    public static AngularVelocity operator *(AngularAcceleration angularAcceleration, Duration duration) => AngularVelocity.FromRadiansPerSecond(angularAcceleration.ToRadiansPerSecondSquared() * duration.ToSeconds());
    public static AngularVelocity operator *(Duration duration, AngularAcceleration angularAcceleration) => AngularVelocity.FromRadiansPerSecond(duration.ToSeconds() * angularAcceleration.ToRadiansPerSecondSquared());

}
