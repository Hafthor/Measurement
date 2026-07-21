namespace com.hafthor.Measurement;

[Measurement("N·m")]
public readonly partial struct Torque {

    // Units
    public static Torque FromNewtonMeters(double newtonMeters) => new(newtonMeters);
    public double ToNewtonMeters() => value;
    public static Torque FromNewtonMillimeters(double newtonMillimeters) => new(newtonMillimeters * (1e-3));
    public double ToNewtonMillimeters() => value / (1e-3);
    public static Torque FromKilogramForceMeters(double kilogramForceMeters) => new(kilogramForceMeters * (9.80665));
    public double ToKilogramForceMeters() => value / (9.80665);
    public static Torque FromPoundFeet(double poundFeet) => new(poundFeet * (1.3558179483314004));
    public double ToPoundFeet() => value / (1.3558179483314004);
    public static Torque FromPoundInches(double poundInches) => new(poundInches * (0.11298482902762));
    public double ToPoundInches() => value / (0.11298482902762);

}
