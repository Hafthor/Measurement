namespace com.hafthor.Measurement;

[Measurement("Pa·s", VariableName = "millipascalSeconds", DisplayFactor = 1e3)]
[SiUnit("PascalSeconds", 3, "None Milli")]
[SiUnit("Poise", 2, "None Centi")]
[Product<Pressure, Duration>]
public readonly partial struct DynamicViscosity { }
