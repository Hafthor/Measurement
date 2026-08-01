namespace com.hafthor.Measurement;

[Measurement("V", VariableName = "microvolts", DisplayFactor = 1e6)]
[SiUnit("Volts", 6, "None Mega Kilo Milli Micro")]
[SiUnit("Abvolts", -2)]
[Unit("Statvolts", 299.792458e6)]
[Product<ElectricResistance, ElectricCurrent>]
[Product<Length, ElectricFieldStrength>]
public readonly partial struct Voltage { }
