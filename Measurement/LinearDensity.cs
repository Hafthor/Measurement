namespace com.hafthor.Measurement;

[Measurement("g/m", VariableName = "gramsPerMeter")]
[SiUnit("GramsPerMeter", 0, "None Kilo")]
[SiUnit("GramsPerCentimeter", 2)]
[SiUnit("Tex", -3)]
[Unit("Denier", 1.1111111111111e-4)]
public readonly partial struct LinearDensity {
    // Composite relationships
    public static Mass operator *(LinearDensity linearDensity, Length length) => Mass.FromKilograms(linearDensity.ToKilogramsPerMeter() * length.ToMeters());
    public static Mass operator *(Length length, LinearDensity linearDensity) => Mass.FromKilograms(length.ToMeters() * linearDensity.ToKilogramsPerMeter());
}
