namespace com.hafthor.Measurement;

[Measurement("Ω·m", VariableName = "microohmCentimeters", DisplayFactor = 1e8)]
[SiUnit("OhmMeters", 8)]
[SiUnit("OhmCentimeters", 6, "None Micro")]
[Product<ElectricResistance, Length>]
public readonly partial struct Resistivity {
    // Reciprocal quantity (conductivity σ = 1/ρ)
    public Conductivity ToConductivity() => Conductivity.FromSiemensPerMeter(1.0 / ToOhmMeters());
}
