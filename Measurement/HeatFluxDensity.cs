namespace com.hafthor.Measurement;

[Measurement("W/m²", VariableName = "milliwattsPerSquareMeter", DisplayFactor = 1e3)]
public readonly partial struct HeatFluxDensity {
    // Units
    public static HeatFluxDensity FromWattsPerSquareMeter(double wattsPerSquareMeter) => new(wattsPerSquareMeter * 1e3);
    public double ToWattsPerSquareMeter() => milliwattsPerSquareMeter / 1e3;
    public static HeatFluxDensity FromMilliwattsPerSquareMeter(double milliwattsPerSquareMeter) => new(milliwattsPerSquareMeter);
    public double ToMilliwattsPerSquareMeter() => milliwattsPerSquareMeter;
    public static HeatFluxDensity FromWattsPerSquareCentimeter(double wattsPerSquareCentimeter) => new(wattsPerSquareCentimeter * (1e7));
    public double ToWattsPerSquareCentimeter() => milliwattsPerSquareMeter / (1e7);

    // Composite relationships
    public static Power operator *(HeatFluxDensity heatFluxDensity, Area area) => Power.FromWatts(heatFluxDensity.ToWattsPerSquareMeter() * area.ToSquareMeters());
    public static Power operator *(Area area, HeatFluxDensity heatFluxDensity) => Power.FromWatts(area.ToSquareMeters() * heatFluxDensity.ToWattsPerSquareMeter());
}
