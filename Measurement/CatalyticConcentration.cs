namespace com.hafthor.Measurement;

[Measurement("kat/m³", VariableName = "katalsPerCubicMeter")]
[SiUnit("KatalsPerCubicMeter", 0)]
[SiUnit("KatalsPerLiter", 3)]
public readonly partial struct CatalyticConcentration {
    // Composite relationships
    public static CatalyticActivity operator *(CatalyticConcentration catalyticConcentration, Volume volume) => CatalyticActivity.FromKatals(catalyticConcentration.ToKatalsPerCubicMeter() * volume.ToCubicMeters());
    public static CatalyticActivity operator *(Volume volume, CatalyticConcentration catalyticConcentration) => CatalyticActivity.FromKatals(volume.ToCubicMeters() * catalyticConcentration.ToKatalsPerCubicMeter());
}
