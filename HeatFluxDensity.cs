namespace com.hafthor.Measurement;

[Measurement("W/m²")]
public readonly partial struct HeatFluxDensity {

    // Units
    public static HeatFluxDensity FromWattsPerSquareMeter(double wattsPerSquareMeter) => new(wattsPerSquareMeter);
    public double ToWattsPerSquareMeter() => value;
    public static HeatFluxDensity FromMilliwattsPerSquareMeter(double milliwattsPerSquareMeter) => new(milliwattsPerSquareMeter * (1e-3));
    public double ToMilliwattsPerSquareMeter() => value / (1e-3);
    public static HeatFluxDensity FromWattsPerSquareCentimeter(double wattsPerSquareCentimeter) => new(wattsPerSquareCentimeter * (1e4));
    public double ToWattsPerSquareCentimeter() => value / (1e4);

    // Composite relationships
    public static Power operator *(HeatFluxDensity heatFluxDensity, Area area) => Power.FromWatts(heatFluxDensity.ToWattsPerSquareMeter() * area.ToSquareMeters());
    public static Power operator *(Area area, HeatFluxDensity heatFluxDensity) => Power.FromWatts(area.ToSquareMeters() * heatFluxDensity.ToWattsPerSquareMeter());

}
