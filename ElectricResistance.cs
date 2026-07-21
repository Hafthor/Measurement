namespace com.hafthor.Measurement;

public class ElectricResistance {
    private readonly double ohms;

    private ElectricResistance(double ohms) => this.ohms = ohms;

    // Arithmetic
    public static ElectricResistance operator +(ElectricResistance a, ElectricResistance b) => new ElectricResistance(a.ohms + b.ohms);
    public static ElectricResistance operator -(ElectricResistance a, ElectricResistance b) => new ElectricResistance(a.ohms - b.ohms);
    public static ElectricResistance operator -(ElectricResistance x) => new ElectricResistance(-x.ohms);

    // SI units
    public static ElectricResistance FromGigaohms(double gigaohms) => new ElectricResistance(gigaohms * 1e9);
    public double ToGigaohms() => ohms / 1e9;
    public static ElectricResistance FromMegaohms(double megaohms) => new ElectricResistance(megaohms * 1e6);
    public double ToMegaohms() => ohms / 1e6;
    public static ElectricResistance FromKiloohms(double kiloohms) => new ElectricResistance(kiloohms * 1e3);
    public double ToKiloohms() => ohms / 1e3;
    public static ElectricResistance FromOhms(double ohms) => new ElectricResistance(ohms);
    public double ToOhms() => ohms;
    public static ElectricResistance FromMilliohms(double milliohms) => new ElectricResistance(milliohms * 1e-3);
    public double ToMilliohms() => ohms / 1e-3;
    public static ElectricResistance FromMicroohms(double microohms) => new ElectricResistance(microohms * 1e-6);
    public double ToMicroohms() => ohms / 1e-6;

    // Composite relationships
    public static Voltage operator *(ElectricResistance resistance, ElectricCurrent current) => Voltage.FromVolts(resistance.ohms * current.ToAmperes());

    // Composite relationships (derived)
    public static Resistivity operator *(ElectricResistance electricResistance, Length length) => Resistivity.FromOhmMeters(electricResistance.ToOhms() * length.ToMeters());
}
