namespace com.hafthor.Measurement;

[Measurement("m³/g", VariableName = "cubicMillimetersPerGram", DisplayFactor = 1e9)]
[SiUnit("CubicMetersPerKilogram", 6)]
[SiUnit("LitersPerKilogram", 3)]
[SiUnit("CubicCentimetersPerGram", 3)]
public readonly partial struct SpecificVolume {
    // Composite relationships
    public static Volume operator *(SpecificVolume specificVolume, Mass mass) => Volume.FromCubicMeters(specificVolume.ToCubicMetersPerKilogram() * mass.ToKilograms());
    public static Volume operator *(Mass mass, SpecificVolume specificVolume) => Volume.FromCubicMeters(mass.ToKilograms() * specificVolume.ToCubicMetersPerKilogram());
}
