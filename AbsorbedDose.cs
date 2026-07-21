namespace com.hafthor.Measurement;

public sealed class AbsorbedDose : Measurement<AbsorbedDose> {

    private AbsorbedDose(double value) : base(value) { }

    protected override AbsorbedDose Create(double value) => new(value);
    protected override string Symbol => "Gy";

    // SI units
    public static AbsorbedDose FromKilograys(double kilograys) => new(kilograys * 1e3);
    public double ToKilograys() => value / 1e3;
    public static AbsorbedDose FromGrays(double value) => new(value);
    public double ToGrays() => value;
    public static AbsorbedDose FromMilligrays(double milligrays) => new(milligrays * 1e-3);
    public double ToMilligrays() => value / 1e-3;
    public static AbsorbedDose FromMicrograys(double micrograys) => new(micrograys * 1e-6);
    public double ToMicrograys() => value / 1e-6;

    // Legacy units
    public static AbsorbedDose FromRads(double rads) => new(rads * 1e-2);
    public double ToRads() => value / 1e-2;
    public static AbsorbedDose FromMillirads(double millirads) => new(millirads * 1e-5);
    public double ToMillirads() => value / 1e-5;

    // Composite relationships (derived)
    public static DoseRate operator /(AbsorbedDose absorbedDose, Duration duration) => DoseRate.FromGraysPerSecond(absorbedDose.ToGrays() / duration.ToSeconds());

}
