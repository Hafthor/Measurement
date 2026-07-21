namespace com.hafthor.Measurement;

public class LuminousFlux {
    private readonly double lumens;

    private LuminousFlux(double lumens) => this.lumens = lumens;

    // Arithmetic
    public static LuminousFlux operator +(LuminousFlux a, LuminousFlux b) => new LuminousFlux(a.lumens + b.lumens);
    public static LuminousFlux operator -(LuminousFlux a, LuminousFlux b) => new LuminousFlux(a.lumens - b.lumens);
    public static LuminousFlux operator -(LuminousFlux x) => new LuminousFlux(-x.lumens);

    // SI units
    public static LuminousFlux FromKilolumens(double kilolumens) => new LuminousFlux(kilolumens * 1e3);
    public double ToKilolumens() => lumens / 1e3;
    public static LuminousFlux FromLumens(double lumens) => new LuminousFlux(lumens);
    public double ToLumens() => lumens;
    public static LuminousFlux FromMillilumens(double millilumens) => new LuminousFlux(millilumens * 1e-3);
    public double ToMillilumens() => lumens / 1e-3;

    // Composite relationships
    public static LuminousIntensity operator /(LuminousFlux flux, SolidAngle solidAngle) => LuminousIntensity.FromCandelas(flux.lumens / solidAngle.ToSteradians());
    public static Illuminance operator /(LuminousFlux flux, Area area) => Illuminance.FromLux(flux.lumens / area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousEnergy operator *(LuminousFlux luminousFlux, Duration duration) => LuminousEnergy.FromLumenSeconds(luminousFlux.ToLumens() * duration.ToSeconds());
}
