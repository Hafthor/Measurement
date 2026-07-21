namespace com.hafthor.Measurement;

[Measurement("rad")]
public readonly partial struct Angle {

    public static Angle FromRadians(double radians) => new(radians);
    public double ToRadians() => value;
    public static Angle FromMilliradians(double milliradians) => new(milliradians * 1e-3);
    public double ToMilliradians() => value / 1e-3;
    public static Angle FromTurns(double turns) => new(turns * 2 * Math.PI);
    public double ToTurns() => value / (2 * Math.PI);
    public static Angle FromDegrees(double degrees) => new(degrees * Math.PI / 180);
    public double ToDegrees() => value * 180 / Math.PI;
    public static Angle FromGradians(double gradians) => new(gradians * Math.PI / 200);
    public double ToGradians() => value * 200 / Math.PI;
    public static Angle FromArcminutes(double arcminutes) => new(arcminutes * Math.PI / 10800);
    public double ToArcminutes() => value * 10800 / Math.PI;
    public static Angle FromArcseconds(double arcseconds) => new(arcseconds * Math.PI / 648000);
    public double ToArcseconds() => value * 648000 / Math.PI;

    // Composite relationships (derived)
    public static AngularVelocity operator /(Angle angle, Duration duration) => AngularVelocity.FromRadiansPerSecond(angle.ToRadians() / duration.ToSeconds());

}
