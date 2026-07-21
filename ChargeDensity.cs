namespace com.hafthor.Measurement;

public class ChargeDensity {
    private readonly double coulombsPerCubicMeter;

    private ChargeDensity(double coulombsPerCubicMeter) => this.coulombsPerCubicMeter = coulombsPerCubicMeter;

    // Arithmetic
    public static ChargeDensity operator +(ChargeDensity a, ChargeDensity b) => new(a.coulombsPerCubicMeter + b.coulombsPerCubicMeter);
    public static ChargeDensity operator -(ChargeDensity a, ChargeDensity b) => new(a.coulombsPerCubicMeter - b.coulombsPerCubicMeter);
    public static ChargeDensity operator -(ChargeDensity x) => new(-x.coulombsPerCubicMeter);

    // Units
    public static ChargeDensity FromCoulombsPerCubicMeter(double coulombsPerCubicMeter) => new(coulombsPerCubicMeter);
    public double ToCoulombsPerCubicMeter() => coulombsPerCubicMeter;

    // Composite relationships
    public static ElectricCharge operator *(ChargeDensity chargeDensity, Volume volume) => ElectricCharge.FromCoulombs(chargeDensity.ToCoulombsPerCubicMeter() * volume.ToCubicMeters());
    public static ElectricCharge operator *(Volume volume, ChargeDensity chargeDensity) => ElectricCharge.FromCoulombs(volume.ToCubicMeters() * chargeDensity.ToCoulombsPerCubicMeter());
}
