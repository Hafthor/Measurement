namespace com.hafthor.Measurement;

public class CurrentDensity {
    private readonly double amperesPerSquareMeter;

    private CurrentDensity(double amperesPerSquareMeter) => this.amperesPerSquareMeter = amperesPerSquareMeter;

    // Arithmetic
    public static CurrentDensity operator +(CurrentDensity a, CurrentDensity b) => new(a.amperesPerSquareMeter + b.amperesPerSquareMeter);
    public static CurrentDensity operator -(CurrentDensity a, CurrentDensity b) => new(a.amperesPerSquareMeter - b.amperesPerSquareMeter);
    public static CurrentDensity operator -(CurrentDensity x) => new(-x.amperesPerSquareMeter);

    // Units
    public static CurrentDensity FromAmperesPerSquareMeter(double amperesPerSquareMeter) => new(amperesPerSquareMeter);
    public double ToAmperesPerSquareMeter() => amperesPerSquareMeter;
    public static CurrentDensity FromAmperesPerSquareCentimeter(double amperesPerSquareCentimeter) => new(amperesPerSquareCentimeter * (1e4));
    public double ToAmperesPerSquareCentimeter() => amperesPerSquareMeter / (1e4);

    // Composite relationships
    public static ElectricCurrent operator *(CurrentDensity currentDensity, Area area) => ElectricCurrent.FromAmperes(currentDensity.ToAmperesPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCurrent operator *(Area area, CurrentDensity currentDensity) => ElectricCurrent.FromAmperes(area.ToSquareMeters() * currentDensity.ToAmperesPerSquareMeter());

    public override string ToString() => $"{amperesPerSquareMeter} A/m²";

    public override bool Equals(object obj) => obj is CurrentDensity other && other.amperesPerSquareMeter == amperesPerSquareMeter;
    public override int GetHashCode() => amperesPerSquareMeter.GetHashCode();
}
