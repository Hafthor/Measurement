namespace com.hafthor.Measurement;

[Measurement("m³/kg")]
public readonly partial struct SpecificVolume {

    // Units
    public static SpecificVolume FromCubicMetersPerKilogram(double cubicMetersPerKilogram) => new(cubicMetersPerKilogram);
    public double ToCubicMetersPerKilogram() => value;
    public static SpecificVolume FromLitersPerKilogram(double litersPerKilogram) => new(litersPerKilogram * (1e-3));
    public double ToLitersPerKilogram() => value / (1e-3);
    public static SpecificVolume FromCubicCentimetersPerGram(double cubicCentimetersPerGram) => new(cubicCentimetersPerGram * (1e-3));
    public double ToCubicCentimetersPerGram() => value / (1e-3);

    // Composite relationships
    public static Volume operator *(SpecificVolume specificVolume, Mass mass) => Volume.FromCubicMeters(specificVolume.ToCubicMetersPerKilogram() * mass.ToKilograms());
    public static Volume operator *(Mass mass, SpecificVolume specificVolume) => Volume.FromCubicMeters(mass.ToKilograms() * specificVolume.ToCubicMetersPerKilogram());

}
