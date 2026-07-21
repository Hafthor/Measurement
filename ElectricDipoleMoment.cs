namespace com.hafthor.Measurement;

public class ElectricDipoleMoment {
    private readonly double coulombMeters;

    private ElectricDipoleMoment(double coulombMeters) => this.coulombMeters = coulombMeters;

    // Arithmetic
    public static ElectricDipoleMoment operator +(ElectricDipoleMoment a, ElectricDipoleMoment b) => new ElectricDipoleMoment(a.coulombMeters + b.coulombMeters);
    public static ElectricDipoleMoment operator -(ElectricDipoleMoment a, ElectricDipoleMoment b) => new ElectricDipoleMoment(a.coulombMeters - b.coulombMeters);
    public static ElectricDipoleMoment operator -(ElectricDipoleMoment x) => new ElectricDipoleMoment(-x.coulombMeters);

    // Units
    public static ElectricDipoleMoment FromCoulombMeters(double coulombMeters) => new ElectricDipoleMoment(coulombMeters);
    public double ToCoulombMeters() => coulombMeters;
    public static ElectricDipoleMoment FromDebyes(double debyes) => new ElectricDipoleMoment(debyes * (3.33564095198e-30));
    public double ToDebyes() => coulombMeters / (3.33564095198e-30);

    // Composite relationships
    public static ElectricCharge operator /(ElectricDipoleMoment electricDipoleMoment, Length length) => ElectricCharge.FromCoulombs(electricDipoleMoment.ToCoulombMeters() / length.ToMeters());
    public static Length operator /(ElectricDipoleMoment electricDipoleMoment, ElectricCharge electricCharge) => Length.FromMeters(electricDipoleMoment.ToCoulombMeters() / electricCharge.ToCoulombs());
}
