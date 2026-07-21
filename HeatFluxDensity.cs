namespace com.hafthor.Measurement;

public class HeatFluxDensity {
    private readonly double wattsPerSquareMeter;

    private HeatFluxDensity(double wattsPerSquareMeter) => this.wattsPerSquareMeter = wattsPerSquareMeter;

    // Arithmetic
    public static HeatFluxDensity operator +(HeatFluxDensity a, HeatFluxDensity b) => new(a.wattsPerSquareMeter + b.wattsPerSquareMeter);
    public static HeatFluxDensity operator -(HeatFluxDensity a, HeatFluxDensity b) => new(a.wattsPerSquareMeter - b.wattsPerSquareMeter);
    public static HeatFluxDensity operator -(HeatFluxDensity x) => new(-x.wattsPerSquareMeter);

    // Units
    public static HeatFluxDensity FromWattsPerSquareMeter(double wattsPerSquareMeter) => new(wattsPerSquareMeter);
    public double ToWattsPerSquareMeter() => wattsPerSquareMeter;
    public static HeatFluxDensity FromMilliwattsPerSquareMeter(double milliwattsPerSquareMeter) => new(milliwattsPerSquareMeter * (1e-3));
    public double ToMilliwattsPerSquareMeter() => wattsPerSquareMeter / (1e-3);
    public static HeatFluxDensity FromWattsPerSquareCentimeter(double wattsPerSquareCentimeter) => new(wattsPerSquareCentimeter * (1e4));
    public double ToWattsPerSquareCentimeter() => wattsPerSquareMeter / (1e4);

    // Composite relationships
    public static Power operator *(HeatFluxDensity heatFluxDensity, Area area) => Power.FromWatts(heatFluxDensity.ToWattsPerSquareMeter() * area.ToSquareMeters());
    public static Power operator *(Area area, HeatFluxDensity heatFluxDensity) => Power.FromWatts(area.ToSquareMeters() * heatFluxDensity.ToWattsPerSquareMeter());

    public override string ToString() => $"{wattsPerSquareMeter} W/m²";
}
