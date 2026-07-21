namespace com.hafthor.Measurement;

public sealed class Permittivity : Measurement<Permittivity> {

    private Permittivity(double value) : base(value) { }

    protected override Permittivity Create(double value) => new(value);
    protected override string Symbol => "F/m";

    // Units
    public static Permittivity FromFaradsPerMeter(double value) => new(value);
    public double ToFaradsPerMeter() => value;

    // Composite relationships
    public static Capacitance operator *(Permittivity permittivity, Length length) => Capacitance.FromFarads(permittivity.ToFaradsPerMeter() * length.ToMeters());
    public static Capacitance operator *(Length length, Permittivity permittivity) => Capacitance.FromFarads(length.ToMeters() * permittivity.ToFaradsPerMeter());

}
