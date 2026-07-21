namespace com.hafthor.Measurement;

public class MagneticFlux {
    private readonly double webers;

    private MagneticFlux(double webers) => this.webers = webers;

    // Arithmetic
    public static MagneticFlux operator +(MagneticFlux a, MagneticFlux b) => new(a.webers + b.webers);
    public static MagneticFlux operator -(MagneticFlux a, MagneticFlux b) => new(a.webers - b.webers);
    public static MagneticFlux operator -(MagneticFlux x) => new(-x.webers);

    // SI units
    public static MagneticFlux FromWebers(double webers) => new(webers);
    public double ToWebers() => webers;
    public static MagneticFlux FromMilliwebers(double milliwebers) => new(milliwebers * 1e-3);
    public double ToMilliwebers() => webers / 1e-3;
    public static MagneticFlux FromMicrowebers(double microwebers) => new(microwebers * 1e-6);
    public double ToMicrowebers() => webers / 1e-6;

    // CGS units
    public static MagneticFlux FromMaxwells(double maxwells) => new(maxwells * 1e-8);
    public double ToMaxwells() => webers / 1e-8;

    // Composite relationships
    public static Voltage operator /(MagneticFlux flux, Duration duration) => Voltage.FromVolts(flux.webers / duration.ToSeconds());
    public static Inductance operator /(MagneticFlux flux, ElectricCurrent current) => Inductance.FromHenries(flux.webers / current.ToAmperes());
    public static MagneticFluxDensity operator /(MagneticFlux flux, Area area) => MagneticFluxDensity.FromTeslas(flux.webers / area.ToSquareMeters());
}
