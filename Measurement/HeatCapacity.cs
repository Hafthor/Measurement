namespace com.hafthor.Measurement;

[Measurement("J/K", VariableName = "joulesPerKelvin")]
[SiUnit("JoulesPerKelvin", 0, "None Kilo")]
[Unit("CaloriesPerKelvin", 4.184)]
[Product<Mass, SpecificHeatCapacity>]
[Product<MolarHeatCapacity, Quantity>]
public readonly partial struct HeatCapacity { }
