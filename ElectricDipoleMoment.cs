namespace com.hafthor.Measurement;

public class ElectricDipoleMoment {
    private readonly double coulombMeters;

    private ElectricDipoleMoment(double coulombMeters) => this.coulombMeters = coulombMeters;

    // Arithmetic
    public static ElectricDipoleMoment operator +(ElectricDipoleMoment a, ElectricDipoleMoment b) => new(a.coulombMeters + b.coulombMeters);
    public static ElectricDipoleMoment operator -(ElectricDipoleMoment a, ElectricDipoleMoment b) => new(a.coulombMeters - b.coulombMeters);
    public static ElectricDipoleMoment operator -(ElectricDipoleMoment x) => new(-x.coulombMeters);

    // Units
    public static ElectricDipoleMoment FromCoulombMeters(double coulombMeters) => new(coulombMeters);
    public double ToCoulombMeters() => coulombMeters;
    public static ElectricDipoleMoment FromDebyes(double debyes) => new(debyes * (3.33564095198e-30));
    public double ToDebyes() => coulombMeters / (3.33564095198e-30);

    // Composite relationships
    public static ElectricCharge operator /(ElectricDipoleMoment electricDipoleMoment, Length length) => ElectricCharge.FromCoulombs(electricDipoleMoment.ToCoulombMeters() / length.ToMeters());
    public static Length operator /(ElectricDipoleMoment electricDipoleMoment, ElectricCharge electricCharge) => Length.FromMeters(electricDipoleMoment.ToCoulombMeters() / electricCharge.ToCoulombs());

    public override string ToString() => $"{coulombMeters} C·m";

    public override bool Equals(object obj) => obj is ElectricDipoleMoment other && other.coulombMeters == coulombMeters;
    public override int GetHashCode() => coulombMeters.GetHashCode();
}
