namespace com.hafthor.Measurement;

public class ReactionRate {
    private readonly double molesPerCubicMeterSecond;

    private ReactionRate(double molesPerCubicMeterSecond) => this.molesPerCubicMeterSecond = molesPerCubicMeterSecond;

    // Arithmetic
    public static ReactionRate operator +(ReactionRate a, ReactionRate b) => new ReactionRate(a.molesPerCubicMeterSecond + b.molesPerCubicMeterSecond);
    public static ReactionRate operator -(ReactionRate a, ReactionRate b) => new ReactionRate(a.molesPerCubicMeterSecond - b.molesPerCubicMeterSecond);
    public static ReactionRate operator -(ReactionRate x) => new ReactionRate(-x.molesPerCubicMeterSecond);

    // Units
    public static ReactionRate FromMolesPerCubicMeterSecond(double molesPerCubicMeterSecond) => new ReactionRate(molesPerCubicMeterSecond);
    public double ToMolesPerCubicMeterSecond() => molesPerCubicMeterSecond;
    public static ReactionRate FromMolesPerLiterSecond(double molesPerLiterSecond) => new ReactionRate(molesPerLiterSecond * (1000));
    public double ToMolesPerLiterSecond() => molesPerCubicMeterSecond / (1000);

    // Composite relationships
    public static Concentration operator *(ReactionRate reactionRate, Duration duration) => Concentration.FromMolesPerCubicMeter(reactionRate.ToMolesPerCubicMeterSecond() * duration.ToSeconds());
    public static Concentration operator *(Duration duration, ReactionRate reactionRate) => Concentration.FromMolesPerCubicMeter(duration.ToSeconds() * reactionRate.ToMolesPerCubicMeterSecond());
}
