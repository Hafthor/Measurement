namespace com.hafthor.Measurement;

public sealed class Torque : Measurement<Torque> {

    private Torque(double value) : base(value) { }

    protected override Torque Create(double value) => new(value);
    protected override string Symbol => "N·m";

    // Units
    public static Torque FromNewtonMeters(double value) => new(value);
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
