namespace com.hafthor.Measurement;

[Measurement("Wb", VariableName = "nanowebers", DisplayFactor = 1e9)]
[SiUnit("Webers", 9, "None Milli Micro Nano")]
[SiUnit("Maxwells", 1)]
public readonly partial struct MagneticFlux {
    // Composite relationships
    public static Voltage operator /(MagneticFlux flux, Duration duration) => Voltage.FromVolts(flux.ToWebers() / duration.ToSeconds());
    public static Inductance operator /(MagneticFlux flux, ElectricCurrent current) => Inductance.FromHenries(flux.ToWebers() / current.ToAmperes());
    public static MagneticFluxDensity operator /(MagneticFlux flux, Area area) => MagneticFluxDensity.FromTeslas(flux.ToWebers() / area.ToSquareMeters());
    public static LinearMagneticFluxDensity operator /(MagneticFlux flux, Length length) =>
        LinearMagneticFluxDensity.FromWebersPerMeter(flux.ToWebers() / length.ToMeters());
}
