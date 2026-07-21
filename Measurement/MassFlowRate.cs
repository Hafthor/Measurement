namespace com.hafthor.Measurement;

[Measurement("kg/s")]
public readonly partial struct MassFlowRate {

    // Units
    public static MassFlowRate FromKilogramsPerSecond(double kilogramsPerSecond) => new(kilogramsPerSecond);
    public double ToKilogramsPerSecond() => value;
    public static MassFlowRate FromGramsPerSecond(double gramsPerSecond) => new(gramsPerSecond * (1e-3));
    public double ToGramsPerSecond() => value / (1e-3);
    public static MassFlowRate FromKilogramsPerHour(double kilogramsPerHour) => new(kilogramsPerHour * (1.0 / 3600));
    public double ToKilogramsPerHour() => value / (1.0 / 3600);
    public static MassFlowRate FromPoundsPerSecond(double poundsPerSecond) => new(poundsPerSecond * (0.45359237));
    public double ToPoundsPerSecond() => value / (0.45359237);
    public static MassFlowRate FromPoundsPerHour(double poundsPerHour) => new(poundsPerHour * (0.45359237 / 3600));
    public double ToPoundsPerHour() => value / (0.45359237 / 3600);
    public static MassFlowRate FromTonnesPerHour(double tonnesPerHour) => new(tonnesPerHour * (1000.0 / 3600));
    public double ToTonnesPerHour() => value / (1000.0 / 3600);

    // Composite relationships
    public static Mass operator *(MassFlowRate massFlowRate, Duration duration) => Mass.FromKilograms(massFlowRate.ToKilogramsPerSecond() * duration.ToSeconds());
    public static Mass operator *(Duration duration, MassFlowRate massFlowRate) => Mass.FromKilograms(duration.ToSeconds() * massFlowRate.ToKilogramsPerSecond());

}
