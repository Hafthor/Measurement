namespace com.hafthor.Measurement;

public sealed class CatalyticConcentration : Measurement<CatalyticConcentration> {

    private CatalyticConcentration(double value) : base(value) { }

    protected override CatalyticConcentration Create(double value) => new(value);
    protected override string Symbol => "kat/m³";

    // Units
    public static CatalyticConcentration FromKatalsPerCubicMeter(double value) => new(value);
    public double ToKatalsPerCubicMeter() => value;
    public static CatalyticConcentration FromKatalsPerLiter(double katalsPerLiter) => new(katalsPerLiter * (1000));
    public double ToKatalsPerLiter() => value / (1000);

    // Composite relationships
    public static CatalyticActivity operator *(CatalyticConcentration catalyticConcentration, Volume volume) => CatalyticActivity.FromKatals(catalyticConcentration.ToKatalsPerCubicMeter() * volume.ToCubicMeters());
    public static CatalyticActivity operator *(Volume volume, CatalyticConcentration catalyticConcentration) => CatalyticActivity.FromKatals(volume.ToCubicMeters() * catalyticConcentration.ToKatalsPerCubicMeter());

}
