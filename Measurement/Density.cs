namespace com.hafthor.Measurement;

[Measurement("g/m³", VariableName = "gramsPerCubicMeter")]
[SiUnit("KilogramsPerCubicMeter", 3)]
[SiUnit("GramsPerCubicCentimeter", 6)]
[SiUnit("KilogramsPerLiter", 6)]
[SiUnit("GramsPerMilliliter", 6)]
[Unit("PoundsPerCubicFoot", 16.018463373947e3)]
[Unit("PoundsPerGallon", 119.82642731689e3)]
public readonly partial struct Density {
    // Composite relationships
    public static Mass operator *(Density density, Volume volume) => Mass.FromKilograms(density.ToKilogramsPerCubicMeter() * volume.ToCubicMeters());
    public static Mass operator *(Volume volume, Density density) => Mass.FromKilograms(volume.ToCubicMeters() * density.ToKilogramsPerCubicMeter());
}
