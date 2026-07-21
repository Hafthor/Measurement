namespace com.hafthor.Measurement;

public class Density {
    private readonly double kilogramsPerCubicMeter;

    private Density(double kilogramsPerCubicMeter) => this.kilogramsPerCubicMeter = kilogramsPerCubicMeter;

    // Arithmetic
    public static Density operator +(Density a, Density b) => new(a.kilogramsPerCubicMeter + b.kilogramsPerCubicMeter);
    public static Density operator -(Density a, Density b) => new(a.kilogramsPerCubicMeter - b.kilogramsPerCubicMeter);
    public static Density operator -(Density x) => new(-x.kilogramsPerCubicMeter);

    // Units
    public static Density FromKilogramsPerCubicMeter(double kilogramsPerCubicMeter) => new(kilogramsPerCubicMeter);
    public double ToKilogramsPerCubicMeter() => kilogramsPerCubicMeter;
    public static Density FromGramsPerCubicCentimeter(double gramsPerCubicCentimeter) => new(gramsPerCubicCentimeter * (1000));
    public double ToGramsPerCubicCentimeter() => kilogramsPerCubicMeter / (1000);
    public static Density FromKilogramsPerLiter(double kilogramsPerLiter) => new(kilogramsPerLiter * (1000));
    public double ToKilogramsPerLiter() => kilogramsPerCubicMeter / (1000);
    public static Density FromGramsPerMilliliter(double gramsPerMilliliter) => new(gramsPerMilliliter * (1000));
    public double ToGramsPerMilliliter() => kilogramsPerCubicMeter / (1000);
    public static Density FromPoundsPerCubicFoot(double poundsPerCubicFoot) => new(poundsPerCubicFoot * (16.018463373947));
    public double ToPoundsPerCubicFoot() => kilogramsPerCubicMeter / (16.018463373947);
    public static Density FromPoundsPerGallon(double poundsPerGallon) => new(poundsPerGallon * (119.82642731689));
    public double ToPoundsPerGallon() => kilogramsPerCubicMeter / (119.82642731689);

    // Composite relationships
    public static Mass operator *(Density density, Volume volume) => Mass.FromKilograms(density.ToKilogramsPerCubicMeter() * volume.ToCubicMeters());
    public static Mass operator *(Volume volume, Density density) => Mass.FromKilograms(volume.ToCubicMeters() * density.ToKilogramsPerCubicMeter());

    public override string ToString() => $"{kilogramsPerCubicMeter} kg/m³";
}
