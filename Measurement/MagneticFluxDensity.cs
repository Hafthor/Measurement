namespace com.hafthor.Measurement;

[Measurement("T", VariableName = "nanoteslas", DisplayFactor = 1e9)]
[SiUnit("Teslas", 9, "None Milli Micro Nano")]
[SiUnit("Gauss", 5, "None Milli")]
public readonly partial struct MagneticFluxDensity {
    // Composite relationships
    public static MagneticFlux operator *(MagneticFluxDensity density, Area area) => MagneticFlux.FromWebers(density.ToTeslas() * area.ToSquareMeters());
}
