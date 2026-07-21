namespace com.hafthor.Measurement;

[Measurement("mol/(m³·s)")]
public readonly partial struct ReactionRate {

    // Units
    public static ReactionRate FromMolesPerCubicMeterSecond(double molesPerCubicMeterSecond) => new(molesPerCubicMeterSecond);
    public double ToMolesPerCubicMeterSecond() => value;
    public static ReactionRate FromMolesPerLiterSecond(double molesPerLiterSecond) => new(molesPerLiterSecond * (1000));
    public double ToMolesPerLiterSecond() => value / (1000);

    // Composite relationships
    public static Concentration operator *(ReactionRate reactionRate, Duration duration) => Concentration.FromMolesPerCubicMeter(reactionRate.ToMolesPerCubicMeterSecond() * duration.ToSeconds());
    public static Concentration operator *(Duration duration, ReactionRate reactionRate) => Concentration.FromMolesPerCubicMeter(duration.ToSeconds() * reactionRate.ToMolesPerCubicMeterSecond());

}
