namespace com.hafthor.Measurement;

[Measurement("Ω", VariableName = "ohms")]
[SiUnit("Ohms", 0, "None Giga Mega Kilo Milli Micro")]
public readonly partial struct ElectricResistance {
    // Composite relationships
    public static Voltage operator *(ElectricResistance resistance, ElectricCurrent current) => Voltage.FromVolts(resistance.ohms * current.ToAmperes());

    // Composite relationships (derived)
    public static Resistivity operator *(ElectricResistance electricResistance, Length length) => Resistivity.FromOhmMeters(electricResistance.ohms * length.ToMeters());

    // Reciprocal quantity (conductance G = 1/R)
    public ElectricConductance ToElectricConductance() => ElectricConductance.FromSiemens(1.0 / ohms);
}
