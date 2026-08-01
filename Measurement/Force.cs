namespace com.hafthor.Measurement;

[Measurement("N", VariableName = "newtons")]
[SiUnit("Newtons", 0, "None Mega Kilo Milli")]
[SiUnit("Dynes", -5)]
[Unit("KilogramsForce", 9.80665)]
[Unit("PoundsForce", 4.4482216152605)]
[Unit("OuncesForce", 0.27801385095378125)]
[Unit("Poundals", 0.138254954376)]
[Product<Mass, Acceleration>]
[Product<Pressure, Area>]
[Product<SurfaceTension, Length>]
[Product<Length, SurfaceTension>]
public readonly partial struct Force { }
