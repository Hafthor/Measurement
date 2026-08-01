namespace com.hafthor.Measurement;

[Measurement("S/m", VariableName = "millisiemensPerCentimeter", DisplayFactor = 10)]
[SiUnit("SiemensPerMeter", 1)]
[SiUnit("SiemensPerCentimeter", 3, "None Milli")]
public readonly partial struct Conductivity {
    // Composite relationships
    public static ElectricConductance operator *(Conductivity conductivity, Length length) => ElectricConductance.FromSiemens(conductivity.ToSiemensPerMeter() * length.ToMeters());
    public static ElectricConductance operator *(Length length, Conductivity conductivity) => ElectricConductance.FromSiemens(length.ToMeters() * conductivity.ToSiemensPerMeter());

    // Reciprocal quantity (resistivity ρ = 1/σ)
    public Resistivity ToResistivity() => Resistivity.FromOhmMeters(1.0 / ToSiemensPerMeter());
}
