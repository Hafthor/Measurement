namespace com.hafthor.Measurement;

[Measurement("kat", VariableName = "nanokatals", DisplayFactor = 1e9)]
public readonly partial struct CatalyticActivity {
    // SI units
    public static CatalyticActivity FromKatals(double katals) => new(katals * 1e9);
    public double ToKatals() => nanokatals / 1e9;
    public static CatalyticActivity FromMillikatals(double millikatals) => new(millikatals * 1e6);
    public double ToMillikatals() => nanokatals / 1e6;
    public static CatalyticActivity FromMicrokatals(double microkatals) => new(microkatals * 1e3);
    public double ToMicrokatals() => nanokatals / 1e3;
    public static CatalyticActivity FromNanokatals(double nanokatals) => new(nanokatals);
    public double ToNanokatals() => nanokatals;

    // Enzyme unit (1 U = 1 micromole per minute)
    private const double EnzymeUnitsToNanokatals = 50.0 / 3;
    public static CatalyticActivity FromEnzymeUnits(double enzymeUnits) => new(enzymeUnits * EnzymeUnitsToNanokatals);
    public double ToEnzymeUnits() => nanokatals / EnzymeUnitsToNanokatals;

    // Composite relationships (derived)
    public static CatalyticConcentration operator /(CatalyticActivity catalyticActivity, Volume volume) => CatalyticConcentration.FromKatalsPerCubicMeter(catalyticActivity.ToKatals() / volume.ToCubicMeters());
}
