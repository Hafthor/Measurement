namespace com.hafthor.Measurement;

public sealed class MagneticFluxDensity : Measurement<MagneticFluxDensity> {

    private MagneticFluxDensity(double value) : base(value) { }

    protected override MagneticFluxDensity Create(double value) => new(value);
    protected override string Symbol => "T";

    // SI units
    public static MagneticFluxDensity FromTeslas(double value) => new(value);
    public double ToTeslas() => value;
    public static MagneticFluxDensity FromMilliteslas(double milliteslas) => new(milliteslas * 1e-3);
    public double ToMilliteslas() => value / 1e-3;
    public static MagneticFluxDensity FromMicroteslas(double microteslas) => new(microteslas * 1e-6);
    public double ToMicroteslas() => value / 1e-6;
    public static MagneticFluxDensity FromNanoteslas(double nanoteslas) => new(nanoteslas * 1e-9);
    public double ToNanoteslas() => value / 1e-9;

    // CGS units
    public static MagneticFluxDensity FromGauss(double gauss) => new(gauss * 1e-4);
    public double ToGauss() => value / 1e-4;
    public static MagneticFluxDensity FromMilligauss(double milligauss) => new(milligauss * 1e-7);
    public double ToMilligauss() => value / 1e-7;

    // Composite relationships
    public static MagneticFlux operator *(MagneticFluxDensity density, Area area) => MagneticFlux.FromWebers(density.value * area.ToSquareMeters());

}
