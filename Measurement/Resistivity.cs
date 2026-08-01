namespace com.hafthor.Measurement;

[Measurement("Ω·m", VariableName = "microohmCentimeters", DisplayFactor = 1e8)]
[SiUnit("OhmMeters", 8)]
[SiUnit("OhmCentimeters", 6, "None Micro")]
public readonly partial struct Resistivity {
    // Composite relationships
    public static ElectricResistance operator /(Resistivity resistivity, Length length) => ElectricResistance.FromOhms(resistivity.ToOhmMeters() / length.ToMeters());
    public static Length operator /(Resistivity resistivity, ElectricResistance electricResistance) => Length.FromMeters(resistivity.ToOhmMeters() / electricResistance.ToOhms());

    // Reciprocal quantity (conductivity σ = 1/ρ)
    public Conductivity ToConductivity() => Conductivity.FromSiemensPerMeter(1.0 / ToOhmMeters());
}
