namespace com.hafthor.Measurement;

public class AngularAcceleration {
    private readonly double radiansPerSecondSquared;

    private AngularAcceleration(double radiansPerSecondSquared) => this.radiansPerSecondSquared = radiansPerSecondSquared;

    // Arithmetic
    public static AngularAcceleration operator +(AngularAcceleration a, AngularAcceleration b) => new(a.radiansPerSecondSquared + b.radiansPerSecondSquared);
    public static AngularAcceleration operator -(AngularAcceleration a, AngularAcceleration b) => new(a.radiansPerSecondSquared - b.radiansPerSecondSquared);
    public static AngularAcceleration operator -(AngularAcceleration x) => new(-x.radiansPerSecondSquared);

    // Units
    public static AngularAcceleration FromRadiansPerSecondSquared(double radiansPerSecondSquared) => new(radiansPerSecondSquared);
    public double ToRadiansPerSecondSquared() => radiansPerSecondSquared;
    public static AngularAcceleration FromDegreesPerSecondSquared(double degreesPerSecondSquared) => new(degreesPerSecondSquared * (Math.PI / 180));
    public double ToDegreesPerSecondSquared() => radiansPerSecondSquared / (Math.PI / 180);
    public static AngularAcceleration FromRevolutionsPerMinutePerSecond(double revolutionsPerMinutePerSecond) => new(revolutionsPerMinutePerSecond * (2 * Math.PI / 60));
    public double ToRevolutionsPerMinutePerSecond() => radiansPerSecondSquared / (2 * Math.PI / 60);

    // Composite relationships
    public static AngularVelocity operator *(AngularAcceleration angularAcceleration, Duration duration) => AngularVelocity.FromRadiansPerSecond(angularAcceleration.ToRadiansPerSecondSquared() * duration.ToSeconds());
    public static AngularVelocity operator *(Duration duration, AngularAcceleration angularAcceleration) => AngularVelocity.FromRadiansPerSecond(duration.ToSeconds() * angularAcceleration.ToRadiansPerSecondSquared());

    public override string ToString() => $"{radiansPerSecondSquared} rad/s²";

    public override bool Equals(object obj) => obj is AngularAcceleration other && other.radiansPerSecondSquared == radiansPerSecondSquared;
    public override int GetHashCode() => radiansPerSecondSquared.GetHashCode();
}
