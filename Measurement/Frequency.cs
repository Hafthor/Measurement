namespace com.hafthor.Measurement;

[Measurement("Hz", VariableName = "hertz")]
[SiUnit("Hertz", 0, "None Tera Giga Mega Kilo Milli")]
[Unit("RevolutionsPerMinute", 1.0/(60))]
public readonly partial struct Frequency {
    // Period (reciprocal of a duration: f = 1 / T)
    public static Frequency FromPeriod(Duration period) => new(1 / period.ToSeconds());
    public Duration ToPeriod() => Duration.FromSeconds(1 / hertz);

}
