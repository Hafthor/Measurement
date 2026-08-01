namespace com.hafthor.Measurement;

[Measurement("Wb", VariableName = "nanowebers", DisplayFactor = 1e9)]
[SiUnit("Webers", 9, "None Milli Micro Nano")]
[SiUnit("Maxwells", 1)]
[Product<Voltage, Duration>]
[Product<Inductance, ElectricCurrent>]
[Product<MagneticFluxDensity, Area>]
[Product<LinearMagneticFluxDensity, Length>]
public readonly partial struct MagneticFlux { }
