namespace com.hafthor.Measurement;

public class AreaDensity {
    private readonly double kilogramsPerSquareMeter;

    private AreaDensity(double kilogramsPerSquareMeter) => this.kilogramsPerSquareMeter = kilogramsPerSquareMeter;

    // Arithmetic
    public static AreaDensity operator +(AreaDensity a, AreaDensity b) => new(a.kilogramsPerSquareMeter + b.kilogramsPerSquareMeter);
    public static AreaDensity operator -(AreaDensity a, AreaDensity b) => new(a.kilogramsPerSquareMeter - b.kilogramsPerSquareMeter);
    public static AreaDensity operator -(AreaDensity x) => new(-x.kilogramsPerSquareMeter);

    // Units
    public static AreaDensity FromKilogramsPerSquareMeter(double kilogramsPerSquareMeter) => new(kilogramsPerSquareMeter);
    public double ToKilogramsPerSquareMeter() => kilogramsPerSquareMeter;
    public static AreaDensity FromGramsPerSquareMeter(double gramsPerSquareMeter) => new(gramsPerSquareMeter * (1e-3));
    public double ToGramsPerSquareMeter() => kilogramsPerSquareMeter / (1e-3);

    // Composite relationships
    public static Mass operator *(AreaDensity areaDensity, Area area) => Mass.FromKilograms(areaDensity.ToKilogramsPerSquareMeter() * area.ToSquareMeters());
    public static Mass operator *(Area area, AreaDensity areaDensity) => Mass.FromKilograms(area.ToSquareMeters() * areaDensity.ToKilogramsPerSquareMeter());

    public override string ToString() => $"{kilogramsPerSquareMeter} kg/m²";
}
