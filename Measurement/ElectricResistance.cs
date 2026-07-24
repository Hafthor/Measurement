namespace com.hafthor.Measurement;

[Measurement("Ω")]
public readonly partial struct ElectricResistance {

    // SI units
    public static ElectricResistance FromGigaohms(double gigaohms) => new(gigaohms * 1e9);
    public double ToGigaohms() => value / 1e9;
    public static ElectricResistance FromMegaohms(double megaohms) => new(megaohms * 1e6);
    public double ToMegaohms() => value / 1e6;
    public static ElectricResistance FromKiloohms(double kiloohms) => new(kiloohms * 1e3);
    public double ToKiloohms() => value / 1e3;
    public static ElectricResistance FromOhms(double ohms) => new(ohms);
    public double ToOhms() => value;
    public static ElectricResistance FromMilliohms(double milliohms) => new(milliohms * 1e-3);
    public double ToMilliohms() => value / 1e-3;
    public static ElectricResistance FromMicroohms(double microohms) => new(microohms * 1e-6);
    public double ToMicroohms() => value / 1e-6;

    // Composite relationships
    public static Voltage operator *(ElectricResistance resistance, ElectricCurrent current) => Voltage.FromVolts(resistance.value * current.ToAmperes());

    // Composite relationships (derived)
    public static Resistivity operator *(ElectricResistance electricResistance, Length length) => Resistivity.FromOhmMeters(electricResistance.ToOhms() * length.ToMeters());

    // Reciprocal quantity (conductance G = 1/R)
    public ElectricConductance ToElectricConductance() => ElectricConductance.FromSiemens(1.0 / ToOhms());

}
