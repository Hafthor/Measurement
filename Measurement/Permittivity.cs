namespace com.hafthor.Measurement;

[Measurement("F/m", VariableName = "faradsPerMeter")]
public readonly partial struct Permittivity {
    // Units
    public static Permittivity FromFaradsPerMeter(double faradsPerMeter) => new(faradsPerMeter);
    public double ToFaradsPerMeter() => faradsPerMeter;

    // Composite relationships
    public static Capacitance operator *(Permittivity permittivity, Length length) => Capacitance.FromFarads(permittivity.ToFaradsPerMeter() * length.ToMeters());
    public static Capacitance operator *(Length length, Permittivity permittivity) => Capacitance.FromFarads(length.ToMeters() * permittivity.ToFaradsPerMeter());
}
