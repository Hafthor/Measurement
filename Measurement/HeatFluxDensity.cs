namespace com.hafthor.Measurement;

[Measurement("W/m²", VariableName = "milliwattsPerSquareMeter", DisplayFactor = 1e3)]
[SiUnit("WattsPerSquareMeter", 3, "None Milli", PerPrefixes = "None Centi Milli")]
public readonly partial struct HeatFluxDensity {
    // Unit From/To methods are generated from the [SiUnit] declaration above (numerator ×
    // denominator prefixes → WattsPerSquareMeter/Centimeter/Millimeter and the Milliwatt variants).

    // Composite relationships
    public static Power operator *(HeatFluxDensity heatFluxDensity, Area area) => Power.FromWatts(heatFluxDensity.ToWattsPerSquareMeter() * area.ToSquareMeters());
    public static Power operator *(Area area, HeatFluxDensity heatFluxDensity) => Power.FromWatts(area.ToSquareMeters() * heatFluxDensity.ToWattsPerSquareMeter());
}
