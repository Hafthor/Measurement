namespace com.hafthor.Measurement;

public class ElectricConductance {
    private readonly double siemens;

    private ElectricConductance(double siemens) => this.siemens = siemens;

    // Arithmetic
    public static ElectricConductance operator +(ElectricConductance a, ElectricConductance b) => new(a.siemens + b.siemens);
    public static ElectricConductance operator -(ElectricConductance a, ElectricConductance b) => new(a.siemens - b.siemens);
    public static ElectricConductance operator -(ElectricConductance x) => new(-x.siemens);

    // SI units
    public static ElectricConductance FromKilosiemens(double kilosiemens) => new(kilosiemens * 1e3);
    public double ToKilosiemens() => siemens / 1e3;
    public static ElectricConductance FromSiemens(double siemens) => new(siemens);
    public double ToSiemens() => siemens;
    public static ElectricConductance FromMillisiemens(double millisiemens) => new(millisiemens * 1e-3);
    public double ToMillisiemens() => siemens / 1e-3;
    public static ElectricConductance FromMicrosiemens(double microsiemens) => new(microsiemens * 1e-6);
    public double ToMicrosiemens() => siemens / 1e-6;

    // Legacy synonym (mho = siemens)
    public static ElectricConductance FromMhos(double mhos) => new(mhos);
    public double ToMhos() => siemens;

    // Composite relationships (derived)
    public static Conductivity operator /(ElectricConductance electricConductance, Length length) => Conductivity.FromSiemensPerMeter(electricConductance.ToSiemens() / length.ToMeters());
}
