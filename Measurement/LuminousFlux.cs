namespace com.hafthor.Measurement;

[Measurement("lm", VariableName = "millilumens", DisplayFactor = 1e3)]
[SiUnit("Lumens", 3, "None Kilo Milli")]
[Product<Illuminance, Area>]
[Product<SolidAngle, LuminousIntensity>]
public readonly partial struct LuminousFlux { }
