namespace com.hafthor.Measurement;

[Measurement("A/m²", VariableName = "amperesPerSquareMeter")]
public readonly partial struct CurrentDensity {
    // Units
    public static CurrentDensity FromAmperesPerSquareMeter(double amperesPerSquareMeter) => new(amperesPerSquareMeter);
    public double ToAmperesPerSquareMeter() => amperesPerSquareMeter;
    public static CurrentDensity FromAmperesPerSquareCentimeter(double amperesPerSquareCentimeter) => new(amperesPerSquareCentimeter * (1e4));
    public double ToAmperesPerSquareCentimeter() => amperesPerSquareMeter / (1e4);

    // Composite relationships
    public static ElectricCurrent operator *(CurrentDensity currentDensity, Area area) => ElectricCurrent.FromAmperes(currentDensity.ToAmperesPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCurrent operator *(Area area, CurrentDensity currentDensity) => ElectricCurrent.FromAmperes(area.ToSquareMeters() * currentDensity.ToAmperesPerSquareMeter());
}
