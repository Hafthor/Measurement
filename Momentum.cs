namespace com.hafthor.Measurement;

public class Momentum {
    private readonly double kilogramMetersPerSecond;

    private Momentum(double kilogramMetersPerSecond) => this.kilogramMetersPerSecond = kilogramMetersPerSecond;

    // Arithmetic
    public static Momentum operator +(Momentum a, Momentum b) => new(a.kilogramMetersPerSecond + b.kilogramMetersPerSecond);
    public static Momentum operator -(Momentum a, Momentum b) => new(a.kilogramMetersPerSecond - b.kilogramMetersPerSecond);
    public static Momentum operator -(Momentum x) => new(-x.kilogramMetersPerSecond);

    // Units
    public static Momentum FromKilogramMetersPerSecond(double kilogramMetersPerSecond) => new(kilogramMetersPerSecond);
    public double ToKilogramMetersPerSecond() => kilogramMetersPerSecond;
    public static Momentum FromNewtonSeconds(double newtonSeconds) => new(newtonSeconds);
    public double ToNewtonSeconds() => kilogramMetersPerSecond;

    // Composite relationships
    public static Mass operator /(Momentum momentum, Speed speed) => Mass.FromKilograms(momentum.ToKilogramMetersPerSecond() / speed.ToMetersPerSecond());
    public static Speed operator /(Momentum momentum, Mass mass) => Speed.FromMetersPerSecond(momentum.ToKilogramMetersPerSecond() / mass.ToKilograms());
    public static Energy operator *(Momentum momentum, Speed speed) => Energy.FromJoules(momentum.ToKilogramMetersPerSecond() * speed.ToMetersPerSecond());
    public static Energy operator *(Speed speed, Momentum momentum) => Energy.FromJoules(speed.ToMetersPerSecond() * momentum.ToKilogramMetersPerSecond());

    // Famous relations
    public static Force operator /(Momentum momentum, Duration duration) => Force.FromNewtons(momentum.ToKilogramMetersPerSecond() / duration.ToSeconds());

    public override string ToString() => $"{kilogramMetersPerSecond} kg·m/s";

    public override bool Equals(object obj) => obj is Momentum other && other.kilogramMetersPerSecond == kilogramMetersPerSecond;
    public override int GetHashCode() => kilogramMetersPerSecond.GetHashCode();
}
