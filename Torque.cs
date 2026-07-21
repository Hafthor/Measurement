namespace com.hafthor.Measurement;

public class Torque {
    private readonly double newtonMeters;

    private Torque(double newtonMeters) => this.newtonMeters = newtonMeters;

    // Arithmetic
    public static Torque operator +(Torque a, Torque b) => new Torque(a.newtonMeters + b.newtonMeters);
    public static Torque operator -(Torque a, Torque b) => new Torque(a.newtonMeters - b.newtonMeters);
    public static Torque operator -(Torque x) => new Torque(-x.newtonMeters);

    // Units
    public static Torque FromNewtonMeters(double newtonMeters) => new Torque(newtonMeters);
    public double ToNewtonMeters() => newtonMeters;
    public static Torque FromNewtonMillimeters(double newtonMillimeters) => new Torque(newtonMillimeters * (1e-3));
    public double ToNewtonMillimeters() => newtonMeters / (1e-3);
    public static Torque FromKilogramForceMeters(double kilogramForceMeters) => new Torque(kilogramForceMeters * (9.80665));
    public double ToKilogramForceMeters() => newtonMeters / (9.80665);
    public static Torque FromPoundFeet(double poundFeet) => new Torque(poundFeet * (1.3558179483314004));
    public double ToPoundFeet() => newtonMeters / (1.3558179483314004);
    public static Torque FromPoundInches(double poundInches) => new Torque(poundInches * (0.11298482902762));
    public double ToPoundInches() => newtonMeters / (0.11298482902762);
}
