namespace com.hafthor.Measurement;

public class AbsorbedDose {
    private readonly double grays;

    private AbsorbedDose(double grays) => this.grays = grays;

    // Arithmetic
    public static AbsorbedDose operator +(AbsorbedDose a, AbsorbedDose b) => new AbsorbedDose(a.grays + b.grays);
    public static AbsorbedDose operator -(AbsorbedDose a, AbsorbedDose b) => new AbsorbedDose(a.grays - b.grays);
    public static AbsorbedDose operator -(AbsorbedDose x) => new AbsorbedDose(-x.grays);

    // SI units
    public static AbsorbedDose FromKilograys(double kilograys) => new AbsorbedDose(kilograys * 1e3);
    public double ToKilograys() => grays / 1e3;
    public static AbsorbedDose FromGrays(double grays) => new AbsorbedDose(grays);
    public double ToGrays() => grays;
    public static AbsorbedDose FromMilligrays(double milligrays) => new AbsorbedDose(milligrays * 1e-3);
    public double ToMilligrays() => grays / 1e-3;
    public static AbsorbedDose FromMicrograys(double micrograys) => new AbsorbedDose(micrograys * 1e-6);
    public double ToMicrograys() => grays / 1e-6;

    // Legacy units
    public static AbsorbedDose FromRads(double rads) => new AbsorbedDose(rads * 1e-2);
    public double ToRads() => grays / 1e-2;
    public static AbsorbedDose FromMillirads(double millirads) => new AbsorbedDose(millirads * 1e-5);
    public double ToMillirads() => grays / 1e-5;

    // Composite relationships (derived)
    public static DoseRate operator /(AbsorbedDose absorbedDose, Duration duration) => DoseRate.FromGraysPerSecond(absorbedDose.ToGrays() / duration.ToSeconds());
}
