namespace com.hafthor.Measurement;

public sealed class ReactionRate : Measurement<ReactionRate> {

    private ReactionRate(double value) : base(value) { }

    protected override ReactionRate Create(double value) => new(value);
    protected override string Symbol => "mol/(m³·s)";

    // Units
    public static ReactionRate FromMolesPerCubicMeterSecond(double value) => new(value);
    public double ToMolesPerCubicMeterSecond() => value;
    public static ReactionRate FromMolesPerLiterSecond(double molesPerLiterSecond) => new(molesPerLiterSecond * (1000));
    public double ToMolesPerLiterSecond() => value / (1000);

    // Composite relationships
    public static Concentration operator *(ReactionRate reactionRate, Duration duration) => Concentration.FromMolesPerCubicMeter(reactionRate.ToMolesPerCubicMeterSecond() * duration.ToSeconds());
    public static Concentration operator *(Duration duration, ReactionRate reactionRate) => Concentration.FromMolesPerCubicMeter(duration.ToSeconds() * reactionRate.ToMolesPerCubicMeterSecond());

}
