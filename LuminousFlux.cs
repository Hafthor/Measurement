namespace com.hafthor.Measurement;

public class LuminousFlux {
    private readonly double lumens;

    private LuminousFlux(double lumens) => this.lumens = lumens;

    // Arithmetic
    public static LuminousFlux operator +(LuminousFlux a, LuminousFlux b) => new(a.lumens + b.lumens);
    public static LuminousFlux operator -(LuminousFlux a, LuminousFlux b) => new(a.lumens - b.lumens);
    public static LuminousFlux operator -(LuminousFlux x) => new(-x.lumens);

    // SI units
    public static LuminousFlux FromKilolumens(double kilolumens) => new(kilolumens * 1e3);
    public double ToKilolumens() => lumens / 1e3;
    public static LuminousFlux FromLumens(double lumens) => new(lumens);
    public double ToLumens() => lumens;
    public static LuminousFlux FromMillilumens(double millilumens) => new(millilumens * 1e-3);
    public double ToMillilumens() => lumens / 1e-3;

    // Composite relationships
    public static LuminousIntensity operator /(LuminousFlux flux, SolidAngle solidAngle) => LuminousIntensity.FromCandelas(flux.lumens / solidAngle.ToSteradians());
    public static Illuminance operator /(LuminousFlux flux, Area area) => Illuminance.FromLux(flux.lumens / area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousEnergy operator *(LuminousFlux luminousFlux, Duration duration) => LuminousEnergy.FromLumenSeconds(luminousFlux.ToLumens() * duration.ToSeconds());
}
