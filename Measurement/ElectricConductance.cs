namespace com.hafthor.Measurement;

[Measurement("S", DisplayFactor = 1e9)]
public readonly partial struct ElectricConductance {

    // Canonical (stored) unit is the nanosiemens, so nS/µS/mS-scale values land on exact
    // integers in IEEE-754; ToString presents siemens (DisplayFactor = 1e9).
    public static ElectricConductance FromKilosiemens(double kilosiemens) => new(kilosiemens * 1e12);
    public double ToKilosiemens() => value / 1e12;
    public static ElectricConductance FromSiemens(double siemens) => new(siemens * 1e9);
    public double ToSiemens() => value / 1e9;
    public static ElectricConductance FromMillisiemens(double millisiemens) => new(millisiemens * 1e6);
    public double ToMillisiemens() => value / 1e6;
    public static ElectricConductance FromMicrosiemens(double microsiemens) => new(microsiemens * 1e3);
    public double ToMicrosiemens() => value / 1e3;
    public static ElectricConductance FromNanosiemens(double nanosiemens) => new(nanosiemens);
    public double ToNanosiemens() => value;

    // Legacy synonym (mho = siemens)
    public static ElectricConductance FromMhos(double mhos) => new(mhos * 1e9);
    public double ToMhos() => value / 1e9;

    // Composite relationships (derived)
    public static Conductivity operator /(ElectricConductance electricConductance, Length length) => Conductivity.FromSiemensPerMeter(electricConductance.ToSiemens() / length.ToMeters());

    // Reciprocal quantity (resistance R = 1/G)
    public ElectricResistance ToElectricResistance() => ElectricResistance.FromOhms(1.0 / ToSiemens());

}
