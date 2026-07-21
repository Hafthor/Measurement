namespace com.hafthor.Measurement;

[Measurement("kg/m³")]
public readonly partial struct Density {

    // Units
    public static Density FromKilogramsPerCubicMeter(double kilogramsPerCubicMeter) => new(kilogramsPerCubicMeter);
    public double ToKilogramsPerCubicMeter() => value;
    public static Density FromGramsPerCubicCentimeter(double gramsPerCubicCentimeter) => new(gramsPerCubicCentimeter * (1000));
    public double ToGramsPerCubicCentimeter() => value / (1000);
    public static Density FromKilogramsPerLiter(double kilogramsPerLiter) => new(kilogramsPerLiter * (1000));
    public double ToKilogramsPerLiter() => value / (1000);
    public static Density FromGramsPerMilliliter(double gramsPerMilliliter) => new(gramsPerMilliliter * (1000));
    public double ToGramsPerMilliliter() => value / (1000);
    public static Density FromPoundsPerCubicFoot(double poundsPerCubicFoot) => new(poundsPerCubicFoot * (16.018463373947));
    public double ToPoundsPerCubicFoot() => value / (16.018463373947);
    public static Density FromPoundsPerGallon(double poundsPerGallon) => new(poundsPerGallon * (119.82642731689));
    public double ToPoundsPerGallon() => value / (119.82642731689);

    // Composite relationships
    public static Mass operator *(Density density, Volume volume) => Mass.FromKilograms(density.ToKilogramsPerCubicMeter() * volume.ToCubicMeters());
    public static Mass operator *(Volume volume, Density density) => Mass.FromKilograms(volume.ToCubicMeters() * density.ToKilogramsPerCubicMeter());

}
