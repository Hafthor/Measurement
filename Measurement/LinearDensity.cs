namespace com.hafthor.Measurement;

[Measurement("g/m", VariableName = "gramsPerMeter")]
public readonly partial struct LinearDensity {
    // Units
    public static LinearDensity FromKilogramsPerMeter(double kilogramsPerMeter) => new(kilogramsPerMeter * 1e3);
    public double ToKilogramsPerMeter() => gramsPerMeter / 1e3;
    public static LinearDensity FromGramsPerMeter(double gramsPerMeter) => new(gramsPerMeter);
    public double ToGramsPerMeter() => gramsPerMeter;
    public static LinearDensity FromGramsPerCentimeter(double gramsPerCentimeter) => new(gramsPerCentimeter * 1e2);
    public double ToGramsPerCentimeter() => gramsPerMeter / 1e2;
    public static LinearDensity FromTex(double tex) => new(tex * (1e-3));
    public double ToTex() => gramsPerMeter / (1e-3);
    public static LinearDensity FromDenier(double denier) => new(denier * (1.1111111111111e-4));
    public double ToDenier() => gramsPerMeter / (1.1111111111111e-4);

    // Composite relationships
    public static Mass operator *(LinearDensity linearDensity, Length length) => Mass.FromKilograms(linearDensity.ToKilogramsPerMeter() * length.ToMeters());
    public static Mass operator *(Length length, LinearDensity linearDensity) => Mass.FromKilograms(length.ToMeters() * linearDensity.ToKilogramsPerMeter());
}
