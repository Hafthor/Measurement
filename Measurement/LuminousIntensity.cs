namespace com.hafthor.Measurement;

[Measurement("cd", VariableName = "millicandelas", DisplayFactor = 1e3)]
[SiUnit("Candelas", 3, "None Kilo Milli")]
[Unit("Candlepower", 0.981e3)]
[Unit("Hefnerkerze", 0.903e3)]
[Unit("Carcels", 9.74e3)]
[Product<Area, Luminance>]
public readonly partial struct LuminousIntensity { }
