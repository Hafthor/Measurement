namespace com.hafthor.Measurement;

[Measurement("g·m²", VariableName = "gramSquareMeters")]
[SiUnit("GramSquareMeters", 0, "None Kilo")]
[SiUnit("GramSquareCentimeters", -4, "None Kilo")]
[Unit("PoundSquareFeet", 0.04214011009380e3)]
public readonly partial struct MomentOfInertia {
    // Composite relationships
    public static Mass operator /(MomentOfInertia momentOfInertia, Area area) => Mass.FromKilograms(momentOfInertia.ToKilogramSquareMeters() / area.ToSquareMeters());
    public static Area operator /(MomentOfInertia momentOfInertia, Mass mass) => Area.FromSquareMeters(momentOfInertia.ToKilogramSquareMeters() / mass.ToKilograms());
    public static AngularMomentum operator *(MomentOfInertia momentOfInertia, AngularVelocity angularVelocity) => AngularMomentum.FromKilogramSquareMetersPerSecond(momentOfInertia.ToKilogramSquareMeters() * angularVelocity.ToRadiansPerSecond());
}
