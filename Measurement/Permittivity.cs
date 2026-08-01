namespace com.hafthor.Measurement;

[Measurement("F/m", VariableName = "faradsPerMeter")]
[SiUnit("FaradsPerMeter", 0)]
public readonly partial struct Permittivity {
    // Composite relationships
    public static Capacitance operator *(Permittivity permittivity, Length length) => Capacitance.FromFarads(permittivity.ToFaradsPerMeter() * length.ToMeters());
    public static Capacitance operator *(Length length, Permittivity permittivity) => Capacitance.FromFarads(length.ToMeters() * permittivity.ToFaradsPerMeter());
}
