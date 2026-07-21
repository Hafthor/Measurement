namespace com.hafthor.Measurement;

public sealed class Momentum : Measurement<Momentum> {

    private Momentum(double value) : base(value) { }

    protected override Momentum Create(double value) => new(value);
    protected override string Symbol => "kg·m/s";

    // Units
    public static Momentum FromKilogramMetersPerSecond(double value) => new(value);
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
