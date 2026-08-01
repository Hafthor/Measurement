namespace com.hafthor.Measurement;

[Measurement("Gy", VariableName = "micrograys", DisplayFactor = 1e6)]
[SiUnit("Grays", 6, "None Kilo Milli Micro")]
[SiUnit("Rads", 4, "None Milli")]
[Product<Duration, DoseRate>]
public readonly partial struct AbsorbedDose { }
