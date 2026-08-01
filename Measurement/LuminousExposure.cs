namespace com.hafthor.Measurement;

[Measurement("lx·s", VariableName = "luxSeconds")]
[SiUnit("LuxSeconds", 0)]
[Unit("LuxHours", 3600)]
[Product<Duration, Illuminance>]
public readonly partial struct LuminousExposure { }
