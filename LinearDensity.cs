namespace com.hafthor.Measurement;

public class LinearDensity {
    private readonly double kilogramsPerMeter;

    private LinearDensity(double kilogramsPerMeter) => this.kilogramsPerMeter = kilogramsPerMeter;

    // Arithmetic
    public static LinearDensity operator +(LinearDensity a, LinearDensity b) => new(a.kilogramsPerMeter + b.kilogramsPerMeter);
    public static LinearDensity operator -(LinearDensity a, LinearDensity b) => new(a.kilogramsPerMeter - b.kilogramsPerMeter);
    public static LinearDensity operator -(LinearDensity x) => new(-x.kilogramsPerMeter);

    // Units
    public static LinearDensity FromKilogramsPerMeter(double kilogramsPerMeter) => new(kilogramsPerMeter);
    public double ToKilogramsPerMeter() => kilogramsPerMeter;
    public static LinearDensity FromGramsPerMeter(double gramsPerMeter) => new(gramsPerMeter * (1e-3));
    public double ToGramsPerMeter() => kilogramsPerMeter / (1e-3);
    public static LinearDensity FromGramsPerCentimeter(double gramsPerCentimeter) => new(gramsPerCentimeter * (0.1));
    public double ToGramsPerCentimeter() => kilogramsPerMeter / (0.1);
    public static LinearDensity FromTex(double tex) => new(tex * (1e-6));
    public double ToTex() => kilogramsPerMeter / (1e-6);
    public static LinearDensity FromDenier(double denier) => new(denier * (1.1111111111111e-7));
    public double ToDenier() => kilogramsPerMeter / (1.1111111111111e-7);

    // Composite relationships
    public static Mass operator *(LinearDensity linearDensity, Length length) => Mass.FromKilograms(linearDensity.ToKilogramsPerMeter() * length.ToMeters());
    public static Mass operator *(Length length, LinearDensity linearDensity) => Mass.FromKilograms(length.ToMeters() * linearDensity.ToKilogramsPerMeter());

    public override string ToString() => $"{kilogramsPerMeter} kg/m";

    public override bool Equals(object obj) => obj is LinearDensity other && other.kilogramsPerMeter == kilogramsPerMeter;
    public override int GetHashCode() => kilogramsPerMeter.GetHashCode();
}
