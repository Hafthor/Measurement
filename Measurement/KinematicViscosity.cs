namespace com.hafthor.Measurement;

[Measurement("m²/s")]
public readonly partial struct KinematicViscosity {

    // Units
    public static KinematicViscosity FromSquareMetersPerSecond(double squareMetersPerSecond) => new(squareMetersPerSecond);
    public double ToSquareMetersPerSecond() => value;
    public static KinematicViscosity FromStokes(double stokes) => new(stokes * (1e-4));
    public double ToStokes() => value / (1e-4);
    public static KinematicViscosity FromCentistokes(double centistokes) => new(centistokes * (1e-6));
    public double ToCentistokes() => value / (1e-6);

    // Composite relationships
    public static Area operator *(KinematicViscosity kinematicViscosity, Duration duration) => Area.FromSquareMeters(kinematicViscosity.ToSquareMetersPerSecond() * duration.ToSeconds());
    public static Area operator *(Duration duration, KinematicViscosity kinematicViscosity) => Area.FromSquareMeters(duration.ToSeconds() * kinematicViscosity.ToSquareMetersPerSecond());

}
