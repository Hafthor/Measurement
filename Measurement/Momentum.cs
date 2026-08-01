namespace com.hafthor.Measurement;

[Measurement("g·m/s", VariableName = "gramMetersPerSecond")]
[SiUnit("GramMetersPerSecond", 0, "None Kilo")]
[SiUnit("NewtonSeconds", 3)]
[Product<Mass, Speed>]
[Product<Force, Duration>]
public readonly partial struct Momentum { }
