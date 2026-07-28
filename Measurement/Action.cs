namespace com.hafthor.Measurement;

[Measurement("J·s", VariableName = "nanoJouleSeconds", DisplayFactor = 1e9)]
public readonly partial struct Action {
    // Units
    public static Action FromJouleSeconds(double jouleSeconds) => new(jouleSeconds * 1e9);
    public double ToJouleSeconds() => nanoJouleSeconds / 1e9;
    public static Action FromErgSeconds(double ergSeconds) => new(ergSeconds * 1e2);
    public double ToErgSeconds() => nanoJouleSeconds / 1e2;
    public static Action FromPlanckConstants(double planckConstants) => new(planckConstants * (6.62607015e-25));
    public double ToPlanckConstants() => nanoJouleSeconds / (6.62607015e-25);

    // Composite relationships
    public static Energy operator /(Action action, Duration duration) => Energy.FromJoules(action.ToJouleSeconds() / duration.ToSeconds());
    public static Duration operator /(Action action, Energy energy) => Duration.FromSeconds(action.ToJouleSeconds() / energy.ToJoules());
    public static Energy operator *(Action action, Frequency frequency) => Energy.FromJoules(action.ToJouleSeconds() * frequency.ToHertz());
    public static Energy operator *(Frequency frequency, Action action) => Energy.FromJoules(frequency.ToHertz() * action.ToJouleSeconds());

    // Famous relations
    public static Length operator /(Action action, Momentum momentum) => Length.FromMeters(action.ToJouleSeconds() / momentum.ToKilogramMetersPerSecond());
    public static Momentum operator /(Action action, Length length) => Momentum.FromKilogramMetersPerSecond(action.ToJouleSeconds() / length.ToMeters());
}
