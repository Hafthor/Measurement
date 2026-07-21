namespace com.hafthor.Measurement;

public class AbsorbedDose {
    private readonly double grays;

    private AbsorbedDose(double grays) => this.grays = grays;

    // Arithmetic
    public static AbsorbedDose operator +(AbsorbedDose a, AbsorbedDose b) => new(a.grays + b.grays);
    public static AbsorbedDose operator -(AbsorbedDose a, AbsorbedDose b) => new(a.grays - b.grays);
    public static AbsorbedDose operator -(AbsorbedDose x) => new(-x.grays);

    // SI units
    public static AbsorbedDose FromKilograys(double kilograys) => new(kilograys * 1e3);
    public double ToKilograys() => grays / 1e3;
    public static AbsorbedDose FromGrays(double grays) => new(grays);
    public double ToGrays() => grays;
    public static AbsorbedDose FromMilligrays(double milligrays) => new(milligrays * 1e-3);
    public double ToMilligrays() => grays / 1e-3;
    public static AbsorbedDose FromMicrograys(double micrograys) => new(micrograys * 1e-6);
    public double ToMicrograys() => grays / 1e-6;

    // Legacy units
    public static AbsorbedDose FromRads(double rads) => new(rads * 1e-2);
    public double ToRads() => grays / 1e-2;
    public static AbsorbedDose FromMillirads(double millirads) => new(millirads * 1e-5);
    public double ToMillirads() => grays / 1e-5;

    // Composite relationships (derived)
    public static DoseRate operator /(AbsorbedDose absorbedDose, Duration duration) => DoseRate.FromGraysPerSecond(absorbedDose.ToGrays() / duration.ToSeconds());

    public override string ToString() => $"{grays} Gy";

    public override bool Equals(object obj) => obj is AbsorbedDose other && other.grays == grays;
    public override int GetHashCode() => grays.GetHashCode();
}
