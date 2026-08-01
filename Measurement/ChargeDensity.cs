namespace com.hafthor.Measurement;

[Measurement("C/m³", VariableName = "coulombsPerCubicMeter")]
[SiUnit("CoulombsPerCubicMeter", 0)]
public readonly partial struct ChargeDensity {
    // Composite relationships
    public static ElectricCharge operator *(ChargeDensity chargeDensity, Volume volume) => ElectricCharge.FromCoulombs(chargeDensity.ToCoulombsPerCubicMeter() * volume.ToCubicMeters());
    public static ElectricCharge operator *(Volume volume, ChargeDensity chargeDensity) => ElectricCharge.FromCoulombs(volume.ToCubicMeters() * chargeDensity.ToCoulombsPerCubicMeter());
}
