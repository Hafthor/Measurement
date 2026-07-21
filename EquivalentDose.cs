namespace com.hafthor.Measurement;

public sealed class EquivalentDose : Measurement<EquivalentDose> {

    private EquivalentDose(double value) : base(value) { }

    protected override EquivalentDose Create(double value) => new(value);
    protected override string Symbol => "Sv";

    // SI units
    public static EquivalentDose FromSieverts(double value) => new(value);
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
