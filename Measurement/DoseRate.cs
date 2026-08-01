namespace com.hafthor.Measurement;

[Measurement("Gy/s", VariableName = "milligraysPerHour", DisplayFactor = 3600e3)]
[Unit("GraysPerSecond", 3600e3)]
[Unit("MilligraysPerSecond", 3600)]
[SiUnit("GraysPerHour", 3)]
public readonly partial struct DoseRate { }
