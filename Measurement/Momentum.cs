namespace com.hafthor.Measurement;

[Measurement("kg·m/s")]
public readonly partial struct Momentum {

    // Units
    public static Momentum FromKilogramMetersPerSecond(double kilogramMetersPerSecond) => new(kilogramMetersPerSecond);
    public double ToKilogramMetersPerSecond() => value;
    public static Momentum FromNewtonSeconds(double newtonSeconds) => new(newtonSeconds);
    public double ToNewtonSeconds() => value;

    // Composite relationships
    public static Mass operator /(Momentum momentum, Speed speed) => Mass.FromKilograms(momentum.ToKilogramMetersPerSecond() / speed.ToMetersPerSecond());
    public static Speed operator /(Momentum momentum, Mass mass) => Speed.FromMetersPerSecond(momentum.ToKilogramMetersPerSecond() / mass.ToKilograms());
    public static Energy operator *(Momentum momentum, Speed speed) => Energy.FromJoules(momentum.ToKilogramMetersPerSecond() * speed.ToMetersPerSecond());
    public static Energy operator *(Speed speed, Momentum momentum) => Energy.FromJoules(speed.ToMetersPerSecond() * momentum.ToKilogramMetersPerSecond());

    // Famous relations
    public static Force operator /(Momentum momentum, Duration duration) => Force.FromNewtons(momentum.ToKilogramMetersPerSecond() / duration.ToSeconds());

}
