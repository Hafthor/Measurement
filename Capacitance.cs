namespace com.hafthor.Measurement;

public sealed class Capacitance : Measurement<Capacitance> {

    private Capacitance(double value) : base(value) { }

    protected override Capacitance Create(double value) => new(value);
    protected override string Symbol => "F";

    // SI units
    public static Capacitance FromFarads(double value) => new(value);
    public double ToFarads() => value;
    public static Capacitance FromMillifarads(double millifarads) => new(millifarads * 1e-3);
    public double ToMillifarads() => value / 1e-3;
    public static Capacitance FromMicrofarads(double microfarads) => new(microfarads * 1e-6);
    public double ToMicrofarads() => value / 1e-6;
    public static Capacitance FromNanofarads(double nanofarads) => new(nanofarads * 1e-9);
    public double ToNanofarads() => value / 1e-9;
    public static Capacitance FromPicofarads(double picofarads) => new(picofarads * 1e-12);
    public double ToPicofarads() => value / 1e-12;

    // CGS units
    public static Capacitance FromAbfarads(double abfarads) => new(abfarads * 1e9);
    public double ToAbfarads() => value / 1e9;
    public static Capacitance FromStatfarads(double statfarads) => new(statfarads * 1.1126500560536185e-12);
    public double ToStatfarads() => value / 1.1126500560536185e-12;

    // Composite relationships
    public static ElectricCharge operator *(Capacitance capacitance, Voltage voltage) => ElectricCharge.FromCoulombs(capacitance.value * voltage.ToVolts());

    // Composite relationships (derived)
    public static Permittivity operator /(Capacitance capacitance, Length length) => Permittivity.FromFaradsPerMeter(capacitance.ToFarads() / length.ToMeters());

}
