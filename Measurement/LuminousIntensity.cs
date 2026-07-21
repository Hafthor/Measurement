namespace com.hafthor.Measurement;

[Measurement("cd")]
public readonly partial struct LuminousIntensity {

    // SI units
    public static LuminousIntensity FromKilocandelas(double kilocandelas) => new(kilocandelas * 1e3);
    public double ToKilocandelas() => value / 1e3;
    public static LuminousIntensity FromCandelas(double candelas) => new(candelas);
    public double ToCandelas() => value;
    public static LuminousIntensity FromMillicandelas(double millicandelas) => new(millicandelas * 1e-3);
    public double ToMillicandelas() => value / 1e-3;

    // Historical units
    public static LuminousIntensity FromCandlepower(double candlepower) => new(candlepower * 0.981);
    public double ToCandlepower() => value / 0.981;
    public static LuminousIntensity FromHefnerkerze(double hefnerkerze) => new(hefnerkerze * 0.903);
    public double ToHefnerkerze() => value / 0.903;
    public static LuminousIntensity FromCarcels(double carcels) => new(carcels * 9.74);
    public double ToCarcels() => value / 9.74;

    // Composite relationships
    public static LuminousFlux operator *(LuminousIntensity intensity, SolidAngle solidAngle) => LuminousFlux.FromLumens(intensity.value * solidAngle.ToSteradians());

    // Composite relationships (derived)
    public static Luminance operator /(LuminousIntensity luminousIntensity, Area area) => Luminance.FromCandelasPerSquareMeter(luminousIntensity.ToCandelas() / area.ToSquareMeters());

}
