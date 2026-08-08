namespace com.hafthor.Measurement;

[Measurement("m⁻¹", VariableName = "perMeter")]
[SiUnit("PerMeter", 0)]
[SiUnit("PerCentimeter", 2)]
[Reciprocal<Length>(Name = "Wavelength")]
public readonly partial struct Wavenumber { }
