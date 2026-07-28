namespace com.hafthor.Measurement;

[Measurement("g·m²", VariableName = "gramSquareMeters")]
public readonly partial struct MomentOfInertia {
    // Units
    public static MomentOfInertia FromKilogramSquareMeters(double kilogramSquareMeters) => new(kilogramSquareMeters * 1e3);
    public double ToKilogramSquareMeters() => gramSquareMeters / 1e3;
    public static MomentOfInertia FromGramSquareMeters(double gramSquareMeters) => new(gramSquareMeters);
    public double ToGramSquareMeters() => gramSquareMeters;
    public static MomentOfInertia FromKilogramSquareCentimeters(double kilogramSquareCentimeters) => new(kilogramSquareCentimeters * (1e-1));
    public double ToKilogramSquareCentimeters() => gramSquareMeters / (1e-1);
    public static MomentOfInertia FromGramSquareCentimeters(double gramSquareCentimeters) => new(gramSquareCentimeters * (1e-4));
    public double ToGramSquareCentimeters() => gramSquareMeters / (1e-4);
    public static MomentOfInertia FromPoundSquareFeet(double poundSquareFeet) => new(poundSquareFeet * (0.04214011009380e3));
    public double ToPoundSquareFeet() => gramSquareMeters / (0.04214011009380e3);

    // Composite relationships
    public static Mass operator /(MomentOfInertia momentOfInertia, Area area) => Mass.FromKilograms(momentOfInertia.ToKilogramSquareMeters() / area.ToSquareMeters());
    public static Area operator /(MomentOfInertia momentOfInertia, Mass mass) => Area.FromSquareMeters(momentOfInertia.ToKilogramSquareMeters() / mass.ToKilograms());
    public static AngularMomentum operator *(MomentOfInertia momentOfInertia, AngularVelocity angularVelocity) => AngularMomentum.FromKilogramSquareMetersPerSecond(momentOfInertia.ToKilogramSquareMeters() * angularVelocity.ToRadiansPerSecond());
}
