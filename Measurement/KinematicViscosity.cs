namespace com.hafthor.Measurement;

[Measurement("m²/s", VariableName = "centistokes", DisplayFactor = 1e6)]
[SiUnit("SquareMetersPerSecond", 6)]
[SiUnit("Stokes", 2, "None Centi")]
public readonly partial struct KinematicViscosity {
    // Composite relationships
    public static Area operator *(KinematicViscosity kinematicViscosity, Duration duration) => Area.FromSquareMeters(kinematicViscosity.ToSquareMetersPerSecond() * duration.ToSeconds());
    public static Area operator *(Duration duration, KinematicViscosity kinematicViscosity) => Area.FromSquareMeters(duration.ToSeconds() * kinematicViscosity.ToSquareMetersPerSecond());
}
