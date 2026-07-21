namespace com.hafthor.Measurement;

public class SurfaceChargeDensity {
    private readonly double coulombsPerSquareMeter;

    private SurfaceChargeDensity(double coulombsPerSquareMeter) => this.coulombsPerSquareMeter = coulombsPerSquareMeter;

    // Arithmetic
    public static SurfaceChargeDensity operator +(SurfaceChargeDensity a, SurfaceChargeDensity b) => new(a.coulombsPerSquareMeter + b.coulombsPerSquareMeter);
    public static SurfaceChargeDensity operator -(SurfaceChargeDensity a, SurfaceChargeDensity b) => new(a.coulombsPerSquareMeter - b.coulombsPerSquareMeter);
    public static SurfaceChargeDensity operator -(SurfaceChargeDensity x) => new(-x.coulombsPerSquareMeter);

    // Units
    public static SurfaceChargeDensity FromCoulombsPerSquareMeter(double coulombsPerSquareMeter) => new(coulombsPerSquareMeter);
    public double ToCoulombsPerSquareMeter() => coulombsPerSquareMeter;

    // Composite relationships
    public static ElectricCharge operator *(SurfaceChargeDensity surfaceChargeDensity, Area area) => ElectricCharge.FromCoulombs(surfaceChargeDensity.ToCoulombsPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCharge operator *(Area area, SurfaceChargeDensity surfaceChargeDensity) => ElectricCharge.FromCoulombs(area.ToSquareMeters() * surfaceChargeDensity.ToCoulombsPerSquareMeter());

    public override string ToString() => $"{coulombsPerSquareMeter} C/m²";

    public override bool Equals(object obj) => obj is SurfaceChargeDensity other && other.coulombsPerSquareMeter == coulombsPerSquareMeter;
    public override int GetHashCode() => coulombsPerSquareMeter.GetHashCode();
}
