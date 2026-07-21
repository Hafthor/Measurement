namespace com.hafthor.Measurement;

public sealed class LinearDensity : Measurement<LinearDensity> {

    private LinearDensity(double value) : base(value) { }

    protected override LinearDensity Create(double value) => new(value);
    protected override string Symbol => "kg/m";

    // Units
    public static LinearDensity FromKilogramsPerMeter(double value) => new(value);
    public double ToKilogramsPerMeter() => value;
    public static LinearDensity FromGramsPerMeter(double gramsPerMeter) => new(gramsPerMeter * (1e-3));
    public double ToGramsPerMeter() => value / (1e-3);
    public static LinearDensity FromGramsPerCentimeter(double gramsPerCentimeter) => new(gramsPerCentimeter * (0.1));
    public double ToGramsPerCentimeter() => value / (0.1);
    public static LinearDensity FromTex(double tex) => new(tex * (1e-6));
    public double ToTex() => value / (1e-6);
    public static LinearDensity FromDenier(double denier) => new(denier * (1.1111111111111e-7));
    public double ToDenier() => value / (1.1111111111111e-7);

    // Composite relationships
    public static Mass operator *(LinearDensity linearDensity, Length length) => Mass.FromKilograms(linearDensity.ToKilogramsPerMeter() * length.ToMeters());
    public static Mass operator *(Length length, LinearDensity linearDensity) => Mass.FromKilograms(length.ToMeters() * linearDensity.ToKilogramsPerMeter());

}
