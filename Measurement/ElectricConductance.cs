namespace com.hafthor.Measurement;

[Measurement("S", VariableName = "nanosiemens", DisplayFactor = 1e9)]
[SiUnit("Siemens", 9, "None Kilo Milli Micro Nano")]
[SiUnit("Mhos", 9)]
public readonly partial struct ElectricConductance {
    // Composite relationships (derived)
    public static Conductivity operator /(ElectricConductance electricConductance, Length length) => Conductivity.FromSiemensPerMeter(electricConductance.ToSiemens() / length.ToMeters());

    // Reciprocal quantity (resistance R = 1/G)
    public ElectricResistance ToElectricResistance() => ElectricResistance.FromOhms(1.0 / ToSiemens());
}
