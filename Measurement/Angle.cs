namespace com.hafthor.Measurement;

[Measurement("rad", VariableName = "arcseconds", DisplayFactor = 648000 / Math.PI)]
public readonly partial struct Angle {
    public static Angle FromRadians(double radians) => new(radians / Math.PI * 648000);
    public double ToRadians() => arcseconds * Math.PI / 648000;
    public static Angle FromMilliradians(double milliradians) => new(milliradians / Math.PI * 648);
    public double ToMilliradians() => arcseconds / 648 * Math.PI;
    public static Angle FromTurns(double turns) => new(turns * 1296000);
    public double ToTurns() => arcseconds / 1296000;
    public static Angle FromDegrees(double degrees) => new(degrees * 3600);
    public double ToDegrees() => arcseconds / 3600;
    public static Angle FromGradians(double gradians) => new(gradians * 3240);
    public double ToGradians() => arcseconds / 3240;
    public static Angle FromArcminutes(double arcminutes) => new(arcminutes * 60);
    public double ToArcminutes() => arcseconds / 60;
    public static Angle FromArcseconds(double arcseconds) => new(arcseconds);
    public double ToArcseconds() => arcseconds;

    // Composite relationships (derived)
    public static AngularVelocity operator /(Angle angle, Duration duration) => AngularVelocity.FromRadiansPerSecond(angle.ToRadians() / duration.ToSeconds());
}
