namespace com.hafthor.Measurement;

[Measurement("Sv")]
public readonly partial struct EquivalentDose {

    // SI units
    public static EquivalentDose FromSieverts(double sieverts) => new(sieverts);
    public double ToSieverts() => value;
    public static EquivalentDose FromMillisieverts(double millisieverts) => new(millisieverts * 1e-3);
    public double ToMillisieverts() => value / 1e-3;
    public static EquivalentDose FromMicrosieverts(double microsieverts) => new(microsieverts * 1e-6);
    public double ToMicrosieverts() => value / 1e-6;

    // Legacy units
    public static EquivalentDose FromRems(double rems) => new(rems * 1e-2);
    public double ToRems() => value / 1e-2;
    public static EquivalentDose FromMillirems(double millirems) => new(millirems * 1e-5);
    public double ToMillirems() => value / 1e-5;

}
