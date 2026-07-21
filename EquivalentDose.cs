namespace com.hafthor.Measurement;

public class EquivalentDose {
    private readonly double sieverts;

    private EquivalentDose(double sieverts) => this.sieverts = sieverts;

    // Arithmetic
    public static EquivalentDose operator +(EquivalentDose a, EquivalentDose b) => new(a.sieverts + b.sieverts);
    public static EquivalentDose operator -(EquivalentDose a, EquivalentDose b) => new(a.sieverts - b.sieverts);
    public static EquivalentDose operator -(EquivalentDose x) => new(-x.sieverts);

    // SI units
    public static EquivalentDose FromSieverts(double sieverts) => new(sieverts);
    public double ToSieverts() => sieverts;
    public static EquivalentDose FromMillisieverts(double millisieverts) => new(millisieverts * 1e-3);
    public double ToMillisieverts() => sieverts / 1e-3;
    public static EquivalentDose FromMicrosieverts(double microsieverts) => new(microsieverts * 1e-6);
    public double ToMicrosieverts() => sieverts / 1e-6;

    // Legacy units
    public static EquivalentDose FromRems(double rems) => new(rems * 1e-2);
    public double ToRems() => sieverts / 1e-2;
    public static EquivalentDose FromMillirems(double millirems) => new(millirems * 1e-5);
    public double ToMillirems() => sieverts / 1e-5;

    public override string ToString() => $"{sieverts} Sv";
}
