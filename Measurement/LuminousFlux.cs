namespace com.hafthor.Measurement;

[Measurement("lm", VariableName = "millilumens", DisplayFactor = 1e3)]
[SiUnit("Lumens", 3, "None Kilo Milli")]
public readonly partial struct LuminousFlux {
    // Composite relationships
    public static LuminousIntensity operator /(LuminousFlux flux, SolidAngle solidAngle) => LuminousIntensity.FromCandelas(flux.ToLumens() / solidAngle.ToSteradians());
    public static Illuminance operator /(LuminousFlux flux, Area area) => Illuminance.FromLux(flux.ToLumens() / area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousEnergy operator *(LuminousFlux luminousFlux, Duration duration) => LuminousEnergy.FromLumenSeconds(luminousFlux.ToLumens() * duration.ToSeconds());
}
