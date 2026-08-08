namespace com.hafthor.Measurement;

[Measurement("S/m", VariableName = "millisiemensPerCentimeter", DisplayFactor = 10)]
[SiUnit("SiemensPerMeter", 1)]
[SiUnit("SiemensPerCentimeter", 3, "None Milli")]
[Reciprocal<Resistivity>]
public readonly partial struct Conductivity { }
