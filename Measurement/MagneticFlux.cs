namespace com.hafthor.Measurement;

[Measurement("Wb", VariableName = "nanowebers", DisplayFactor = 1e9)]
public readonly partial struct MagneticFlux {
    // Canonical (stored) unit is the nanoweber, so nWb/µWb/mWb-scale values land on exact
    // integers in IEEE-754; ToString presents webers (DisplayFactor = 1e9).
    public static MagneticFlux FromWebers(double webers) => new(webers * 1e9);
    public double ToWebers() => nanowebers / 1e9;
    public static MagneticFlux FromMilliwebers(double milliwebers) => new(milliwebers * 1e6);
    public double ToMilliwebers() => nanowebers / 1e6;
    public static MagneticFlux FromMicrowebers(double microwebers) => new(microwebers * 1e3);
    public double ToMicrowebers() => nanowebers / 1e3;
    public static MagneticFlux FromNanowebers(double nanowebers) => new(nanowebers);
    public double ToNanowebers() => nanowebers;

    // CGS units
    public static MagneticFlux FromMaxwells(double maxwells) => new(maxwells * 10);
    public double ToMaxwells() => nanowebers / 10;

    // Composite relationships
    public static Voltage operator /(MagneticFlux flux, Duration duration) => Voltage.FromVolts(flux.ToWebers() / duration.ToSeconds());
    public static Inductance operator /(MagneticFlux flux, ElectricCurrent current) => Inductance.FromHenries(flux.ToWebers() / current.ToAmperes());
    public static MagneticFluxDensity operator /(MagneticFlux flux, Area area) => MagneticFluxDensity.FromTeslas(flux.ToWebers() / area.ToSquareMeters());
    public static LinearMagneticFluxDensity operator /(MagneticFlux flux, Length length) =>
        LinearMagneticFluxDensity.FromWebersPerMeter(flux.ToWebers() / length.ToMeters());
}
