namespace com.hafthor.Measurement;

[Measurement("cd", VariableName = "millicandelas", DisplayFactor = 1e3)]
public readonly partial struct LuminousIntensity {
    // SI units
    public static LuminousIntensity FromKilocandelas(double kilocandelas) => new(kilocandelas * 1e6);
    public double ToKilocandelas() => millicandelas / 1e6;
    public static LuminousIntensity FromCandelas(double candelas) => new(candelas * 1e3);
    public double ToCandelas() => millicandelas / 1e3;
    public static LuminousIntensity FromMillicandelas(double millicandelas) => new(millicandelas);
    public double ToMillicandelas() => millicandelas;

    // Historical units
    public static LuminousIntensity FromCandlepower(double candlepower) => new(candlepower * 0.981e3);
    public double ToCandlepower() => millicandelas / 0.981e3;
    public static LuminousIntensity FromHefnerkerze(double hefnerkerze) => new(hefnerkerze * 0.903e3);
    public double ToHefnerkerze() => millicandelas / 0.903e3;
    public static LuminousIntensity FromCarcels(double carcels) => new(carcels * 9.74e3);
    public double ToCarcels() => millicandelas / 9.74e3;

    // Composite relationships
    public static LuminousFlux operator *(LuminousIntensity intensity, SolidAngle solidAngle) => LuminousFlux.FromLumens(intensity.ToCandelas() * solidAngle.ToSteradians());

    // Composite relationships (derived)
    public static Luminance operator /(LuminousIntensity luminousIntensity, Area area) => Luminance.FromCandelasPerSquareMeter(luminousIntensity.ToCandelas() / area.ToSquareMeters());
}
