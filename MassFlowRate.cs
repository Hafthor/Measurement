namespace com.hafthor.Measurement;

public class MassFlowRate {
    private readonly double kilogramsPerSecond;

    private MassFlowRate(double kilogramsPerSecond) => this.kilogramsPerSecond = kilogramsPerSecond;

    // Arithmetic
    public static MassFlowRate operator +(MassFlowRate a, MassFlowRate b) => new MassFlowRate(a.kilogramsPerSecond + b.kilogramsPerSecond);
    public static MassFlowRate operator -(MassFlowRate a, MassFlowRate b) => new MassFlowRate(a.kilogramsPerSecond - b.kilogramsPerSecond);
    public static MassFlowRate operator -(MassFlowRate x) => new MassFlowRate(-x.kilogramsPerSecond);

    // Units
    public static MassFlowRate FromKilogramsPerSecond(double kilogramsPerSecond) => new MassFlowRate(kilogramsPerSecond);
    public double ToKilogramsPerSecond() => kilogramsPerSecond;
    public static MassFlowRate FromGramsPerSecond(double gramsPerSecond) => new MassFlowRate(gramsPerSecond * (1e-3));
    public double ToGramsPerSecond() => kilogramsPerSecond / (1e-3);
    public static MassFlowRate FromKilogramsPerHour(double kilogramsPerHour) => new MassFlowRate(kilogramsPerHour * (1.0 / 3600));
    public double ToKilogramsPerHour() => kilogramsPerSecond / (1.0 / 3600);
    public static MassFlowRate FromPoundsPerSecond(double poundsPerSecond) => new MassFlowRate(poundsPerSecond * (0.45359237));
    public double ToPoundsPerSecond() => kilogramsPerSecond / (0.45359237);
    public static MassFlowRate FromPoundsPerHour(double poundsPerHour) => new MassFlowRate(poundsPerHour * (0.45359237 / 3600));
    public double ToPoundsPerHour() => kilogramsPerSecond / (0.45359237 / 3600);
    public static MassFlowRate FromTonnesPerHour(double tonnesPerHour) => new MassFlowRate(tonnesPerHour * (1000.0 / 3600));
    public double ToTonnesPerHour() => kilogramsPerSecond / (1000.0 / 3600);

    // Composite relationships
    public static Mass operator *(MassFlowRate massFlowRate, Duration duration) => Mass.FromKilograms(massFlowRate.ToKilogramsPerSecond() * duration.ToSeconds());
    public static Mass operator *(Duration duration, MassFlowRate massFlowRate) => Mass.FromKilograms(duration.ToSeconds() * massFlowRate.ToKilogramsPerSecond());
}
