namespace com.hafthor.Measurement;

public class MagneticFluxDensity {
    private readonly double teslas;

    private MagneticFluxDensity(double teslas) => this.teslas = teslas;

    // Arithmetic
    public static MagneticFluxDensity operator +(MagneticFluxDensity a, MagneticFluxDensity b) => new(a.teslas + b.teslas);
    public static MagneticFluxDensity operator -(MagneticFluxDensity a, MagneticFluxDensity b) => new(a.teslas - b.teslas);
    public static MagneticFluxDensity operator -(MagneticFluxDensity x) => new(-x.teslas);

    // SI units
    public static MagneticFluxDensity FromTeslas(double teslas) => new(teslas);
    public double ToTeslas() => teslas;
    public static MagneticFluxDensity FromMilliteslas(double milliteslas) => new(milliteslas * 1e-3);
    public double ToMilliteslas() => teslas / 1e-3;
    public static MagneticFluxDensity FromMicroteslas(double microteslas) => new(microteslas * 1e-6);
    public double ToMicroteslas() => teslas / 1e-6;
    public static MagneticFluxDensity FromNanoteslas(double nanoteslas) => new(nanoteslas * 1e-9);
    public double ToNanoteslas() => teslas / 1e-9;

    // CGS units
    public static MagneticFluxDensity FromGauss(double gauss) => new(gauss * 1e-4);
    public double ToGauss() => teslas / 1e-4;
    public static MagneticFluxDensity FromMilligauss(double milligauss) => new(milligauss * 1e-7);
    public double ToMilligauss() => teslas / 1e-7;

    // Composite relationships
    public static MagneticFlux operator *(MagneticFluxDensity density, Area area) => MagneticFlux.FromWebers(density.teslas * area.ToSquareMeters());
}
