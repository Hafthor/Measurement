namespace com.hafthor.Measurement;

[Measurement("Wb/m", VariableName = "nanowebersPerMeter", DisplayFactor = 1e9)]
public readonly partial struct LinearMagneticFluxDensity {
    // Canonical (stored) unit is the nanowebers/meter, so nW/m-scale values land on exact
    // integers in IEEE-754; ToString presents teslas (DisplayFactor = 1e9).
    public static LinearMagneticFluxDensity FromWebersPerMeter(double webersPerMeter) => new(webersPerMeter * 1e9);
    public double ToWebersPerMeter() => nanowebersPerMeter / 1e9;
    public static LinearMagneticFluxDensity FromMilliwebersPerMeter(double milliwebersPerMeter) => new(milliwebersPerMeter * 1e6);
    public double ToMilliwebersPerMeter() => nanowebersPerMeter / 1e6;
    public static LinearMagneticFluxDensity FromMicrowebersPerMeter(double microwebersPerMeter) => new(microwebersPerMeter * 1e3);
    public double ToMicrowebersPerMeter() => nanowebersPerMeter / 1e3;
    public static LinearMagneticFluxDensity FromNanowebersPerMeter(double nanowebersPerMeter) => new(nanowebersPerMeter * 1e2);
    public double ToNanowebersPerMeter() => nanowebersPerMeter;

    // Composite relationships
    public static MagneticFlux operator *(LinearMagneticFluxDensity density, Length length) => MagneticFlux.FromWebers(density.ToWebersPerMeter() * length.ToMeters());
}
