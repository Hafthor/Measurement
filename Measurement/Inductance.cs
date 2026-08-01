namespace com.hafthor.Measurement;

[Measurement("H", VariableName = "nanohenries", DisplayFactor = 1e9)]
[SiUnit("Henries", 9, "None Milli Micro Nano")]
[SiUnit("Abhenries", 0)]
[Unit("Stathenries", 8.987551787368176e20)]
public readonly partial struct Inductance {
    // Composite relationships
    public static MagneticFlux operator *(Inductance inductance, ElectricCurrent current) => MagneticFlux.FromWebers(inductance.ToHenries() * current.ToAmperes());

    // Composite relationships (derived)
    public static Permeability operator /(Inductance inductance, Length length) => Permeability.FromHenriesPerMeter(inductance.ToHenries() / length.ToMeters());
}
