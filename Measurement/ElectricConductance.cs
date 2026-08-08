namespace com.hafthor.Measurement;

[Measurement("S", VariableName = "nanosiemens", DisplayFactor = 1e9)]
[SiUnit("Siemens", 9, "None Kilo Milli Micro Nano")]
[SiUnit("Mhos", 9)]
[Product<Length, Conductivity>]
[Reciprocal<ElectricResistance>]
public readonly partial struct ElectricConductance { }
