namespace com.hafthor.Measurement;

[Measurement("H", VariableName = "nanohenries", DisplayFactor = 1e9)]
[SiUnit("Henries", 9, "None Milli Micro Nano")]
[SiUnit("Abhenries", 0)]
[Unit("Stathenries", 8.987551787368176e20)]
[Product<Length, Permeability>]
public readonly partial struct Inductance { }
