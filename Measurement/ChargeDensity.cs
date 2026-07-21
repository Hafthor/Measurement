namespace com.hafthor.Measurement;

[Measurement("C/m³")]
public readonly partial struct ChargeDensity {

    // Units
    public static ChargeDensity FromCoulombsPerCubicMeter(double coulombsPerCubicMeter) => new(coulombsPerCubicMeter);
    public double ToCoulombsPerCubicMeter() => value;

    // Composite relationships
    public static ElectricCharge operator *(ChargeDensity chargeDensity, Volume volume) => ElectricCharge.FromCoulombs(chargeDensity.ToCoulombsPerCubicMeter() * volume.ToCubicMeters());
    public static ElectricCharge operator *(Volume volume, ChargeDensity chargeDensity) => ElectricCharge.FromCoulombs(volume.ToCubicMeters() * chargeDensity.ToCoulombsPerCubicMeter());

}
