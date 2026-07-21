namespace com.hafthor.Measurement;

public class ElectricConductance {
    private readonly double siemens;

    private ElectricConductance(double siemens) => this.siemens = siemens;

    // Arithmetic
    public static ElectricConductance operator +(ElectricConductance a, ElectricConductance b) => new ElectricConductance(a.siemens + b.siemens);
    public static ElectricConductance operator -(ElectricConductance a, ElectricConductance b) => new ElectricConductance(a.siemens - b.siemens);
    public static ElectricConductance operator -(ElectricConductance x) => new ElectricConductance(-x.siemens);

    // SI units
    public static ElectricConductance FromKilosiemens(double kilosiemens) => new ElectricConductance(kilosiemens * 1e3);
    public double ToKilosiemens() => siemens / 1e3;
    public static ElectricConductance FromSiemens(double siemens) => new ElectricConductance(siemens);
    public double ToSiemens() => siemens;
    public static ElectricConductance FromMillisiemens(double millisiemens) => new ElectricConductance(millisiemens * 1e-3);
    public double ToMillisiemens() => siemens / 1e-3;
    public static ElectricConductance FromMicrosiemens(double microsiemens) => new ElectricConductance(microsiemens * 1e-6);
    public double ToMicrosiemens() => siemens / 1e-6;

    // Legacy synonym (mho = siemens)
    public static ElectricConductance FromMhos(double mhos) => new ElectricConductance(mhos);
    public double ToMhos() => siemens;

    // Composite relationships (derived)
    public static Conductivity operator /(ElectricConductance electricConductance, Length length) => Conductivity.FromSiemensPerMeter(electricConductance.ToSiemens() / length.ToMeters());
}
