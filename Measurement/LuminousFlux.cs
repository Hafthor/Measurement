namespace com.hafthor.Measurement;

[Measurement("lm", VariableName = "millilumens", DisplayFactor = 1e3)]
public readonly partial struct LuminousFlux {
    // SI units
    public static LuminousFlux FromKilolumens(double kilolumens) => new(kilolumens * 1e6);
    public double ToKilolumens() => millilumens / 1e6;
    public static LuminousFlux FromLumens(double lumens) => new(lumens * 1e3);
    public double ToLumens() => millilumens / 1e3;
    public static LuminousFlux FromMillilumens(double millilumens) => new(millilumens);
    public double ToMillilumens() => millilumens;

    // Composite relationships
    public static LuminousIntensity operator /(LuminousFlux flux, SolidAngle solidAngle) => LuminousIntensity.FromCandelas(flux.ToLumens() / solidAngle.ToSteradians());
    public static Illuminance operator /(LuminousFlux flux, Area area) => Illuminance.FromLux(flux.ToLumens() / area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousEnergy operator *(LuminousFlux luminousFlux, Duration duration) => LuminousEnergy.FromLumenSeconds(luminousFlux.ToLumens() * duration.ToSeconds());
}
