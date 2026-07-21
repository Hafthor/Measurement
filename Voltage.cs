namespace com.hafthor.Measurement;

[Measurement("V")]
public readonly partial struct Voltage {

    // SI units
    public static Voltage FromMegavolts(double megavolts) => new(megavolts * 1e6);
    public double ToMegavolts() => value / 1e6;
    public static Voltage FromKilovolts(double kilovolts) => new(kilovolts * 1e3);
    public double ToKilovolts() => value / 1e3;
    public static Voltage FromVolts(double volts) => new(volts);
    public double ToVolts() => value;
    public static Voltage FromMillivolts(double millivolts) => new(millivolts * 1e-3);
    public double ToMillivolts() => value / 1e-3;
    public static Voltage FromMicrovolts(double microvolts) => new(microvolts * 1e-6);
    public double ToMicrovolts() => value / 1e-6;

    // CGS units
    public static Voltage FromAbvolts(double abvolts) => new(abvolts * 1e-8);
    public double ToAbvolts() => value / 1e-8;
    public static Voltage FromStatvolts(double statvolts) => new(statvolts * 299.792458);
    public double ToStatvolts() => value / 299.792458;

    // Composite relationships
    public static ElectricResistance operator /(Voltage voltage, ElectricCurrent current) => ElectricResistance.FromOhms(voltage.value / current.ToAmperes());
    public static ElectricCurrent operator /(Voltage voltage, ElectricResistance resistance) => ElectricCurrent.FromAmperes(voltage.value / resistance.ToOhms());
    public static Power operator *(Voltage voltage, ElectricCurrent current) => Power.FromWatts(voltage.value * current.ToAmperes());
    public static ElectricCharge operator *(Voltage voltage, Capacitance capacitance) => ElectricCharge.FromCoulombs(voltage.value * capacitance.ToFarads());
    public static MagneticFlux operator *(Voltage voltage, Duration duration) => MagneticFlux.FromWebers(voltage.value * duration.ToSeconds());

    // Composite relationships (derived)
    public static ElectricFieldStrength operator /(Voltage voltage, Length length) => ElectricFieldStrength.FromVoltsPerMeter(voltage.ToVolts() / length.ToMeters());

    // Famous relations
    public static Energy operator *(Voltage voltage, ElectricCharge charge) => Energy.FromJoules(voltage.ToVolts() * charge.ToCoulombs());

}
