namespace com.hafthor.Measurement;

[Measurement("V", DisplayFactor = 1e6)]
public readonly partial struct Voltage {

    // Canonical (stored) unit is the microvolt, so µV/mV-scale values land on exact
    // integers in IEEE-754; ToString presents volts (DisplayFactor = 1e6).
    public static Voltage FromMegavolts(double megavolts) => new(megavolts * 1e12);
    public double ToMegavolts() => value / 1e12;
    public static Voltage FromKilovolts(double kilovolts) => new(kilovolts * 1e9);
    public double ToKilovolts() => value / 1e9;
    public static Voltage FromVolts(double volts) => new(volts * 1e6);
    public double ToVolts() => value / 1e6;
    public static Voltage FromMillivolts(double millivolts) => new(millivolts * 1e3);
    public double ToMillivolts() => value / 1e3;
    public static Voltage FromMicrovolts(double microvolts) => new(microvolts);
    public double ToMicrovolts() => value;

    // CGS units
    public static Voltage FromAbvolts(double abvolts) => new(abvolts * 1e-2);
    public double ToAbvolts() => value / 1e-2;
    public static Voltage FromStatvolts(double statvolts) => new(statvolts * 299.792458e6);
    public double ToStatvolts() => value / 299.792458e6;

    // Composite relationships
    public static ElectricResistance operator /(Voltage voltage, ElectricCurrent current) => ElectricResistance.FromOhms(voltage.ToVolts() / current.ToAmperes());
    public static ElectricCurrent operator /(Voltage voltage, ElectricResistance resistance) => ElectricCurrent.FromAmperes(voltage.ToVolts() / resistance.ToOhms());
    public static Power operator *(Voltage voltage, ElectricCurrent current) => Power.FromWatts(voltage.ToVolts() * current.ToAmperes());
    public static ElectricCharge operator *(Voltage voltage, Capacitance capacitance) => ElectricCharge.FromCoulombs(voltage.ToVolts() * capacitance.ToFarads());
    public static MagneticFlux operator *(Voltage voltage, Duration duration) => MagneticFlux.FromWebers(voltage.ToVolts() * duration.ToSeconds());

    // Composite relationships (derived)
    public static ElectricFieldStrength operator /(Voltage voltage, Length length) => ElectricFieldStrength.FromVoltsPerMeter(voltage.ToVolts() / length.ToMeters());

    // Famous relations
    public static Energy operator *(Voltage voltage, ElectricCharge charge) => Energy.FromJoules(voltage.ToVolts() * charge.ToCoulombs());

}
