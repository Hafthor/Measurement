namespace com.hafthor.Measurement;

[Measurement("kg/m²")]
public readonly partial struct AreaDensity {

    // Units
    public static AreaDensity FromKilogramsPerSquareMeter(double kilogramsPerSquareMeter) => new(kilogramsPerSquareMeter);
    public double ToKilogramsPerSquareMeter() => value;
    public static AreaDensity FromGramsPerSquareMeter(double gramsPerSquareMeter) => new(gramsPerSquareMeter * (1e-3));
    public double ToGramsPerSquareMeter() => value / (1e-3);

    // Composite relationships
    public static Mass operator *(AreaDensity areaDensity, Area area) => Mass.FromKilograms(areaDensity.ToKilogramsPerSquareMeter() * area.ToSquareMeters());
    public static Mass operator *(Area area, AreaDensity areaDensity) => Mass.FromKilograms(area.ToSquareMeters() * areaDensity.ToKilogramsPerSquareMeter());

}
