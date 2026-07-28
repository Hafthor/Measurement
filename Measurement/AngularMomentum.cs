namespace com.hafthor.Measurement;

[Measurement("g·m²/s", VariableName = "gramSquareMetersPerSecond")]
public readonly partial struct AngularMomentum {
    // Units
    public static AngularMomentum FromKilogramSquareMetersPerSecond(double kilogramSquareMetersPerSecond) => new(kilogramSquareMetersPerSecond * 1e3);
    public double ToKilogramSquareMetersPerSecond() => gramSquareMetersPerSecond / 1e3;
    public static AngularMomentum FromGramSquareMetersPerSecond(double gramSquareMetersPerSecond) => new(gramSquareMetersPerSecond);
    public double ToGramSquareMetersPerSecond() => gramSquareMetersPerSecond;
    public static AngularMomentum FromJouleSeconds(double jouleSeconds) => new(jouleSeconds * 1e3);
    public double ToJouleSeconds() => gramSquareMetersPerSecond / 1e3;
    public static AngularMomentum FromNewtonMeterSeconds(double newtonMeterSeconds) => new(newtonMeterSeconds  * 1e3);
    public double ToNewtonMeterSeconds() => gramSquareMetersPerSecond / 1e3;

    // Composite relationships
    public static MomentOfInertia operator /(AngularMomentum angularMomentum, AngularVelocity angularVelocity) => MomentOfInertia.FromKilogramSquareMeters(angularMomentum.ToKilogramSquareMetersPerSecond() / angularVelocity.ToRadiansPerSecond());
    public static AngularVelocity operator /(AngularMomentum angularMomentum, MomentOfInertia momentOfInertia) => AngularVelocity.FromRadiansPerSecond(angularMomentum.ToKilogramSquareMetersPerSecond() / momentOfInertia.ToKilogramSquareMeters());
}
