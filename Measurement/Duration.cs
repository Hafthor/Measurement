namespace com.hafthor.Measurement;

[Measurement("s", VariableName = "seconds")]
[SiUnit("Seconds", 0, "None Milli Micro Nano Pico Femto")]
[Unit("Minutes", 60)]
[Unit("Hours", 3600)]
[Unit("Days", 86400)]
[Unit("Weeks", 604800)]
[Unit("Fortnights", 1209600)]
[Unit("CommonYears", 31536000)]
[Unit("JulianYears", 31557600)]
[Unit("TropicalYears", 31556925.216)]
[Unit("SiderealYears", 31558149.7635)]
[Unit("SiderealDays", 86164.0905)]
[Unit("Decades", 31557600e1)]
[Unit("Centuries", 31557600e2)]
[Unit("Millennia", 31557600e3)]
[Unit("Annums", 31557600)]
[Unit("HubbleTimes", 4.803349612e17)]
[Unit("PlanckTimes", 5.391247e-44)]
public readonly partial struct Duration {
    // Frequency (reciprocal relationship: T = 1 / f)
    public static Duration FromFrequency(Frequency frequency) => new(1 / frequency.ToHertz());
    public Frequency ToFrequency() => Frequency.FromHertz(1 / seconds);
}
