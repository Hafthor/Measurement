namespace com.hafthor.Measurement;

public class CurrentDensity {
    private readonly double amperesPerSquareMeter;

    private CurrentDensity(double amperesPerSquareMeter) => this.amperesPerSquareMeter = amperesPerSquareMeter;

    // Arithmetic
    public static CurrentDensity operator +(CurrentDensity a, CurrentDensity b) => new CurrentDensity(a.amperesPerSquareMeter + b.amperesPerSquareMeter);
    public static CurrentDensity operator -(CurrentDensity a, CurrentDensity b) => new CurrentDensity(a.amperesPerSquareMeter - b.amperesPerSquareMeter);
    public static CurrentDensity operator -(CurrentDensity x) => new CurrentDensity(-x.amperesPerSquareMeter);

    // Units
    public static CurrentDensity FromAmperesPerSquareMeter(double amperesPerSquareMeter) => new CurrentDensity(amperesPerSquareMeter);
    public double ToAmperesPerSquareMeter() => amperesPerSquareMeter;
    public static CurrentDensity FromAmperesPerSquareCentimeter(double amperesPerSquareCentimeter) => new CurrentDensity(amperesPerSquareCentimeter * (1e4));
    public double ToAmperesPerSquareCentimeter() => amperesPerSquareMeter / (1e4);

    // Composite relationships
    public static ElectricCurrent operator *(CurrentDensity currentDensity, Area area) => ElectricCurrent.FromAmperes(currentDensity.ToAmperesPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCurrent operator *(Area area, CurrentDensity currentDensity) => ElectricCurrent.FromAmperes(area.ToSquareMeters() * currentDensity.ToAmperesPerSquareMeter());
}
