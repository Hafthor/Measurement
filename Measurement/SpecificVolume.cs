namespace com.hafthor.Measurement;

[Measurement("m³/g", VariableName = "cubicMillimetersPerGram", DisplayFactor = 1e9)]
public readonly partial struct SpecificVolume {
    // Units
    public static SpecificVolume FromCubicMetersPerKilogram(double cubicMetersPerKilogram) => new(cubicMetersPerKilogram * 1e6);
    public double ToCubicMetersPerKilogram() => cubicMillimetersPerGram / (1e6);
    public static SpecificVolume FromLitersPerKilogram(double litersPerKilogram) => new(litersPerKilogram * (1e3));
    public double ToLitersPerKilogram() => cubicMillimetersPerGram / (1e3);
    public static SpecificVolume FromCubicCentimetersPerGram(double cubicCentimetersPerGram) => new(cubicCentimetersPerGram * (1e3));
    public double ToCubicCentimetersPerGram() => cubicMillimetersPerGram / (1e3);

    // Composite relationships
    public static Volume operator *(SpecificVolume specificVolume, Mass mass) => Volume.FromCubicMeters(specificVolume.ToCubicMetersPerKilogram() * mass.ToKilograms());
    public static Volume operator *(Mass mass, SpecificVolume specificVolume) => Volume.FromCubicMeters(mass.ToKilograms() * specificVolume.ToCubicMetersPerKilogram());
}
