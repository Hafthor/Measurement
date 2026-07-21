namespace com.hafthor.Measurement;

public class AngularVelocity {
    private readonly double radiansPerSecond;

    private AngularVelocity(double radiansPerSecond) => this.radiansPerSecond = radiansPerSecond;

    // Arithmetic
    public static AngularVelocity operator +(AngularVelocity a, AngularVelocity b) => new(a.radiansPerSecond + b.radiansPerSecond);
    public static AngularVelocity operator -(AngularVelocity a, AngularVelocity b) => new(a.radiansPerSecond - b.radiansPerSecond);
    public static AngularVelocity operator -(AngularVelocity x) => new(-x.radiansPerSecond);

    // Units
    public static AngularVelocity FromRadiansPerSecond(double radiansPerSecond) => new(radiansPerSecond);
    public double ToRadiansPerSecond() => radiansPerSecond;
    public static AngularVelocity FromDegreesPerSecond(double degreesPerSecond) => new(degreesPerSecond * (Math.PI / 180));
    public double ToDegreesPerSecond() => radiansPerSecond / (Math.PI / 180);
    public static AngularVelocity FromRevolutionsPerSecond(double revolutionsPerSecond) => new(revolutionsPerSecond * (2 * Math.PI));
    public double ToRevolutionsPerSecond() => radiansPerSecond / (2 * Math.PI);
    public static AngularVelocity FromRevolutionsPerMinute(double revolutionsPerMinute) => new(revolutionsPerMinute * (2 * Math.PI / 60));
    public double ToRevolutionsPerMinute() => radiansPerSecond / (2 * Math.PI / 60);

    // Composite relationships
    public static Angle operator *(AngularVelocity angularVelocity, Duration duration) => Angle.FromRadians(angularVelocity.ToRadiansPerSecond() * duration.ToSeconds());
    public static Angle operator *(Duration duration, AngularVelocity angularVelocity) => Angle.FromRadians(duration.ToSeconds() * angularVelocity.ToRadiansPerSecond());
    public static AngularAcceleration operator /(AngularVelocity angularVelocity, Duration duration) => AngularAcceleration.FromRadiansPerSecondSquared(angularVelocity.ToRadiansPerSecond() / duration.ToSeconds());
    public static AngularMomentum operator *(AngularVelocity angularVelocity, MomentOfInertia momentOfInertia) => AngularMomentum.FromKilogramSquareMetersPerSecond(angularVelocity.ToRadiansPerSecond() * momentOfInertia.ToKilogramSquareMeters());

    public override string ToString() => $"{radiansPerSecond} rad/s";

    public override bool Equals(object obj) => obj is AngularVelocity other && other.radiansPerSecond == radiansPerSecond;
    public override int GetHashCode() => radiansPerSecond.GetHashCode();
}
