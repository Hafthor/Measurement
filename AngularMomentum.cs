namespace com.hafthor.Measurement;

public sealed class AngularMomentum : Measurement<AngularMomentum> {

    private AngularMomentum(double value) : base(value) { }

    protected override AngularMomentum Create(double value) => new(value);
    protected override string Symbol => "kg·m²/s";

    // Units
    public static AngularMomentum FromKilogramSquareMetersPerSecond(double value) => new(value);
    public double ToKilogramSquareMetersPerSecond() => value;
    public static AngularMomentum FromJouleSeconds(double jouleSeconds) => new(jouleSeconds);
    public double ToJouleSeconds() => value;
    public static AngularMomentum FromNewtonMeterSeconds(double newtonMeterSeconds) => new(newtonMeterSeconds);
    public double ToNewtonMeterSeconds() => value;

    // Composite relationships
    public static MomentOfInertia operator /(AngularMomentum angularMomentum, AngularVelocity angularVelocity) => MomentOfInertia.FromKilogramSquareMeters(angularMomentum.ToKilogramSquareMetersPerSecond() / angularVelocity.ToRadiansPerSecond());
    public static AngularVelocity operator /(AngularMomentum angularMomentum, MomentOfInertia momentOfInertia) => AngularVelocity.FromRadiansPerSecond(angularMomentum.ToKilogramSquareMetersPerSecond() / momentOfInertia.ToKilogramSquareMeters());

}
