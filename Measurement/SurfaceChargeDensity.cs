namespace com.hafthor.Measurement;

[Measurement("C/m²")]
public readonly partial struct SurfaceChargeDensity {

    // Units
    public static SurfaceChargeDensity FromCoulombsPerSquareMeter(double coulombsPerSquareMeter) => new(coulombsPerSquareMeter);
    public double ToCoulombsPerSquareMeter() => value;

    // Composite relationships
    public static ElectricCharge operator *(SurfaceChargeDensity surfaceChargeDensity, Area area) => ElectricCharge.FromCoulombs(surfaceChargeDensity.ToCoulombsPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCharge operator *(Area area, SurfaceChargeDensity surfaceChargeDensity) => ElectricCharge.FromCoulombs(area.ToSquareMeters() * surfaceChargeDensity.ToCoulombsPerSquareMeter());

}
