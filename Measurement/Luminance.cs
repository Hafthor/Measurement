namespace com.hafthor.Measurement;

[Measurement("cd/m²", VariableName = "nits")]
[SiUnit("CandelasPerSquareMeter", 0)]
[SiUnit("Nits", 0)]
[SiUnit("Stilbs", 4)]
[Unit("FootLamberts", 3.4262590996)]
public readonly partial struct Luminance {
    // Composite relationships
    public static LuminousIntensity operator *(Luminance luminance, Area area) => LuminousIntensity.FromCandelas(luminance.ToCandelasPerSquareMeter() * area.ToSquareMeters());
    public static LuminousIntensity operator *(Area area, Luminance luminance) => LuminousIntensity.FromCandelas(area.ToSquareMeters() * luminance.ToCandelasPerSquareMeter());
}
