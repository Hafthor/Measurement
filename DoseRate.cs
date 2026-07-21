namespace com.hafthor.Measurement;

public class DoseRate {
    private readonly double graysPerSecond;

    private DoseRate(double graysPerSecond) => this.graysPerSecond = graysPerSecond;

    // Arithmetic
    public static DoseRate operator +(DoseRate a, DoseRate b) => new(a.graysPerSecond + b.graysPerSecond);
    public static DoseRate operator -(DoseRate a, DoseRate b) => new(a.graysPerSecond - b.graysPerSecond);
    public static DoseRate operator -(DoseRate x) => new(-x.graysPerSecond);

    // Units
    public static DoseRate FromGraysPerSecond(double graysPerSecond) => new(graysPerSecond);
    public double ToGraysPerSecond() => graysPerSecond;
    public static DoseRate FromMilligraysPerSecond(double milligraysPerSecond) => new(milligraysPerSecond * (1e-3));
    public double ToMilligraysPerSecond() => graysPerSecond / (1e-3);
    public static DoseRate FromGraysPerHour(double graysPerHour) => new(graysPerHour * (1.0 / 3600));
    public double ToGraysPerHour() => graysPerSecond / (1.0 / 3600);

    // Composite relationships
    public static AbsorbedDose operator *(DoseRate doseRate, Duration duration) => AbsorbedDose.FromGrays(doseRate.ToGraysPerSecond() * duration.ToSeconds());
    public static AbsorbedDose operator *(Duration duration, DoseRate doseRate) => AbsorbedDose.FromGrays(duration.ToSeconds() * doseRate.ToGraysPerSecond());

    public override string ToString() => $"{graysPerSecond} Gy/s";
}
