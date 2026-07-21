namespace com.hafthor.Measurement;

public class Inductance {
    private readonly double henries;

    private Inductance(double henries) => this.henries = henries;

    // Arithmetic
    public static Inductance operator +(Inductance a, Inductance b) => new(a.henries + b.henries);
    public static Inductance operator -(Inductance a, Inductance b) => new(a.henries - b.henries);
    public static Inductance operator -(Inductance x) => new(-x.henries);

    // SI units
    public static Inductance FromHenries(double henries) => new(henries);
    public double ToHenries() => henries;
    public static Inductance FromMillihenries(double millihenries) => new(millihenries * 1e-3);
    public double ToMillihenries() => henries / 1e-3;
    public static Inductance FromMicrohenries(double microhenries) => new(microhenries * 1e-6);
    public double ToMicrohenries() => henries / 1e-6;
    public static Inductance FromNanohenries(double nanohenries) => new(nanohenries * 1e-9);
    public double ToNanohenries() => henries / 1e-9;

    // CGS units
    public static Inductance FromAbhenries(double abhenries) => new(abhenries * 1e-9);
    public double ToAbhenries() => henries / 1e-9;
    public static Inductance FromStathenries(double stathenries) => new(stathenries * 8.987551787368176e11);
    public double ToStathenries() => henries / 8.987551787368176e11;

    // Composite relationships
    public static MagneticFlux operator *(Inductance inductance, ElectricCurrent current) => MagneticFlux.FromWebers(inductance.henries * current.ToAmperes());

    // Composite relationships (derived)
    public static Permeability operator /(Inductance inductance, Length length) => Permeability.FromHenriesPerMeter(inductance.ToHenries() / length.ToMeters());

    public override string ToString() => $"{henries} H";

    public override bool Equals(object obj) => obj is Inductance other && other.henries == henries;
    public override int GetHashCode() => henries.GetHashCode();
}
