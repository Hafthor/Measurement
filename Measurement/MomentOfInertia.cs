namespace com.hafthor.Measurement;

[Measurement("g·m²", VariableName = "gramSquareMeters")]
[SiUnit("GramSquareMeters", 0, "None Kilo")]
[SiUnit("GramSquareCentimeters", -4, "None Kilo")]
[Unit("PoundSquareFeet", 0.04214011009380e3)]
[Product<Area, Mass>]
public readonly partial struct MomentOfInertia {
    // Composite relationships
    public static AngularMomentum operator *(MomentOfInertia momentOfInertia, AngularVelocity angularVelocity) => AngularMomentum.FromKilogramSquareMetersPerSecond(momentOfInertia.ToKilogramSquareMeters() * angularVelocity.ToRadiansPerSecond());
}
