namespace com.hafthor.Measurement;

[Measurement("g·m²/s", VariableName = "gramSquareMetersPerSecond")]
[SiUnit("GramSquareMetersPerSecond", 0, "None Kilo")]
[SiUnit("JouleSeconds", 3)]
[SiUnit("NewtonMeterSeconds", 3)]
[Product<Energy, Duration>]
public readonly partial struct AngularMomentum {
    public static MomentOfInertia operator /(AngularMomentum angularMomentum, AngularVelocity angularVelocity) => MomentOfInertia.FromGramSquareMeters(angularMomentum.ToGramSquareMetersPerSecond() / angularVelocity.ToRadiansPerSecond());
    public static AngularVelocity operator /(AngularMomentum angularMomentum, MomentOfInertia momentOfInertia) => AngularVelocity.FromRadiansPerSecond(angularMomentum.ToGramSquareMetersPerSecond() / momentOfInertia.ToGramSquareMeters());
}
