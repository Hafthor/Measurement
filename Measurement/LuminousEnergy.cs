namespace com.hafthor.Measurement;

[Measurement("lm·s", VariableName = "lumenSeconds")]
[SiUnit("LumenSeconds", 0)]
[Unit("LumenHours", 3600)]
[SiUnit("Talbots", 0)]
[Product<LuminousFlux, Duration>]
public readonly partial struct LuminousEnergy { }
