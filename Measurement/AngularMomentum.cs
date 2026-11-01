namespace com.hafthor.Measurement;

[Measurement("g·m²/s", VariableName = "gramSquareMetersPerSecond")]
[SiUnit("GramSquareMetersPerSecond", 0, "None Kilo")]
[SiUnit("JouleSeconds", 3)]
[SiUnit("NewtonMeterSeconds", 3)]
[Product<Energy, Duration>]
[Product<MomentOfInertia, AngularVelocity>]
public readonly partial struct AngularMomentum;
