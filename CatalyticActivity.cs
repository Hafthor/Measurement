namespace com.hafthor.Measurement;

public sealed class CatalyticActivity : Measurement<CatalyticActivity> {

    private CatalyticActivity(double value) : base(value) { }

    protected override CatalyticActivity Create(double value) => new(value);
    protected override string Symbol => "kat";

    // SI units
    public static CatalyticActivity FromKatals(double value) => new(value);
    public double ToKatals() => value;
    public static CatalyticActivity FromMillikatals(double millikatals) => new(millikatals * 1e-3);
    public double ToMillikatals() => value / 1e-3;
    public static CatalyticActivity FromMicrokatals(double microkatals) => new(microkatals * 1e-6);
    public double ToMicrokatals() => value / 1e-6;
    public static CatalyticActivity FromNanokatals(double nanokatals) => new(nanokatals * 1e-9);
    public double ToNanokatals() => value / 1e-9;

    // Enzyme unit (1 U = 1 micromole per minute)
    public static CatalyticActivity FromEnzymeUnits(double enzymeUnits) => new(enzymeUnits * 1.6666666666666667e-8);
    public double ToEnzymeUnits() => value / 1.6666666666666667e-8;

    // Composite relationships (derived)
    public static CatalyticConcentration operator /(CatalyticActivity catalyticActivity, Volume volume) => CatalyticConcentration.FromKatalsPerCubicMeter(catalyticActivity.ToKatals() / volume.ToCubicMeters());

}
