namespace com.hafthor.Measurement;

[Measurement("Sv", VariableName = "microsieverts", DisplayFactor = 1e6)]
public readonly partial struct EquivalentDose {
    // SI units
    public static EquivalentDose FromSieverts(double sieverts) => new(sieverts * 1e6);
    public double ToSieverts() => microsieverts / 1e6;
    public static EquivalentDose FromMillisieverts(double millisieverts) => new(millisieverts * 1e3);
    public double ToMillisieverts() => microsieverts / 1e3;
    public static EquivalentDose FromMicrosieverts(double microsieverts) => new(microsieverts);
    public double ToMicrosieverts() => microsieverts;

    // Legacy units
    public static EquivalentDose FromRems(double rems) => new(rems * 1e4);
    public double ToRems() => microsieverts / 1e4;
    public static EquivalentDose FromMillirems(double millirems) => new(millirems * 1e1);
    public double ToMillirems() => microsieverts / 1e1;
}
