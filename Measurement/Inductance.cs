namespace com.hafthor.Measurement;

[Measurement("H", DisplayFactor = 1e9)]
public readonly partial struct Inductance {

    // Canonical (stored) unit is the nanohenry, so nH/µH/mH-scale values land on exact
    // integers in IEEE-754; ToString presents henries (DisplayFactor = 1e9).
    public static Inductance FromHenries(double henries) => new(henries * 1e9);
    public double ToHenries() => value / 1e9;
    public static Inductance FromMillihenries(double millihenries) => new(millihenries * 1e6);
    public double ToMillihenries() => value / 1e6;
    public static Inductance FromMicrohenries(double microhenries) => new(microhenries * 1e3);
    public double ToMicrohenries() => value / 1e3;
    public static Inductance FromNanohenries(double nanohenries) => new(nanohenries);
    public double ToNanohenries() => value;

    // CGS units
    public static Inductance FromAbhenries(double abhenries) => new(abhenries);
    public double ToAbhenries() => value;
    public static Inductance FromStathenries(double stathenries) => new(stathenries * 8.987551787368176e20);
    public double ToStathenries() => value / 8.987551787368176e20;

    // Composite relationships
    public static MagneticFlux operator *(Inductance inductance, ElectricCurrent current) => MagneticFlux.FromWebers(inductance.ToHenries() * current.ToAmperes());

    // Composite relationships (derived)
    public static Permeability operator /(Inductance inductance, Length length) => Permeability.FromHenriesPerMeter(inductance.ToHenries() / length.ToMeters());

}
