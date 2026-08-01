namespace com.hafthor.Measurement;

[Measurement("Pa", VariableName = "pascals")]
[SiUnit("Pascals", 0, "None Mega Kilo Hecto")]
[SiUnit("Bars", 5, "None Milli")]
[Unit("Atmospheres", 101325)]
[Unit("Torr", 133.32236842105263)]
[Unit("MillimetersOfMercury", 133.322387415)]
[Unit("InchesOfMercury", 3386.389)]
[Unit("InchesOfWater", 249.08891)]
[Unit("PoundsPerSquareInch", 6894.757293168)]
public readonly partial struct Pressure { }
