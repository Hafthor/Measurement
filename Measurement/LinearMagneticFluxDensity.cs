namespace com.hafthor.Measurement;

[Measurement("Wb/m", VariableName = "nanowebersPerMeter", DisplayFactor = 1e9)]
[SiUnit("WebersPerMeter", 9, "None Milli Micro Nano")]
public readonly partial struct LinearMagneticFluxDensity {
    // Composite relationships
    public static MagneticFlux operator *(LinearMagneticFluxDensity density, Length length) => MagneticFlux.FromWebers(density.ToWebersPerMeter() * length.ToMeters());
}
