namespace com.hafthor.Measurement;

[Measurement("cd", VariableName = "millicandelas", DisplayFactor = 1e3)]
[SiUnit("Candelas", 3, "None Kilo Milli")]
[Unit("Candlepower", 0.981e3)]
[Unit("Hefnerkerze", 0.903e3)]
[Unit("Carcels", 9.74e3)]
public readonly partial struct LuminousIntensity {
    // Composite relationships
    public static LuminousFlux operator *(LuminousIntensity intensity, SolidAngle solidAngle) => LuminousFlux.FromLumens(intensity.ToCandelas() * solidAngle.ToSteradians());

    // Composite relationships (derived)
    public static Luminance operator /(LuminousIntensity luminousIntensity, Area area) => Luminance.FromCandelasPerSquareMeter(luminousIntensity.ToCandelas() / area.ToSquareMeters());
}
