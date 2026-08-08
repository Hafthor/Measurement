namespace com.hafthor.Measurement;

[Measurement("Hz", VariableName = "hertz")]
[SiUnit("Hertz", 0, "None Tera Giga Mega Kilo Milli")]
[Reciprocal<Duration>(Name = "Period")]
public readonly partial struct Frequency { }
