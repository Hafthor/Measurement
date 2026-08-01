namespace com.hafthor.Measurement;

[Measurement("Ω", VariableName = "ohms")]
[SiUnit("Ohms", 0, "None Giga Mega Kilo Milli Micro")]
public readonly partial struct ElectricResistance {
    // Reciprocal quantity (conductance G = 1/R)
    public ElectricConductance ToElectricConductance() => ElectricConductance.FromSiemens(1.0 / ohms);
}
