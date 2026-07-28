namespace com.hafthor.Measurement;

[Measurement("H", VariableName = "nanohenries", DisplayFactor = 1e9)]
public readonly partial struct Inductance {
    // Canonical (stored) unit is the nanohenry, so nH/µH/mH-scale values land on exact
    // integers in IEEE-754; ToString presents henries (DisplayFactor = 1e9).
    public static Inductance FromHenries(double henries) => new(henries * 1e9);
    public double ToHenries() => nanohenries / 1e9;
    public static Inductance FromMillihenries(double millihenries) => new(millihenries * 1e6);
    public double ToMillihenries() => nanohenries / 1e6;
    public static Inductance FromMicrohenries(double microhenries) => new(microhenries * 1e3);
    public double ToMicrohenries() => nanohenries / 1e3;
    public static Inductance FromNanohenries(double nanohenries) => new(nanohenries);
    public double ToNanohenries() => nanohenries;

    // CGS units
    public static Inductance FromAbhenries(double abhenries) => new(abhenries);
    public double ToAbhenries() => nanohenries;
    public static Inductance FromStathenries(double stathenries) => new(stathenries * 8.987551787368176e20);
    public double ToStathenries() => nanohenries / 8.987551787368176e20;

    // Composite relationships
    public static MagneticFlux operator *(Inductance inductance, ElectricCurrent current) => MagneticFlux.FromWebers(inductance.ToHenries() * current.ToAmperes());

    // Composite relationships (derived)
    public static Permeability operator /(Inductance inductance, Length length) => Permeability.FromHenriesPerMeter(inductance.ToHenries() / length.ToMeters());
}
