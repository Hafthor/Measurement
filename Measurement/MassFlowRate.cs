namespace com.hafthor.Measurement;

[Measurement("g/s", VariableName = "gramsPerSecond")]
public readonly partial struct MassFlowRate {
    // Units
    public static MassFlowRate FromKilogramsPerSecond(double kilogramsPerSecond) => new(kilogramsPerSecond * 1e3);
    public double ToKilogramsPerSecond() => gramsPerSecond / 1e3;
    public static MassFlowRate FromGramsPerSecond(double gramsPerSecond) => new(gramsPerSecond);
    public double ToGramsPerSecond() => gramsPerSecond;
    public static MassFlowRate FromKilogramsPerHour(double kilogramsPerHour) => new(kilogramsPerHour * (1e3 / 3600));
    public double ToKilogramsPerHour() => gramsPerSecond / (1e3 / 3600);
    public static MassFlowRate FromPoundsPerSecond(double poundsPerSecond) => new(poundsPerSecond * (0.45359237e3));
    public double ToPoundsPerSecond() => gramsPerSecond / (0.45359237e3);
    public static MassFlowRate FromPoundsPerHour(double poundsPerHour) => new(poundsPerHour * (0.45359237e3 / 3600));
    public double ToPoundsPerHour() => gramsPerSecond / (0.45359237e3 / 3600);
    public static MassFlowRate FromTonnesPerHour(double tonnesPerHour) => new(tonnesPerHour * (1000e3 / 3600));
    public double ToTonnesPerHour() => gramsPerSecond / (1000e3 / 3600);

    // Composite relationships
    public static Mass operator *(MassFlowRate massFlowRate, Duration duration) => Mass.FromKilograms(massFlowRate.ToKilogramsPerSecond() * duration.ToSeconds());
    public static Mass operator *(Duration duration, MassFlowRate massFlowRate) => Mass.FromKilograms(duration.ToSeconds() * massFlowRate.ToKilogramsPerSecond());
}
