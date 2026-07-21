namespace com.hafthor.Measurement;

[Measurement("Hz")]
public readonly partial struct Frequency {

    // SI units
    public static Frequency FromTerahertz(double terahertz) => new(terahertz * 1e12);
    public double ToTerahertz() => value / 1e12;
    public static Frequency FromGigahertz(double gigahertz) => new(gigahertz * 1e9);
    public double ToGigahertz() => value / 1e9;
    public static Frequency FromMegahertz(double megahertz) => new(megahertz * 1e6);
    public double ToMegahertz() => value / 1e6;
    public static Frequency FromKilohertz(double kilohertz) => new(kilohertz * 1e3);
    public double ToKilohertz() => value / 1e3;
    public static Frequency FromHertz(double hertz) => new(hertz);
    public double ToHertz() => value;
    public static Frequency FromMillihertz(double millihertz) => new(millihertz * 1e-3);
    public double ToMillihertz() => value / 1e-3;

    // Rotational units
    public static Frequency FromRevolutionsPerMinute(double revolutionsPerMinute) => new(revolutionsPerMinute / 60);
    public double ToRevolutionsPerMinute() => value * 60;

    // Period (reciprocal of a duration: f = 1 / T)
    public static Frequency FromPeriod(Duration period) => new(1 / period.ToSeconds());
    public Duration ToPeriod() => Duration.FromSeconds(1 / value);

    // Famous relations
    public static Speed operator *(Frequency frequency, Length length) => Speed.FromMetersPerSecond(frequency.ToHertz() * length.ToMeters());

}
