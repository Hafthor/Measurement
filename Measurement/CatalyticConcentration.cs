namespace com.hafthor.Measurement;

[Measurement("kat/m³")]
public readonly partial struct CatalyticConcentration {

    // Units
    public static CatalyticConcentration FromKatalsPerCubicMeter(double katalsPerCubicMeter) => new(katalsPerCubicMeter);
    public double ToKatalsPerCubicMeter() => value;
    public static CatalyticConcentration FromKatalsPerLiter(double katalsPerLiter) => new(katalsPerLiter * (1000));
    public double ToKatalsPerLiter() => value / (1000);

    // Composite relationships
    public static CatalyticActivity operator *(CatalyticConcentration catalyticConcentration, Volume volume) => CatalyticActivity.FromKatals(catalyticConcentration.ToKatalsPerCubicMeter() * volume.ToCubicMeters());
    public static CatalyticActivity operator *(Volume volume, CatalyticConcentration catalyticConcentration) => CatalyticActivity.FromKatals(volume.ToCubicMeters() * catalyticConcentration.ToKatalsPerCubicMeter());

}
