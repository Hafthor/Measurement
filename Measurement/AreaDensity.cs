namespace com.hafthor.Measurement;

[Measurement("g/m²", VariableName = "gramsPerSquareMeter")]
public readonly partial struct AreaDensity {
    // Units
    public static AreaDensity FromKilogramsPerSquareMeter(double kilogramsPerSquareMeter) => new(kilogramsPerSquareMeter * 1e3);
    public double ToKilogramsPerSquareMeter() => gramsPerSquareMeter / 1e3;
    public static AreaDensity FromGramsPerSquareMeter(double gramsPerSquareMeter) => new(gramsPerSquareMeter);
    public double ToGramsPerSquareMeter() => gramsPerSquareMeter;

    // Composite relationships
    public static Mass operator *(AreaDensity areaDensity, Area area) => Mass.FromKilograms(areaDensity.ToKilogramsPerSquareMeter() * area.ToSquareMeters());
    public static Mass operator *(Area area, AreaDensity areaDensity) => Mass.FromKilograms(area.ToSquareMeters() * areaDensity.ToKilogramsPerSquareMeter());
}
