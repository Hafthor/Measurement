namespace com.hafthor.Measurement;

[Measurement("V", VariableName = "microvolts", DisplayFactor = 1e6)]
[SiUnit("Volts", 6, "None Mega Kilo Milli Micro")]
[SiUnit("Abvolts", -2)]
[Unit("Statvolts", 299.792458e6)]
public readonly partial struct Voltage {
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
