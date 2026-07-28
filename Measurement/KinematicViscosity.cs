namespace com.hafthor.Measurement;

[Measurement("m²/s", VariableName = "centistokes", DisplayFactor = 1e6)]
public readonly partial struct KinematicViscosity {
    // Units
    public static KinematicViscosity FromSquareMetersPerSecond(double squareMetersPerSecond) => new(squareMetersPerSecond * 1e6);
    public double ToSquareMetersPerSecond() => centistokes / 1e6;
    public static KinematicViscosity FromStokes(double stokes) => new(stokes * (1e2));
    public double ToStokes() => centistokes / (1e2);
    public static KinematicViscosity FromCentistokes(double centistokes) => new(centistokes);
    public double ToCentistokes() => centistokes;

    // Composite relationships
    public static Area operator *(KinematicViscosity kinematicViscosity, Duration duration) => Area.FromSquareMeters(kinematicViscosity.ToSquareMetersPerSecond() * duration.ToSeconds());
    public static Area operator *(Duration duration, KinematicViscosity kinematicViscosity) => Area.FromSquareMeters(duration.ToSeconds() * kinematicViscosity.ToSquareMetersPerSecond());
}
