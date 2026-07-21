namespace com.hafthor.Measurement;

public sealed class Action : Measurement<Action> {

    private Action(double value) : base(value) { }

    protected override Action Create(double value) => new(value);
    protected override string Symbol => "J·s";

    // Units
    public static Action FromJouleSeconds(double value) => new(value);
    public double ToJouleSeconds() => value;
    public static Action FromErgSeconds(double ergSeconds) => new(ergSeconds * (1e-7));
    public double ToErgSeconds() => value / (1e-7);
    public static Action FromPlanckConstants(double planckConstants) => new(planckConstants * (6.62607015e-34));
    public double ToPlanckConstants() => value / (6.62607015e-34);

    // Composite relationships
    public static Energy operator /(Action action, Duration duration) => Energy.FromJoules(action.ToJouleSeconds() / duration.ToSeconds());
    public static Duration operator /(Action action, Energy energy) => Duration.FromSeconds(action.ToJouleSeconds() / energy.ToJoules());
    public static Energy operator *(Action action, Frequency frequency) => Energy.FromJoules(action.ToJouleSeconds() * frequency.ToHertz());
    public static Energy operator *(Frequency frequency, Action action) => Energy.FromJoules(frequency.ToHertz() * action.ToJouleSeconds());

    // Famous relations
    public static Length operator /(Action action, Momentum momentum) => Length.FromMeters(action.ToJouleSeconds() / momentum.ToKilogramMetersPerSecond());
    public static Momentum operator /(Action action, Length length) => Momentum.FromKilogramMetersPerSecond(action.ToJouleSeconds() / length.ToMeters());

}
