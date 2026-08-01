namespace com.hafthor.Measurement;

[Measurement("F", DisplayFactor = 1e12, VariableName = "picofarads")]
[SiUnit("Farads", 12, "None Milli Micro Nano Pico")]
[SiUnit("Abfarads", 21)]
[Unit("Statfarads", 1.1126500560536185)]
public readonly partial struct Capacitance {
    // Composite relationships
    public static ElectricCharge operator *(Capacitance capacitance, Voltage voltage) => ElectricCharge.FromCoulombs(capacitance.ToFarads() * voltage.ToVolts());

    // Composite relationships (derived)
    public static Permittivity operator /(Capacitance capacitance, Length length) => Permittivity.FromFaradsPerMeter(capacitance.ToFarads() / length.ToMeters());
}
