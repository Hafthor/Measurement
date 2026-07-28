namespace com.hafthor.Measurement;

[Measurement("T", VariableName = "nanoteslas", DisplayFactor = 1e9)]
public readonly partial struct MagneticFluxDensity {
    // Canonical (stored) unit is the nanotesla, so nT/µT/mT-scale values land on exact
    // integers in IEEE-754; ToString presents teslas (DisplayFactor = 1e9).
    public static MagneticFluxDensity FromTeslas(double teslas) => new(teslas * 1e9);
    public double ToTeslas() => nanoteslas / 1e9;
    public static MagneticFluxDensity FromMilliteslas(double milliteslas) => new(milliteslas * 1e6);
    public double ToMilliteslas() => nanoteslas / 1e6;
    public static MagneticFluxDensity FromMicroteslas(double microteslas) => new(microteslas * 1e3);
    public double ToMicroteslas() => nanoteslas / 1e3;
    public static MagneticFluxDensity FromNanoteslas(double nanoteslas) => new(nanoteslas);
    public double ToNanoteslas() => nanoteslas;

    // CGS units
    public static MagneticFluxDensity FromGauss(double gauss) => new(gauss * 1e5);
    public double ToGauss() => nanoteslas / 1e5;
    public static MagneticFluxDensity FromMilligauss(double milligauss) => new(milligauss * 1e2);
    public double ToMilligauss() => nanoteslas / 1e2;

    // Composite relationships
    public static MagneticFlux operator *(MagneticFluxDensity density, Area area) => MagneticFlux.FromWebers(density.ToTeslas() * area.ToSquareMeters());
}
