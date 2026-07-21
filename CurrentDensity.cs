namespace com.hafthor.Measurement;

[Measurement("A/m²")]
public readonly partial struct CurrentDensity {

    // Units
    public static CurrentDensity FromAmperesPerSquareMeter(double amperesPerSquareMeter) => new(amperesPerSquareMeter);
    public double ToAmperesPerSquareMeter() => value;
    public static CurrentDensity FromAmperesPerSquareCentimeter(double amperesPerSquareCentimeter) => new(amperesPerSquareCentimeter * (1e4));
    public double ToAmperesPerSquareCentimeter() => value / (1e4);

    // Composite relationships
    public static ElectricCurrent operator *(CurrentDensity currentDensity, Area area) => ElectricCurrent.FromAmperes(currentDensity.ToAmperesPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCurrent operator *(Area area, CurrentDensity currentDensity) => ElectricCurrent.FromAmperes(area.ToSquareMeters() * currentDensity.ToAmperesPerSquareMeter());

}
