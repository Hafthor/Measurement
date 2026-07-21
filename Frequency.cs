namespace com.hafthor.Measurement;

public class Frequency {
    private readonly double hertz;

    private Frequency(double hertz) => this.hertz = hertz;

    // Arithmetic
    public static Frequency operator +(Frequency a, Frequency b) => new Frequency(a.hertz + b.hertz);
    public static Frequency operator -(Frequency a, Frequency b) => new Frequency(a.hertz - b.hertz);
    public static Frequency operator -(Frequency x) => new Frequency(-x.hertz);

    // SI units
    public static Frequency FromTerahertz(double terahertz) => new Frequency(terahertz * 1e12);
    public double ToTerahertz() => hertz / 1e12;
    public static Frequency FromGigahertz(double gigahertz) => new Frequency(gigahertz * 1e9);
    public double ToGigahertz() => hertz / 1e9;
    public static Frequency FromMegahertz(double megahertz) => new Frequency(megahertz * 1e6);
    public double ToMegahertz() => hertz / 1e6;
    public static Frequency FromKilohertz(double kilohertz) => new Frequency(kilohertz * 1e3);
    public double ToKilohertz() => hertz / 1e3;
    public static Frequency FromHertz(double hertz) => new Frequency(hertz);
    public double ToHertz() => hertz;
    public static Frequency FromMillihertz(double millihertz) => new Frequency(millihertz * 1e-3);
    public double ToMillihertz() => hertz / 1e-3;

    // Rotational units
    public static Frequency FromRevolutionsPerMinute(double revolutionsPerMinute) => new Frequency(revolutionsPerMinute / 60);
    public double ToRevolutionsPerMinute() => hertz * 60;

    // Period (reciprocal of a duration: f = 1 / T)
    public static Frequency FromPeriod(Duration period) => new Frequency(1 / period.ToSeconds());
    public Duration ToPeriod() => Duration.FromSeconds(1 / hertz);

    // Famous relations
    public static Speed operator *(Frequency frequency, Length length) => Speed.FromMetersPerSecond(frequency.ToHertz() * length.ToMeters());
}
