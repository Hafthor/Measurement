namespace com.hafthor.Measurement;

[Measurement("Bq", VariableName = "becquerels")]
[SiUnit("Becquerels", 0, "None Giga Mega Kilo")]
[Unit("Curies", 3.7e10)]
[Unit("Millicuries", 3.7e7)]
[Unit("Microcuries", 3.7e4)]
[SiUnit("Rutherfords", 6)]
public readonly partial struct Radioactivity {
}
