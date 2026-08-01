namespace com.hafthor.Measurement;

[Measurement("g/s", VariableName = "gramsPerSecond")]
[SiUnit("GramsPerSecond", 0, "None Kilo")]
[Unit("KilogramsPerHour", 1e3 / 3600)]
[Unit("PoundsPerSecond", 0.45359237e3)]
[Unit("PoundsPerHour", 0.45359237e3 / 3600)]
[Unit("TonnesPerHour", 1000e3 / 3600)]
public readonly partial struct MassFlowRate {
    // Composite relationships
    public static Mass operator *(MassFlowRate massFlowRate, Duration duration) => Mass.FromKilograms(massFlowRate.ToKilogramsPerSecond() * duration.ToSeconds());
    public static Mass operator *(Duration duration, MassFlowRate massFlowRate) => Mass.FromKilograms(duration.ToSeconds() * massFlowRate.ToKilogramsPerSecond());
}
