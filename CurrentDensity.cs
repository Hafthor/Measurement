namespace com.hafthor.Measurement;

public sealed class CurrentDensity : Measurement<CurrentDensity> {

    private CurrentDensity(double value) : base(value) { }

    protected override CurrentDensity Create(double value) => new(value);
    protected override string Symbol => "A/m²";

    // Units
    public static CurrentDensity FromAmperesPerSquareMeter(double value) => new(value);
    public double ToAmperesPerSquareMeter() => value;
    public static CurrentDensity FromAmperesPerSquareCentimeter(double amperesPerSquareCentimeter) => new(amperesPerSquareCentimeter * (1e4));
    public double ToAmperesPerSquareCentimeter() => value / (1e4);

    // Composite relationships
    public static ElectricCurrent operator *(CurrentDensity currentDensity, Area area) => ElectricCurrent.FromAmperes(currentDensity.ToAmperesPerSquareMeter() * area.ToSquareMeters());
    public static ElectricCurrent operator *(Area area, CurrentDensity currentDensity) => ElectricCurrent.FromAmperes(area.ToSquareMeters() * currentDensity.ToAmperesPerSquareMeter());

}
