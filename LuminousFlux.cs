namespace com.hafthor.Measurement;

[Measurement("lm")]
public readonly partial struct LuminousFlux {

    // SI units
    public static LuminousFlux FromKilolumens(double kilolumens) => new(kilolumens * 1e3);
    public double ToKilolumens() => value / 1e3;
    public static LuminousFlux FromLumens(double lumens) => new(lumens);
    public double ToLumens() => value;
    public static LuminousFlux FromMillilumens(double millilumens) => new(millilumens * 1e-3);
    public double ToMillilumens() => value / 1e-3;

    // Composite relationships
    public static LuminousIntensity operator /(LuminousFlux flux, SolidAngle solidAngle) => LuminousIntensity.FromCandelas(flux.value / solidAngle.ToSteradians());
    public static Illuminance operator /(LuminousFlux flux, Area area) => Illuminance.FromLux(flux.value / area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousEnergy operator *(LuminousFlux luminousFlux, Duration duration) => LuminousEnergy.FromLumenSeconds(luminousFlux.ToLumens() * duration.ToSeconds());

}
