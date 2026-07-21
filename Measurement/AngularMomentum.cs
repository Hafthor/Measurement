namespace com.hafthor.Measurement;

[Measurement("kg·m²/s")]
public readonly partial struct AngularMomentum {

    // Units
    public static AngularMomentum FromKilogramSquareMetersPerSecond(double kilogramSquareMetersPerSecond) => new(kilogramSquareMetersPerSecond);
    public double ToKilogramSquareMetersPerSecond() => value;
    public static AngularMomentum FromJouleSeconds(double jouleSeconds) => new(jouleSeconds);
    public double ToJouleSeconds() => value;
    public static AngularMomentum FromNewtonMeterSeconds(double newtonMeterSeconds) => new(newtonMeterSeconds);
    public double ToNewtonMeterSeconds() => value;

    // Composite relationships
    public static MomentOfInertia operator /(AngularMomentum angularMomentum, AngularVelocity angularVelocity) => MomentOfInertia.FromKilogramSquareMeters(angularMomentum.ToKilogramSquareMetersPerSecond() / angularVelocity.ToRadiansPerSecond());
    public static AngularVelocity operator /(AngularMomentum angularMomentum, MomentOfInertia momentOfInertia) => AngularVelocity.FromRadiansPerSecond(angularMomentum.ToKilogramSquareMetersPerSecond() / momentOfInertia.ToKilogramSquareMeters());

}
