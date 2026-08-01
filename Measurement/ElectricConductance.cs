namespace com.hafthor.Measurement;

[Measurement("S", VariableName = "nanosiemens", DisplayFactor = 1e9)]
[SiUnit("Siemens", 9, "None Kilo Milli Micro Nano")]
[SiUnit("Mhos", 9)]
[Product<Length, Conductivity>]
public readonly partial struct ElectricConductance {
    // Reciprocal quantity (resistance R = 1/G)
    public ElectricResistance ToElectricResistance() => ElectricResistance.FromOhms(1.0 / ToSiemens());
}
