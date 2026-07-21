namespace com.hafthor.Measurement;

public sealed class Inductance : Measurement<Inductance> {

    private Inductance(double value) : base(value) { }

    protected override Inductance Create(double value) => new(value);
    protected override string Symbol => "H";

    // SI units
    public static Inductance FromHenries(double value) => new(value);
    public double ToHenries() => value;
    public static Inductance FromMillihenries(double millihenries) => new(millihenries * 1e-3);
    public double ToMillihenries() => value / 1e-3;
    public static Inductance FromMicrohenries(double microhenries) => new(microhenries * 1e-6);
    public double ToMicrohenries() => value / 1e-6;
    public static Inductance FromNanohenries(double nanohenries) => new(nanohenries * 1e-9);
    public double ToNanohenries() => value / 1e-9;

    // CGS units
    public static Inductance FromAbhenries(double abhenries) => new(abhenries * 1e-9);
    public double ToAbhenries() => value / 1e-9;
    public static Inductance FromStathenries(double stathenries) => new(stathenries * 8.987551787368176e11);
    public double ToStathenries() => value / 8.987551787368176e11;

    // Composite relationships
    public static MagneticFlux operator *(Inductance inductance, ElectricCurrent current) => MagneticFlux.FromWebers(inductance.value * current.ToAmperes());

    // Composite relationships (derived)
    public static Permeability operator /(Inductance inductance, Length length) => Permeability.FromHenriesPerMeter(inductance.ToHenries() / length.ToMeters());

}
