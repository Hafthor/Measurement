namespace com.hafthor.Measurement;

[Measurement("F", DisplayFactor = 1e12, VariableName = "picofarads")]
[SiUnit("Farads", 12, "None Milli Micro Nano Pico")]
[SiUnit("Abfarads", 21)]
[Unit("Statfarads", 1.1126500560536185)]
[Product<Length, Permittivity>]
public readonly partial struct Capacitance { }
