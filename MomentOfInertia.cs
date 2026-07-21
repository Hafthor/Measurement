namespace com.hafthor.Measurement;

[Measurement("kg·m²")]
public readonly partial struct MomentOfInertia {

    // Units
    public static MomentOfInertia FromKilogramSquareMeters(double kilogramSquareMeters) => new(kilogramSquareMeters);
    public double ToKilogramSquareMeters() => value;
    public static MomentOfInertia FromKilogramSquareCentimeters(double kilogramSquareCentimeters) => new(kilogramSquareCentimeters * (1e-4));
    public double ToKilogramSquareCentimeters() => value / (1e-4);
    public static MomentOfInertia FromGramSquareCentimeters(double gramSquareCentimeters) => new(gramSquareCentimeters * (1e-7));
    public double ToGramSquareCentimeters() => value / (1e-7);
    public static MomentOfInertia FromPoundSquareFeet(double poundSquareFeet) => new(poundSquareFeet * (0.04214011009380));
    public double ToPoundSquareFeet() => value / (0.04214011009380);

    // Composite relationships
    public static Mass operator /(MomentOfInertia momentOfInertia, Area area) => Mass.FromKilograms(momentOfInertia.ToKilogramSquareMeters() / area.ToSquareMeters());
    public static Area operator /(MomentOfInertia momentOfInertia, Mass mass) => Area.FromSquareMeters(momentOfInertia.ToKilogramSquareMeters() / mass.ToKilograms());
    public static AngularMomentum operator *(MomentOfInertia momentOfInertia, AngularVelocity angularVelocity) => AngularMomentum.FromKilogramSquareMetersPerSecond(momentOfInertia.ToKilogramSquareMeters() * angularVelocity.ToRadiansPerSecond());

}
