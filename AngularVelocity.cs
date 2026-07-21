namespace com.hafthor.Measurement;

public sealed class AngularVelocity : Measurement<AngularVelocity> {

    private AngularVelocity(double value) : base(value) { }

    protected override AngularVelocity Create(double value) => new(value);
    protected override string Symbol => "rad/s";

    // Units
    public static AngularVelocity FromRadiansPerSecond(double value) => new(value);
    public double ToRadiansPerSecond() => value;
    public static AngularVelocity FromDegreesPerSecond(double degreesPerSecond) => new(degreesPerSecond * (Math.PI / 180));
    public double ToDegreesPerSecond() => value / (Math.PI / 180);
    public static AngularVelocity FromRevolutionsPerSecond(double revolutionsPerSecond) => new(revolutionsPerSecond * (2 * Math.PI));
    public double ToRevolutionsPerSecond() => value / (2 * Math.PI);
    public static AngularVelocity FromRevolutionsPerMinute(double revolutionsPerMinute) => new(revolutionsPerMinute * (2 * Math.PI / 60));
    public double ToRevolutionsPerMinute() => value / (2 * Math.PI / 60);

    // Composite relationships
    public static Angle operator *(AngularVelocity angularVelocity, Duration duration) => Angle.FromRadians(angularVelocity.ToRadiansPerSecond() * duration.ToSeconds());
    public static Angle operator *(Duration duration, AngularVelocity angularVelocity) => Angle.FromRadians(duration.ToSeconds() * angularVelocity.ToRadiansPerSecond());
    public static AngularAcceleration operator /(AngularVelocity angularVelocity, Duration duration) => AngularAcceleration.FromRadiansPerSecondSquared(angularVelocity.ToRadiansPerSecond() / duration.ToSeconds());
    public static AngularMomentum operator *(AngularVelocity angularVelocity, MomentOfInertia momentOfInertia) => AngularMomentum.FromKilogramSquareMetersPerSecond(angularVelocity.ToRadiansPerSecond() * momentOfInertia.ToKilogramSquareMeters());

}
