namespace com.hafthor.Measurement;

public class SurfaceChargeDensity {
    private readonly double coulombsPerSquareMeter;

    private SurfaceChargeDensity(double coulombsPerSquareMeter) => this.coulombsPerSquareMeter = coulombsPerSquareMeter;

    // Arithmetic
    public static SurfaceChargeDensity operator +(SurfaceChargeDensity a, SurfaceChargeDensity b) => new SurfaceChargeDensity(a.coulombsPerSquareMeter + b.coulombsPerSquareMeter);
    public static SurfaceChargeDensity operator -(SurfaceChargeDensity a, SurfaceChargeDensity b) => new SurfaceChargeDensity(a.coulombsPerSquareMeter - b.coulombsPerSquareMeter);
    public static SurfaceChargeDensity operator -(SurfaceChargeDensity x) => new SurfaceChargeDensity(-x.coulombsPerSquareMeter);

    // Units
    public static SurfaceChargeDensity FromCoulombsPerSquareMeter(double coulombsPerSquareMeter) => new SurfaceChargeDensity(coulombsPerSquareMeter);
    public double ToCoulombsPerSquareMeter() => coulombsPerSquareMeter;

    // Composite relationships
    public static ElectricCharge operator *(SurfaceChargeDensity surfaceChargeDensity, Area area) => ElectricCharge.FromCoulombs(surfaceChargeDensity.ToCoulombsPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCharge operator *(Area area, SurfaceChargeDensity surfaceChargeDensity) => ElectricCharge.FromCoulombs(area.ToSquareMeters() * surfaceChargeDensity.ToCoulombsPerSquareMeter());
}
