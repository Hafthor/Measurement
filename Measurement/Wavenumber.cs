namespace com.hafthor.Measurement;

[Measurement("m⁻¹", VariableName = "perMeter")]
[SiUnit("PerMeter", 0)]
[SiUnit("PerCentimeter", 2)]
public readonly partial struct Wavenumber {
    // Reciprocal of wavelength
    public static Wavenumber FromWavelength(Length wavelength) => new(1 / wavelength.ToMeters());
    public Length ToWavelength() => Length.FromMeters(1 / perMeter);
}
