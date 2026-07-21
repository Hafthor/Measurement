namespace com.hafthor.Measurement;

[Measurement("Wb")]
public readonly partial struct MagneticFlux {

    // SI units
    public static MagneticFlux FromWebers(double webers) => new(webers);
    public double ToWebers() => value;
    public static MagneticFlux FromMilliwebers(double milliwebers) => new(milliwebers * 1e-3);
    public double ToMilliwebers() => value / 1e-3;
    public static MagneticFlux FromMicrowebers(double microwebers) => new(microwebers * 1e-6);
    public double ToMicrowebers() => value / 1e-6;

    // CGS units
    public static MagneticFlux FromMaxwells(double maxwells) => new(maxwells * 1e-8);
    public double ToMaxwells() => value / 1e-8;

    // Composite relationships
    public static Voltage operator /(MagneticFlux flux, Duration duration) => Voltage.FromVolts(flux.value / duration.ToSeconds());
    public static Inductance operator /(MagneticFlux flux, ElectricCurrent current) => Inductance.FromHenries(flux.value / current.ToAmperes());
    public static MagneticFluxDensity operator /(MagneticFlux flux, Area area) => MagneticFluxDensity.FromTeslas(flux.value / area.ToSquareMeters());

}
