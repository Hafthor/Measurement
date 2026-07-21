namespace com.hafthor.Measurement;

public class Voltage {
    private readonly double volts;

    private Voltage(double volts) => this.volts = volts;

    // Arithmetic
    public static Voltage operator +(Voltage a, Voltage b) => new(a.volts + b.volts);
    public static Voltage operator -(Voltage a, Voltage b) => new(a.volts - b.volts);
    public static Voltage operator -(Voltage x) => new(-x.volts);

    // SI units
    public static Voltage FromMegavolts(double megavolts) => new(megavolts * 1e6);
    public double ToMegavolts() => volts / 1e6;
    public static Voltage FromKilovolts(double kilovolts) => new(kilovolts * 1e3);
    public double ToKilovolts() => volts / 1e3;
    public static Voltage FromVolts(double volts) => new(volts);
    public double ToVolts() => volts;
    public static Voltage FromMillivolts(double millivolts) => new(millivolts * 1e-3);
    public double ToMillivolts() => volts / 1e-3;
    public static Voltage FromMicrovolts(double microvolts) => new(microvolts * 1e-6);
    public double ToMicrovolts() => volts / 1e-6;

    // CGS units
    public static Voltage FromAbvolts(double abvolts) => new(abvolts * 1e-8);
    public double ToAbvolts() => volts / 1e-8;
    public static Voltage FromStatvolts(double statvolts) => new(statvolts * 299.792458);
    public double ToStatvolts() => volts / 299.792458;

    // Composite relationships
    public static ElectricResistance operator /(Voltage voltage, ElectricCurrent current) => ElectricResistance.FromOhms(voltage.volts / current.ToAmperes());
    public static ElectricCurrent operator /(Voltage voltage, ElectricResistance resistance) => ElectricCurrent.FromAmperes(voltage.volts / resistance.ToOhms());
    public static Power operator *(Voltage voltage, ElectricCurrent current) => Power.FromWatts(voltage.volts * current.ToAmperes());
    public static ElectricCharge operator *(Voltage voltage, Capacitance capacitance) => ElectricCharge.FromCoulombs(voltage.volts * capacitance.ToFarads());
    public static MagneticFlux operator *(Voltage voltage, Duration duration) => MagneticFlux.FromWebers(voltage.volts * duration.ToSeconds());

    // Composite relationships (derived)
    public static ElectricFieldStrength operator /(Voltage voltage, Length length) => ElectricFieldStrength.FromVoltsPerMeter(voltage.ToVolts() / length.ToMeters());

    // Famous relations
    public static Energy operator *(Voltage voltage, ElectricCharge charge) => Energy.FromJoules(voltage.ToVolts() * charge.ToCoulombs());

    public override string ToString() => $"{volts} V";

    public override bool Equals(object obj) => obj is Voltage other && other.volts == volts;
    public override int GetHashCode() => volts.GetHashCode();
}
