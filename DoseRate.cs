namespace com.hafthor.Measurement;

[Measurement("Gy/s")]
public readonly partial struct DoseRate {

    // Units
    public static DoseRate FromGraysPerSecond(double graysPerSecond) => new(graysPerSecond);
    public double ToGraysPerSecond() => value;
    public static DoseRate FromMilligraysPerSecond(double milligraysPerSecond) => new(milligraysPerSecond * (1e-3));
    public double ToMilligraysPerSecond() => value / (1e-3);
    public static DoseRate FromGraysPerHour(double graysPerHour) => new(graysPerHour * (1.0 / 3600));
    public double ToGraysPerHour() => value / (1.0 / 3600);

    // Composite relationships
    public static AbsorbedDose operator *(DoseRate doseRate, Duration duration) => AbsorbedDose.FromGrays(doseRate.ToGraysPerSecond() * duration.ToSeconds());
    public static AbsorbedDose operator *(Duration duration, DoseRate doseRate) => AbsorbedDose.FromGrays(duration.ToSeconds() * doseRate.ToGraysPerSecond());

}
