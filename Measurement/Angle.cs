namespace com.hafthor.Measurement;

[Measurement("rad", VariableName = "arcseconds", DisplayFactor = 648000 / Math.PI)]
[Unit("Radians", 648000 / Math.PI)]
[Unit("Turns", 1296000)]
[Unit("Degrees", 3600)]
[Unit("Gradians", 3240)]
[Unit("Arcminutes", 60)]
[SiUnit("Arcseconds", 0)]
public readonly partial struct Angle {
    // Composite relationships (derived)
    public static AngularVelocity operator /(Angle angle, Duration duration) => AngularVelocity.FromRadiansPerSecond(angle.ToRadians() / duration.ToSeconds());
}
