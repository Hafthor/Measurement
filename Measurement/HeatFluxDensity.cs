namespace com.hafthor.Measurement;

[Measurement("W/m²", VariableName = "milliwattsPerSquareMeter", DisplayFactor = 1e3)]
[SiUnit("WattsPerSquareMeter", 3, "None Milli", PerPrefixes = "None Centi Milli")]
public readonly partial struct HeatFluxDensity {
    // denominator prefixes → WattsPerSquareMeter/Centimeter/Millimeter and the Milliwatt variants).

}
