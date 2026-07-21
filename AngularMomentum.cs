namespace com.hafthor.Measurement;

public class AngularMomentum {
    private readonly double kilogramSquareMetersPerSecond;

    private AngularMomentum(double kilogramSquareMetersPerSecond) => this.kilogramSquareMetersPerSecond = kilogramSquareMetersPerSecond;

    // Arithmetic
    public static AngularMomentum operator +(AngularMomentum a, AngularMomentum b) => new AngularMomentum(a.kilogramSquareMetersPerSecond + b.kilogramSquareMetersPerSecond);
    public static AngularMomentum operator -(AngularMomentum a, AngularMomentum b) => new AngularMomentum(a.kilogramSquareMetersPerSecond - b.kilogramSquareMetersPerSecond);
    public static AngularMomentum operator -(AngularMomentum x) => new AngularMomentum(-x.kilogramSquareMetersPerSecond);

    // Units
    public static AngularMomentum FromKilogramSquareMetersPerSecond(double kilogramSquareMetersPerSecond) => new AngularMomentum(kilogramSquareMetersPerSecond);
    public double ToKilogramSquareMetersPerSecond() => kilogramSquareMetersPerSecond;
    public static AngularMomentum FromJouleSeconds(double jouleSeconds) => new AngularMomentum(jouleSeconds);
    public double ToJouleSeconds() => kilogramSquareMetersPerSecond;
    public static AngularMomentum FromNewtonMeterSeconds(double newtonMeterSeconds) => new AngularMomentum(newtonMeterSeconds);
    public double ToNewtonMeterSeconds() => kilogramSquareMetersPerSecond;

    // Composite relationships
    public static MomentOfInertia operator /(AngularMomentum angularMomentum, AngularVelocity angularVelocity) => MomentOfInertia.FromKilogramSquareMeters(angularMomentum.ToKilogramSquareMetersPerSecond() / angularVelocity.ToRadiansPerSecond());
    public static AngularVelocity operator /(AngularMomentum angularMomentum, MomentOfInertia momentOfInertia) => AngularVelocity.FromRadiansPerSecond(angularMomentum.ToKilogramSquareMetersPerSecond() / momentOfInertia.ToKilogramSquareMeters());
}
