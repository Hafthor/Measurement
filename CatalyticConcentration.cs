namespace com.hafthor.Measurement;

public class CatalyticConcentration {
    private readonly double katalsPerCubicMeter;

    private CatalyticConcentration(double katalsPerCubicMeter) => this.katalsPerCubicMeter = katalsPerCubicMeter;

    // Arithmetic
    public static CatalyticConcentration operator +(CatalyticConcentration a, CatalyticConcentration b) => new(a.katalsPerCubicMeter + b.katalsPerCubicMeter);
    public static CatalyticConcentration operator -(CatalyticConcentration a, CatalyticConcentration b) => new(a.katalsPerCubicMeter - b.katalsPerCubicMeter);
    public static CatalyticConcentration operator -(CatalyticConcentration x) => new(-x.katalsPerCubicMeter);

    // Units
    public static CatalyticConcentration FromKatalsPerCubicMeter(double katalsPerCubicMeter) => new(katalsPerCubicMeter);
    public double ToKatalsPerCubicMeter() => katalsPerCubicMeter;
    public static CatalyticConcentration FromKatalsPerLiter(double katalsPerLiter) => new(katalsPerLiter * (1000));
    public double ToKatalsPerLiter() => katalsPerCubicMeter / (1000);

    // Composite relationships
    public static CatalyticActivity operator *(CatalyticConcentration catalyticConcentration, Volume volume) => CatalyticActivity.FromKatals(catalyticConcentration.ToKatalsPerCubicMeter() * volume.ToCubicMeters());
    public static CatalyticActivity operator *(Volume volume, CatalyticConcentration catalyticConcentration) => CatalyticActivity.FromKatals(volume.ToCubicMeters() * catalyticConcentration.ToKatalsPerCubicMeter());
}
