namespace com.hafthor.Measurement;

public sealed class DoseRate : Measurement<DoseRate> {

    private DoseRate(double value) : base(value) { }

    protected override DoseRate Create(double value) => new(value);
    protected override string Symbol => "Gy/s";

    // Units
    public static DoseRate FromGraysPerSecond(double value) => new(value);
    public double ToGraysPerSecond() => value;
    public static DoseRate FromMilligraysPerSecond(double milligraysPerSecond) => new(milligraysPerSecond * (1e-3));
    public double ToMilligraysPerSecond() => value / (1e-3);
    public static DoseRate FromGraysPerHour(double graysPerHour) => new(graysPerHour * (1.0 / 3600));
    public double ToGraysPerHour() => value / (1.0 / 3600);

    // Composite relationships
    public static AbsorbedDose operator *(DoseRate doseRate, Duration duration) => AbsorbedDose.FromGrays(doseRate.ToGraysPerSecond() * duration.ToSeconds());
    public static AbsorbedDose operator *(Duration duration, DoseRate doseRate) => AbsorbedDose.FromGrays(duration.ToSeconds() * doseRate.ToGraysPerSecond());

}
