namespace com.hafthor.Measurement;

[Measurement("S/m", VariableName = "millisiemensPerCentimeter", DisplayFactor = 10)]
[SiUnit("SiemensPerMeter", 1)]
[SiUnit("SiemensPerCentimeter", 3, "None Milli")]
public readonly partial struct Conductivity {
    // Reciprocal quantity (resistivity ρ = 1/σ)
    public Resistivity ToResistivity() => Resistivity.FromOhmMeters(1.0 / ToSiemensPerMeter());
}
