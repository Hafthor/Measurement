namespace com.hafthor.Measurement;

public class CatalyticActivity {
    private readonly double katals;

    private CatalyticActivity(double katals) => this.katals = katals;

    // Arithmetic
    public static CatalyticActivity operator +(CatalyticActivity a, CatalyticActivity b) => new(a.katals + b.katals);
    public static CatalyticActivity operator -(CatalyticActivity a, CatalyticActivity b) => new(a.katals - b.katals);
    public static CatalyticActivity operator -(CatalyticActivity x) => new(-x.katals);

    // SI units
    public static CatalyticActivity FromKatals(double katals) => new(katals);
    public double ToKatals() => katals;
    public static CatalyticActivity FromMillikatals(double millikatals) => new(millikatals * 1e-3);
    public double ToMillikatals() => katals / 1e-3;
    public static CatalyticActivity FromMicrokatals(double microkatals) => new(microkatals * 1e-6);
    public double ToMicrokatals() => katals / 1e-6;
    public static CatalyticActivity FromNanokatals(double nanokatals) => new(nanokatals * 1e-9);
    public double ToNanokatals() => katals / 1e-9;

    // Enzyme unit (1 U = 1 micromole per minute)
    public static CatalyticActivity FromEnzymeUnits(double enzymeUnits) => new(enzymeUnits * 1.6666666666666667e-8);
    public double ToEnzymeUnits() => katals / 1.6666666666666667e-8;

    // Composite relationships (derived)
    public static CatalyticConcentration operator /(CatalyticActivity catalyticActivity, Volume volume) => CatalyticConcentration.FromKatalsPerCubicMeter(catalyticActivity.ToKatals() / volume.ToCubicMeters());

    public override string ToString() => $"{katals} kat";
}
