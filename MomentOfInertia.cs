namespace com.hafthor.Measurement;

public class MomentOfInertia {
    private readonly double kilogramSquareMeters;

    private MomentOfInertia(double kilogramSquareMeters) => this.kilogramSquareMeters = kilogramSquareMeters;

    // Arithmetic
    public static MomentOfInertia operator +(MomentOfInertia a, MomentOfInertia b) => new MomentOfInertia(a.kilogramSquareMeters + b.kilogramSquareMeters);
    public static MomentOfInertia operator -(MomentOfInertia a, MomentOfInertia b) => new MomentOfInertia(a.kilogramSquareMeters - b.kilogramSquareMeters);
    public static MomentOfInertia operator -(MomentOfInertia x) => new MomentOfInertia(-x.kilogramSquareMeters);

    // Units
    public static MomentOfInertia FromKilogramSquareMeters(double kilogramSquareMeters) => new MomentOfInertia(kilogramSquareMeters);
    public double ToKilogramSquareMeters() => kilogramSquareMeters;
    public static MomentOfInertia FromKilogramSquareCentimeters(double kilogramSquareCentimeters) => new MomentOfInertia(kilogramSquareCentimeters * (1e-4));
    public double ToKilogramSquareCentimeters() => kilogramSquareMeters / (1e-4);
    public static MomentOfInertia FromGramSquareCentimeters(double gramSquareCentimeters) => new MomentOfInertia(gramSquareCentimeters * (1e-7));
    public double ToGramSquareCentimeters() => kilogramSquareMeters / (1e-7);
    public static MomentOfInertia FromPoundSquareFeet(double poundSquareFeet) => new MomentOfInertia(poundSquareFeet * (0.04214011009380));
    public double ToPoundSquareFeet() => kilogramSquareMeters / (0.04214011009380);

    // Composite relationships
    public static Mass operator /(MomentOfInertia momentOfInertia, Area area) => Mass.FromKilograms(momentOfInertia.ToKilogramSquareMeters() / area.ToSquareMeters());
    public static Area operator /(MomentOfInertia momentOfInertia, Mass mass) => Area.FromSquareMeters(momentOfInertia.ToKilogramSquareMeters() / mass.ToKilograms());
    public static AngularMomentum operator *(MomentOfInertia momentOfInertia, AngularVelocity angularVelocity) => AngularMomentum.FromKilogramSquareMetersPerSecond(momentOfInertia.ToKilogramSquareMeters() * angularVelocity.ToRadiansPerSecond());
}
