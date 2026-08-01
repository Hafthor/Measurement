namespace com.hafthor.Measurement;

[Measurement("g/m²", VariableName = "gramsPerSquareMeter")]
[SiUnit("GramsPerSquareMeter", 0, "None Kilo")]
public readonly partial struct AreaDensity {
    // Composite relationships
    public static Mass operator *(AreaDensity areaDensity, Area area) => Mass.FromKilograms(areaDensity.ToKilogramsPerSquareMeter() * area.ToSquareMeters());
    public static Mass operator *(Area area, AreaDensity areaDensity) => Mass.FromKilograms(area.ToSquareMeters() * areaDensity.ToKilogramsPerSquareMeter());
}
