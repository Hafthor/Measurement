namespace com.hafthor.Measurement;

public sealed class SurfaceChargeDensity : Measurement<SurfaceChargeDensity> {

    private SurfaceChargeDensity(double value) : base(value) { }

    protected override SurfaceChargeDensity Create(double value) => new(value);
    protected override string Symbol => "C/m²";

    // Units
    public static SurfaceChargeDensity FromCoulombsPerSquareMeter(double value) => new(value);
    public double ToCoulombsPerSquareMeter() => value;

    // Composite relationships
    public static ElectricCharge operator *(SurfaceChargeDensity surfaceChargeDensity, Area area) => ElectricCharge.FromCoulombs(surfaceChargeDensity.ToCoulombsPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCharge operator *(Area area, SurfaceChargeDensity surfaceChargeDensity) => ElectricCharge.FromCoulombs(area.ToSquareMeters() * surfaceChargeDensity.ToCoulombsPerSquareMeter());

}
