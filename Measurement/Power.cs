namespace com.hafthor.Measurement;

[Measurement("W", VariableName = "watts")]
[SiUnit("Watts", 0, "None Giga Mega Kilo Milli")]
[Unit("Horsepower", 745.6998715822702)]
[Unit("MetricHorsepower", 735.49875)]
[Unit("BritishThermalUnitsPerHour", 0.29307107017)]
[Unit("FootPoundsPerSecond", 1.3558179483314004)]
[Product<Force, Speed>]
[Product<Voltage, ElectricCurrent>]
[Product<SolidAngle, RadiantIntensity>]
[Product<Area, HeatFluxDensity>]
public readonly partial struct Power { }
