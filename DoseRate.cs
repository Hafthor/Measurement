namespace com.hafthor.Measurement;

public class DoseRate {
    private readonly double graysPerSecond;

    private DoseRate(double graysPerSecond) => this.graysPerSecond = graysPerSecond;

    // Arithmetic
    public static DoseRate operator +(DoseRate a, DoseRate b) => new DoseRate(a.graysPerSecond + b.graysPerSecond);
    public static DoseRate operator -(DoseRate a, DoseRate b) => new DoseRate(a.graysPerSecond - b.graysPerSecond);
    public static DoseRate operator -(DoseRate x) => new DoseRate(-x.graysPerSecond);

    // Units
    public static DoseRate FromGraysPerSecond(double graysPerSecond) => new DoseRate(graysPerSecond);
    public double ToGraysPerSecond() => graysPerSecond;
    public static DoseRate FromMilligraysPerSecond(double milligraysPerSecond) => new DoseRate(milligraysPerSecond * (1e-3));
    public double ToMilligraysPerSecond() => graysPerSecond / (1e-3);
    public static DoseRate FromGraysPerHour(double graysPerHour) => new DoseRate(graysPerHour * (1.0 / 3600));
    public double ToGraysPerHour() => graysPerSecond / (1.0 / 3600);

    // Composite relationships
    public static AbsorbedDose operator *(DoseRate doseRate, Duration duration) => AbsorbedDose.FromGrays(doseRate.ToGraysPerSecond() * duration.ToSeconds());
    public static AbsorbedDose operator *(Duration duration, DoseRate doseRate) => AbsorbedDose.FromGrays(duration.ToSeconds() * doseRate.ToGraysPerSecond());
}
