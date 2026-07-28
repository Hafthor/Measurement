namespace com.hafthor.Measurement;

[Measurement("Gy/s", VariableName = "milligraysPerHour", DisplayFactor = 3600e3)]
public readonly partial struct DoseRate {
    // Units
    public static DoseRate FromGraysPerSecond(double graysPerSecond) => new(graysPerSecond * 3600e3);
    public double ToGraysPerSecond() => milligraysPerHour / 3600e3;
    public static DoseRate FromMilligraysPerSecond(double milligraysPerSecond) => new(milligraysPerSecond * 3600);
    public double ToMilligraysPerSecond() => milligraysPerHour / 3600;
    public static DoseRate FromGraysPerHour(double graysPerHour) => new(graysPerHour * 1e3);
    public double ToGraysPerHour() => milligraysPerHour / 1e3;

    // Composite relationships
    public static AbsorbedDose operator *(DoseRate doseRate, Duration duration) => AbsorbedDose.FromGrays(doseRate.ToGraysPerSecond() * duration.ToSeconds());
    public static AbsorbedDose operator *(Duration duration, DoseRate doseRate) => AbsorbedDose.FromGrays(duration.ToSeconds() * doseRate.ToGraysPerSecond());
}
