namespace com.hafthor.Measurement;

public class Capacitance {
    private readonly double farads;

    private Capacitance(double farads) => this.farads = farads;

    // Arithmetic
    public static Capacitance operator +(Capacitance a, Capacitance b) => new Capacitance(a.farads + b.farads);
    public static Capacitance operator -(Capacitance a, Capacitance b) => new Capacitance(a.farads - b.farads);
    public static Capacitance operator -(Capacitance x) => new Capacitance(-x.farads);

    // SI units
    public static Capacitance FromFarads(double farads) => new Capacitance(farads);
    public double ToFarads() => farads;
    public static Capacitance FromMillifarads(double millifarads) => new Capacitance(millifarads * 1e-3);
    public double ToMillifarads() => farads / 1e-3;
    public static Capacitance FromMicrofarads(double microfarads) => new Capacitance(microfarads * 1e-6);
    public double ToMicrofarads() => farads / 1e-6;
    public static Capacitance FromNanofarads(double nanofarads) => new Capacitance(nanofarads * 1e-9);
    public double ToNanofarads() => farads / 1e-9;
    public static Capacitance FromPicofarads(double picofarads) => new Capacitance(picofarads * 1e-12);
    public double ToPicofarads() => farads / 1e-12;

    // CGS units
    public static Capacitance FromAbfarads(double abfarads) => new Capacitance(abfarads * 1e9);
    public double ToAbfarads() => farads / 1e9;
    public static Capacitance FromStatfarads(double statfarads) => new Capacitance(statfarads * 1.1126500560536185e-12);
    public double ToStatfarads() => farads / 1.1126500560536185e-12;

    // Composite relationships
    public static ElectricCharge operator *(Capacitance capacitance, Voltage voltage) => ElectricCharge.FromCoulombs(capacitance.farads * voltage.ToVolts());

    // Composite relationships (derived)
    public static Permittivity operator /(Capacitance capacitance, Length length) => Permittivity.FromFaradsPerMeter(capacitance.ToFarads() / length.ToMeters());
}
