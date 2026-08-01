namespace com.hafthor.Measurement;

[Measurement("Gy/s", VariableName = "milligraysPerHour", DisplayFactor = 3600e3)]
[Unit("GraysPerSecond", 3600e3)]
[Unit("MilligraysPerSecond", 3600)]
[SiUnit("GraysPerHour", 3)]
public readonly partial struct DoseRate {
    // Composite relationships
    public static AbsorbedDose operator *(DoseRate doseRate, Duration duration) => AbsorbedDose.FromGrays(doseRate.ToGraysPerSecond() * duration.ToSeconds());
    public static AbsorbedDose operator *(Duration duration, DoseRate doseRate) => AbsorbedDose.FromGrays(duration.ToSeconds() * doseRate.ToGraysPerSecond());
}
