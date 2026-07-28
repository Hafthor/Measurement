namespace com.hafthor.Measurement;

[Measurement("N·m", VariableName = "newtonMillimeters", DisplayFactor = 1e3)]
public readonly partial struct Torque {
    // Units
    public static Torque FromNewtonMeters(double newtonMeters) => new(newtonMeters * 1e3);
    public double ToNewtonMeters() => newtonMillimeters / 1e3;
    public static Torque FromNewtonMillimeters(double newtonMillimeters) => new(newtonMillimeters);
    public double ToNewtonMillimeters() => newtonMillimeters;
    public static Torque FromKilogramForceMeters(double kilogramForceMeters) => new(kilogramForceMeters * (9.80665e3));
    public double ToKilogramForceMeters() => newtonMillimeters / (9.80665e3);
    public static Torque FromPoundFeet(double poundFeet) => new(poundFeet * (1.3558179483314004e3));
    public double ToPoundFeet() => newtonMillimeters / (1.3558179483314004e3);
    public static Torque FromPoundInches(double poundInches) => new(poundInches * (0.11298482902762e3));
    public double ToPoundInches() => newtonMillimeters / (0.11298482902762e3);
}
