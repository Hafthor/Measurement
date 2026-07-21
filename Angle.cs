namespace com.hafthor.Measurement;

public class Angle {
    private readonly double radians;

    private Angle(double radians) => this.radians = radians;

    // Arithmetic
    public static Angle operator +(Angle a, Angle b) => new Angle(a.radians + b.radians);
    public static Angle operator -(Angle a, Angle b) => new Angle(a.radians - b.radians);
    public static Angle operator -(Angle x) => new Angle(-x.radians);

    public static Angle FromRadians(double radians) => new Angle(radians);
    public double ToRadians() => radians;
    public static Angle FromMilliradians(double milliradians) => new Angle(milliradians * 1e-3);
    public double ToMilliradians() => radians / 1e-3;
    public static Angle FromTurns(double turns) => new Angle(turns * 2 * Math.PI);
    public double ToTurns() => radians / (2 * Math.PI);
    public static Angle FromDegrees(double degrees) => new Angle(degrees * Math.PI / 180);
    public double ToDegrees() => radians * 180 / Math.PI;
    public static Angle FromGradians(double gradians) => new Angle(gradians * Math.PI / 200);
    public double ToGradians() => radians * 200 / Math.PI;
    public static Angle FromArcminutes(double arcminutes) => new Angle(arcminutes * Math.PI / 10800);
    public double ToArcminutes() => radians * 10800 / Math.PI;
    public static Angle FromArcseconds(double arcseconds) => new Angle(arcseconds * Math.PI / 648000);
    public double ToArcseconds() => radians * 648000 / Math.PI;

    // Composite relationships (derived)
    public static AngularVelocity operator /(Angle angle, Duration duration) => AngularVelocity.FromRadiansPerSecond(angle.ToRadians() / duration.ToSeconds());
}
