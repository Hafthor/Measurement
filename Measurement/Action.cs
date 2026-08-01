namespace com.hafthor.Measurement;

[Measurement("J·s", VariableName = "nanoJouleSeconds", DisplayFactor = 1e9)]
[SiUnit("JouleSeconds", 9)]
[SiUnit("ErgSeconds", 2)]
[Unit("PlanckConstants", 6.62607015e-25)]
public readonly partial struct Action {
    // Composite relationships
    public static Energy operator /(Action action, Duration duration) => Energy.FromJoules(action.ToJouleSeconds() / duration.ToSeconds());
    public static Duration operator /(Action action, Energy energy) => Duration.FromSeconds(action.ToJouleSeconds() / energy.ToJoules());
    public static Energy operator *(Action action, Frequency frequency) => Energy.FromJoules(action.ToJouleSeconds() * frequency.ToHertz());
    public static Energy operator *(Frequency frequency, Action action) => Energy.FromJoules(frequency.ToHertz() * action.ToJouleSeconds());

    // Famous relations
    public static Length operator /(Action action, Momentum momentum) => Length.FromMeters(action.ToJouleSeconds() / momentum.ToKilogramMetersPerSecond());
    public static Momentum operator /(Action action, Length length) => Momentum.FromKilogramMetersPerSecond(action.ToJouleSeconds() / length.ToMeters());
}
