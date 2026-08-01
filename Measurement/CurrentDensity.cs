namespace com.hafthor.Measurement;

[Measurement("A/m²", VariableName = "amperesPerSquareMeter")]
[SiUnit("AmperesPerSquareMeter", 0)]
[SiUnit("AmperesPerSquareCentimeter", 4)]
public readonly partial struct CurrentDensity {
    // Composite relationships
    public static ElectricCurrent operator *(CurrentDensity currentDensity, Area area) => ElectricCurrent.FromAmperes(currentDensity.ToAmperesPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCurrent operator *(Area area, CurrentDensity currentDensity) => ElectricCurrent.FromAmperes(area.ToSquareMeters() * currentDensity.ToAmperesPerSquareMeter());
}
