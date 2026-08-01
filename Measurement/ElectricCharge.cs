namespace com.hafthor.Measurement;

[Measurement("C", VariableName = "nanocoulombs", DisplayFactor = 1e9)]
[SiUnit("Coulombs", 9, "None Kilo Milli Micro Nano")]
[Unit("AmpereHours", 3.6e12)]
[Unit("MilliampereHours", 3.6e9)]
[Unit("Faradays", 96485.33212e9)]
[Unit("ElementaryCharges", 1.602176634e-10)]
[SiUnit("Abcoulombs", 10)]
[Unit("Statcoulombs", 3.335641e-1)]
[Product<Voltage, Capacitance>]
[Product<Mass, Exposure>]
[Product<ElectricCurrent, Duration>]
[Product<Volume, ChargeDensity>]
[Product<Area, SurfaceChargeDensity>]
public readonly partial struct ElectricCharge { }
