namespace com.hafthor.Measurement;

[Measurement("N", VariableName = "newtons")]
[SiUnit("Newtons", 0, "None Mega Kilo Milli")]
[SiUnit("Dynes", -5)]
[Unit("KilogramsForce", 9.80665)]
[Unit("PoundsForce", 4.4482216152605)]
[Unit("OuncesForce", 0.27801385095378125)]
[Unit("Poundals", 0.138254954376)]
public readonly partial struct Force {
    // Composite relationships
    public static Acceleration operator /(Force force, Mass mass) => Acceleration.FromMetersPerSecondSquared(force.newtons / mass.ToKilograms());
    public static Mass operator /(Force force, Acceleration acceleration) => Mass.FromKilograms(force.newtons / acceleration.ToMetersPerSecondSquared());
    public static Energy operator *(Force force, Length length) => Energy.FromJoules(force.newtons * length.ToMeters());
    public static Pressure operator /(Force force, Area area) => Pressure.FromPascals(force.newtons / area.ToSquareMeters());
    public static Power operator *(Force force, Speed speed) => Power.FromWatts(force.newtons * speed.ToMetersPerSecond());

    // Composite relationships (derived)
    public static SurfaceTension operator /(Force force, Length length) => SurfaceTension.FromNewtonsPerMeter(force.newtons / length.ToMeters());

    // Famous relations
    public static Momentum operator *(Force force, Duration duration) => Momentum.FromKilogramMetersPerSecond(force.newtons * duration.ToSeconds());
}
