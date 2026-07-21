namespace com.hafthor.Measurement;

[Measurement("S")]
public readonly partial struct ElectricConductance {

    // SI units
    public static ElectricConductance FromKilosiemens(double kilosiemens) => new(kilosiemens * 1e3);
    public double ToKilosiemens() => value / 1e3;
    public static ElectricConductance FromSiemens(double siemens) => new(siemens);
    public double ToSiemens() => value;
    public static ElectricConductance FromMillisiemens(double millisiemens) => new(millisiemens * 1e-3);
    public double ToMillisiemens() => value / 1e-3;
    public static ElectricConductance FromMicrosiemens(double microsiemens) => new(microsiemens * 1e-6);
    public double ToMicrosiemens() => value / 1e-6;

    // Legacy synonym (mho = siemens)
    public static ElectricConductance FromMhos(double mhos) => new(mhos);
    public double ToMhos() => value;

    // Composite relationships (derived)
    public static Conductivity operator /(ElectricConductance electricConductance, Length length) => Conductivity.FromSiemensPerMeter(electricConductance.ToSiemens() / length.ToMeters());

}
