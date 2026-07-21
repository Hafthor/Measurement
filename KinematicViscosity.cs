namespace com.hafthor.Measurement;

public class KinematicViscosity {
    private readonly double squareMetersPerSecond;

    private KinematicViscosity(double squareMetersPerSecond) => this.squareMetersPerSecond = squareMetersPerSecond;

    // Arithmetic
    public static KinematicViscosity operator +(KinematicViscosity a, KinematicViscosity b) => new(a.squareMetersPerSecond + b.squareMetersPerSecond);
    public static KinematicViscosity operator -(KinematicViscosity a, KinematicViscosity b) => new(a.squareMetersPerSecond - b.squareMetersPerSecond);
    public static KinematicViscosity operator -(KinematicViscosity x) => new(-x.squareMetersPerSecond);

    // Units
    public static KinematicViscosity FromSquareMetersPerSecond(double squareMetersPerSecond) => new(squareMetersPerSecond);
    public double ToSquareMetersPerSecond() => squareMetersPerSecond;
    public static KinematicViscosity FromStokes(double stokes) => new(stokes * (1e-4));
    public double ToStokes() => squareMetersPerSecond / (1e-4);
    public static KinematicViscosity FromCentistokes(double centistokes) => new(centistokes * (1e-6));
    public double ToCentistokes() => squareMetersPerSecond / (1e-6);

    // Composite relationships
    public static Area operator *(KinematicViscosity kinematicViscosity, Duration duration) => Area.FromSquareMeters(kinematicViscosity.ToSquareMetersPerSecond() * duration.ToSeconds());
    public static Area operator *(Duration duration, KinematicViscosity kinematicViscosity) => Area.FromSquareMeters(duration.ToSeconds() * kinematicViscosity.ToSquareMetersPerSecond());

    public override string ToString() => $"{squareMetersPerSecond} m²/s";
}
