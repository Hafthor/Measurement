namespace com.hafthor.Measurement;

public sealed class KinematicViscosity : Measurement<KinematicViscosity> {

    private KinematicViscosity(double value) : base(value) { }

    protected override KinematicViscosity Create(double value) => new(value);
    protected override string Symbol => "m²/s";

    // Units
    public static KinematicViscosity FromSquareMetersPerSecond(double value) => new(value);
    public double ToSquareMetersPerSecond() => value;
    public static KinematicViscosity FromStokes(double stokes) => new(stokes * (1e-4));
    public double ToStokes() => value / (1e-4);
    public static KinematicViscosity FromCentistokes(double centistokes) => new(centistokes * (1e-6));
    public double ToCentistokes() => value / (1e-6);

    // Composite relationships
    public static Area operator *(KinematicViscosity kinematicViscosity, Duration duration) => Area.FromSquareMeters(kinematicViscosity.ToSquareMetersPerSecond() * duration.ToSeconds());
    public static Area operator *(Duration duration, KinematicViscosity kinematicViscosity) => Area.FromSquareMeters(duration.ToSeconds() * kinematicViscosity.ToSquareMetersPerSecond());

}
