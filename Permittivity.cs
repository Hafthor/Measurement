namespace com.hafthor.Measurement;

public class Permittivity {
    private readonly double faradsPerMeter;

    private Permittivity(double faradsPerMeter) => this.faradsPerMeter = faradsPerMeter;

    // Arithmetic
    public static Permittivity operator +(Permittivity a, Permittivity b) => new Permittivity(a.faradsPerMeter + b.faradsPerMeter);
    public static Permittivity operator -(Permittivity a, Permittivity b) => new Permittivity(a.faradsPerMeter - b.faradsPerMeter);
    public static Permittivity operator -(Permittivity x) => new Permittivity(-x.faradsPerMeter);

    // Units
    public static Permittivity FromFaradsPerMeter(double faradsPerMeter) => new Permittivity(faradsPerMeter);
    public double ToFaradsPerMeter() => faradsPerMeter;

    // Composite relationships
    public static Capacitance operator *(Permittivity permittivity, Length length) => Capacitance.FromFarads(permittivity.ToFaradsPerMeter() * length.ToMeters());
    public static Capacitance operator *(Length length, Permittivity permittivity) => Capacitance.FromFarads(length.ToMeters() * permittivity.ToFaradsPerMeter());
}
