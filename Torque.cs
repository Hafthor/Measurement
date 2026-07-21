namespace com.hafthor.Measurement;

public class Torque {
    private readonly double newtonMeters;

    private Torque(double newtonMeters) => this.newtonMeters = newtonMeters;

    // Arithmetic
    public static Torque operator +(Torque a, Torque b) => new(a.newtonMeters + b.newtonMeters);
    public static Torque operator -(Torque a, Torque b) => new(a.newtonMeters - b.newtonMeters);
    public static Torque operator -(Torque x) => new(-x.newtonMeters);

    // Units
    public static Torque FromNewtonMeters(double newtonMeters) => new(newtonMeters);
    public double ToNewtonMeters() => newtonMeters;
    public static Torque FromNewtonMillimeters(double newtonMillimeters) => new(newtonMillimeters * (1e-3));
    public double ToNewtonMillimeters() => newtonMeters / (1e-3);
    public static Torque FromKilogramForceMeters(double kilogramForceMeters) => new(kilogramForceMeters * (9.80665));
    public double ToKilogramForceMeters() => newtonMeters / (9.80665);
    public static Torque FromPoundFeet(double poundFeet) => new(poundFeet * (1.3558179483314004));
    public double ToPoundFeet() => newtonMeters / (1.3558179483314004);
    public static Torque FromPoundInches(double poundInches) => new(poundInches * (0.11298482902762));
    public double ToPoundInches() => newtonMeters / (0.11298482902762);
}
