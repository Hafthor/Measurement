namespace com.hafthor.Measurement;

[Measurement("g/m³", VariableName = "gramsPerCubicMeter")]
public readonly partial struct Density {
    // Units
    public static Density FromKilogramsPerCubicMeter(double kilogramsPerCubicMeter) => new(kilogramsPerCubicMeter * 1e3);
    public double ToKilogramsPerCubicMeter() => gramsPerCubicMeter / 1e3;
    public static Density FromGramsPerCubicCentimeter(double gramsPerCubicCentimeter) => new(gramsPerCubicCentimeter * 1e6);
    public double ToGramsPerCubicCentimeter() => gramsPerCubicMeter / 1e6;
    public static Density FromKilogramsPerLiter(double kilogramsPerLiter) => new(kilogramsPerLiter * 1e6);
    public double ToKilogramsPerLiter() => gramsPerCubicMeter / 1e6;
    public static Density FromGramsPerMilliliter(double gramsPerMilliliter) => new(gramsPerMilliliter * 1e6);
    public double ToGramsPerMilliliter() => gramsPerCubicMeter / 1e6;
    public static Density FromPoundsPerCubicFoot(double poundsPerCubicFoot) => new(poundsPerCubicFoot * (16.018463373947e3));
    public double ToPoundsPerCubicFoot() => gramsPerCubicMeter / (16.018463373947e3);
    public static Density FromPoundsPerGallon(double poundsPerGallon) => new(poundsPerGallon * (119.82642731689e3));
    public double ToPoundsPerGallon() => gramsPerCubicMeter / (119.82642731689e3);

    // Composite relationships
    public static Mass operator *(Density density, Volume volume) => Mass.FromKilograms(density.ToKilogramsPerCubicMeter() * volume.ToCubicMeters());
    public static Mass operator *(Volume volume, Density density) => Mass.FromKilograms(volume.ToCubicMeters() * density.ToKilogramsPerCubicMeter());
}
