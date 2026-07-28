namespace com.hafthor.Measurement;

[Measurement("Gy", VariableName = "micrograys", DisplayFactor = 1e6)]
public readonly partial struct AbsorbedDose {
    // SI units
    public static AbsorbedDose FromKilograys(double kilograys) => new(kilograys * 1e9);
    public double ToKilograys() => micrograys / 1e9;
    public static AbsorbedDose FromGrays(double grays) => new(grays * 1e6);
    public double ToGrays() => micrograys / 1e6;
    public static AbsorbedDose FromMilligrays(double milligrays) => new(milligrays * 1e3);
    public double ToMilligrays() => micrograys / 1e3;
    public static AbsorbedDose FromMicrograys(double micrograys) => new(micrograys);
    public double ToMicrograys() => micrograys;

    // Legacy units
    public static AbsorbedDose FromRads(double rads) => new(rads * 1e4);
    public double ToRads() => micrograys / 1e4;
    public static AbsorbedDose FromMillirads(double millirads) => new(millirads * 1e1);
    public double ToMillirads() => micrograys / 1e1;

    // Composite relationships (derived)
    public static DoseRate operator /(AbsorbedDose absorbedDose, Duration duration) => DoseRate.FromGraysPerSecond(absorbedDose.ToGrays() / duration.ToSeconds());
}
