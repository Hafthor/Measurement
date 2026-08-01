namespace com.hafthor.Measurement;

[Measurement("g·m²/s", VariableName = "gramSquareMetersPerSecond")]
[SiUnit("GramSquareMetersPerSecond", 0, "None Kilo")]
[SiUnit("JouleSeconds", 3)]
[SiUnit("NewtonMeterSeconds", 3)]
public readonly partial struct AngularMomentum {
    // Composite relationships
    public static MomentOfInertia operator /(AngularMomentum angularMomentum, AngularVelocity angularVelocity) => MomentOfInertia.FromKilogramSquareMeters(angularMomentum.ToKilogramSquareMetersPerSecond() / angularVelocity.ToRadiansPerSecond());
    public static AngularVelocity operator /(AngularMomentum angularMomentum, MomentOfInertia momentOfInertia) => AngularVelocity.FromRadiansPerSecond(angularMomentum.ToKilogramSquareMetersPerSecond() / momentOfInertia.ToKilogramSquareMeters());
}
