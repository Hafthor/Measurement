namespace com.hafthor.Measurement;

[Measurement("C/m²", VariableName = "coulombsPerSquareMeter")]
[SiUnit("CoulombsPerSquareMeter", 0)]
public readonly partial struct SurfaceChargeDensity {
    // Composite relationships
    public static ElectricCharge operator *(SurfaceChargeDensity surfaceChargeDensity, Area area) => ElectricCharge.FromCoulombs(surfaceChargeDensity.ToCoulombsPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCharge operator *(Area area, SurfaceChargeDensity surfaceChargeDensity) => ElectricCharge.FromCoulombs(area.ToSquareMeters() * surfaceChargeDensity.ToCoulombsPerSquareMeter());
}
