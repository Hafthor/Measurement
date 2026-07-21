namespace com.hafthor.Measurement;

public class LuminousIntensity {
    private readonly double candelas;

    private LuminousIntensity(double candelas) => this.candelas = candelas;

    // Arithmetic
    public static LuminousIntensity operator +(LuminousIntensity a, LuminousIntensity b) => new LuminousIntensity(a.candelas + b.candelas);
    public static LuminousIntensity operator -(LuminousIntensity a, LuminousIntensity b) => new LuminousIntensity(a.candelas - b.candelas);
    public static LuminousIntensity operator -(LuminousIntensity x) => new LuminousIntensity(-x.candelas);

    // SI units
    public static LuminousIntensity FromKilocandelas(double kilocandelas) => new LuminousIntensity(kilocandelas * 1e3);
    public double ToKilocandelas() => candelas / 1e3;
    public static LuminousIntensity FromCandelas(double candelas) => new LuminousIntensity(candelas);
    public double ToCandelas() => candelas;
    public static LuminousIntensity FromMillicandelas(double millicandelas) => new LuminousIntensity(millicandelas * 1e-3);
    public double ToMillicandelas() => candelas / 1e-3;

    // Historical units
    public static LuminousIntensity FromCandlepower(double candlepower) => new LuminousIntensity(candlepower * 0.981);
    public double ToCandlepower() => candelas / 0.981;
    public static LuminousIntensity FromHefnerkerze(double hefnerkerze) => new LuminousIntensity(hefnerkerze * 0.903);
    public double ToHefnerkerze() => candelas / 0.903;
    public static LuminousIntensity FromCarcels(double carcels) => new LuminousIntensity(carcels * 9.74);
    public double ToCarcels() => candelas / 9.74;

    // Composite relationships
    public static LuminousFlux operator *(LuminousIntensity intensity, SolidAngle solidAngle) => LuminousFlux.FromLumens(intensity.candelas * solidAngle.ToSteradians());

    // Composite relationships (derived)
    public static Luminance operator /(LuminousIntensity luminousIntensity, Area area) => Luminance.FromCandelasPerSquareMeter(luminousIntensity.ToCandelas() / area.ToSquareMeters());
}
