namespace com.hafthor.Measurement;

[Measurement("F", DisplayFactor = 1e12)]
public readonly partial struct Capacitance {

    // Canonical (stored) unit is the picofarad, so pF/nF/µF-scale values land on exact
    // integers in IEEE-754; ToString presents farads (DisplayFactor = 1e12).
    public static Capacitance FromFarads(double farads) => new(farads * 1e12);
    public double ToFarads() => value / 1e12;
    public static Capacitance FromMillifarads(double millifarads) => new(millifarads * 1e9);
    public double ToMillifarads() => value / 1e9;
    public static Capacitance FromMicrofarads(double microfarads) => new(microfarads * 1e6);
    public double ToMicrofarads() => value / 1e6;
    public static Capacitance FromNanofarads(double nanofarads) => new(nanofarads * 1e3);
    public double ToNanofarads() => value / 1e3;
    public static Capacitance FromPicofarads(double picofarads) => new(picofarads);
    public double ToPicofarads() => value;

    // CGS units
    public static Capacitance FromAbfarads(double abfarads) => new(abfarads * 1e21);
    public double ToAbfarads() => value / 1e21;
    public static Capacitance FromStatfarads(double statfarads) => new(statfarads * 1.1126500560536185);
    public double ToStatfarads() => value / 1.1126500560536185;

    // Composite relationships
    public static ElectricCharge operator *(Capacitance capacitance, Voltage voltage) => ElectricCharge.FromCoulombs(capacitance.ToFarads() * voltage.ToVolts());

    // Composite relationships (derived)
    public static Permittivity operator /(Capacitance capacitance, Length length) => Permittivity.FromFaradsPerMeter(capacitance.ToFarads() / length.ToMeters());

}
