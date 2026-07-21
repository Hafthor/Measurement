namespace com.hafthor.Measurement;

public class ElectricResistance {
    private readonly double ohms;

    private ElectricResistance(double ohms) => this.ohms = ohms;

    // Arithmetic
    public static ElectricResistance operator +(ElectricResistance a, ElectricResistance b) => new(a.ohms + b.ohms);
    public static ElectricResistance operator -(ElectricResistance a, ElectricResistance b) => new(a.ohms - b.ohms);
    public static ElectricResistance operator -(ElectricResistance x) => new(-x.ohms);

    // SI units
    public static ElectricResistance FromGigaohms(double gigaohms) => new(gigaohms * 1e9);
    public double ToGigaohms() => ohms / 1e9;
    public static ElectricResistance FromMegaohms(double megaohms) => new(megaohms * 1e6);
    public double ToMegaohms() => ohms / 1e6;
    public static ElectricResistance FromKiloohms(double kiloohms) => new(kiloohms * 1e3);
    public double ToKiloohms() => ohms / 1e3;
    public static ElectricResistance FromOhms(double ohms) => new(ohms);
    public double ToOhms() => ohms;
    public static ElectricResistance FromMilliohms(double milliohms) => new(milliohms * 1e-3);
    public double ToMilliohms() => ohms / 1e-3;
    public static ElectricResistance FromMicroohms(double microohms) => new(microohms * 1e-6);
    public double ToMicroohms() => ohms / 1e-6;

    // Composite relationships
    public static Voltage operator *(ElectricResistance resistance, ElectricCurrent current) => Voltage.FromVolts(resistance.ohms * current.ToAmperes());

    // Composite relationships (derived)
    public static Resistivity operator *(ElectricResistance electricResistance, Length length) => Resistivity.FromOhmMeters(electricResistance.ToOhms() * length.ToMeters());

    public override string ToString() => $"{ohms} Ω";
}
