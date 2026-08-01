namespace com.hafthor.Measurement;

[Measurement("mol/(m³·s)", VariableName = "molesPerCubicMeterSecond")]
[SiUnit("MolesPerCubicMeterSecond", 0)]
[SiUnit("MolesPerLiterSecond", 3)]
public readonly partial struct ReactionRate {
    // Composite relationships
    public static Concentration operator *(ReactionRate reactionRate, Duration duration) => Concentration.FromMolesPerCubicMeter(reactionRate.ToMolesPerCubicMeterSecond() * duration.ToSeconds());
    public static Concentration operator *(Duration duration, ReactionRate reactionRate) => Concentration.FromMolesPerCubicMeter(duration.ToSeconds() * reactionRate.ToMolesPerCubicMeterSecond());
}
