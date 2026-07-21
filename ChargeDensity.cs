namespace com.hafthor.Measurement;

public sealed class ChargeDensity : Measurement<ChargeDensity> {

    private ChargeDensity(double value) : base(value) { }

    protected override ChargeDensity Create(double value) => new(value);
    protected override string Symbol => "C/m³";

    // Units
    public static ChargeDensity FromCoulombsPerCubicMeter(double value) => new(value);
    public double ToCoulombsPerCubicMeter() => value;

    // Composite relationships
    public static ElectricCharge operator *(ChargeDensity chargeDensity, Volume volume) => ElectricCharge.FromCoulombs(chargeDensity.ToCoulombsPerCubicMeter() * volume.ToCubicMeters());
    public static ElectricCharge operator *(Volume volume, ChargeDensity chargeDensity) => ElectricCharge.FromCoulombs(volume.ToCubicMeters() * chargeDensity.ToCoulombsPerCubicMeter());

}
