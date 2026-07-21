namespace com.hafthor.Measurement;

public sealed class LuminousFlux : Measurement<LuminousFlux> {

    private LuminousFlux(double value) : base(value) { }

    protected override LuminousFlux Create(double value) => new(value);
    protected override string Symbol => "lm";

    // SI units
    public static LuminousFlux FromKilolumens(double kilolumens) => new(kilolumens * 1e3);
    public double ToKilolumens() => value / 1e3;
    public static LuminousFlux FromLumens(double value) => new(value);
    public double ToLumens() => value;
    public static LuminousFlux FromMillilumens(double millilumens) => new(millilumens * 1e-3);
    public double ToMillilumens() => value / 1e-3;

    // Composite relationships
    public static LuminousIntensity operator /(LuminousFlux flux, SolidAngle solidAngle) => LuminousIntensity.FromCandelas(flux.value / solidAngle.ToSteradians());
    public static Illuminance operator /(LuminousFlux flux, Area area) => Illuminance.FromLux(flux.value / area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousEnergy operator *(LuminousFlux luminousFlux, Duration duration) => LuminousEnergy.FromLumenSeconds(luminousFlux.ToLumens() * duration.ToSeconds());

}
