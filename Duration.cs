namespace com.hafthor.Measurement;

public class Duration {
    private readonly double seconds;

    private Duration(double seconds) => this.seconds = seconds;

    // Arithmetic
    public static Duration operator +(Duration a, Duration b) => new(a.seconds + b.seconds);
    public static Duration operator -(Duration a, Duration b) => new(a.seconds - b.seconds);
    public static Duration operator -(Duration x) => new(-x.seconds);

    // SI units
    public static Duration FromSeconds(double seconds) => new(seconds);
    public double ToSeconds() => seconds;
    public static Duration FromMilliseconds(double milliseconds) => new(milliseconds * 1e-3);
    public double ToMilliseconds() => seconds / 1e-3;
    public static Duration FromMicroseconds(double microseconds) => new(microseconds * 1e-6);
    public double ToMicroseconds() => seconds / 1e-6;
    public static Duration FromNanoseconds(double nanoseconds) => new(nanoseconds * 1e-9);
    public double ToNanoseconds() => seconds / 1e-9;
    public static Duration FromPicoseconds(double picoseconds) => new(picoseconds * 1e-12);
    public double ToPicoseconds() => seconds / 1e-12;
    public static Duration FromFemtoseconds(double femtoseconds) => new(femtoseconds * 1e-15);
    public double ToFemtoseconds() => seconds / 1e-15;

    // Common units
    public static Duration FromMinutes(double minutes) => new(minutes * 60);
    public double ToMinutes() => seconds / 60;
    public static Duration FromHours(double hours) => new(hours * 3600);
    public double ToHours() => seconds / 3600;
    public static Duration FromDays(double days) => new(days * 86400);
    public double ToDays() => seconds / 86400;
    public static Duration FromWeeks(double weeks) => new(weeks * 604800);
    public double ToWeeks() => seconds / 604800;
    public static Duration FromFortnights(double fortnights) => new(fortnights * 1209600);
    public double ToFortnights() => seconds / 1209600;

    // Calendar & astronomical units (based on the Julian year = 365.25 days)
    public static Duration FromCommonYears(double commonYears) => new(commonYears * 31536000);
    public double ToCommonYears() => seconds / 31536000;
    public static Duration FromJulianYears(double julianYears) => new(julianYears * 31557600);
    public double ToJulianYears() => seconds / 31557600;
    public static Duration FromTropicalYears(double tropicalYears) => new(tropicalYears * 31556925.216);
    public double ToTropicalYears() => seconds / 31556925.216;
    public static Duration FromSiderealYears(double siderealYears) => new(siderealYears * 31558149.7635);
    public double ToSiderealYears() => seconds / 31558149.7635;
    public static Duration FromSiderealDays(double siderealDays) => new(siderealDays * 86164.0905);
    public double ToSiderealDays() => seconds / 86164.0905;
    public static Duration FromDecades(double decades) => new(decades * 315576000);
    public double ToDecades() => seconds / 315576000;
    public static Duration FromCenturies(double centuries) => new(centuries * 3155760000);
    public double ToCenturies() => seconds / 3155760000;
    public static Duration FromMillennia(double millennia) => new(millennia * 31557600000);
    public double ToMillennia() => seconds / 31557600000;
    public static Duration FromMegaannums(double megaannums) => new(megaannums * 31557600000000);
    public double ToMegaannums() => seconds / 31557600000000;
    public static Duration FromGigaannums(double gigaannums) => new(gigaannums * 31557600000000000);
    public double ToGigaannums() => seconds / 31557600000000000;

    // Cosmological units (Hubble time = Hubble length / c)
    public static Duration FromHubbleTimes(double hubbleTimes) => new(hubbleTimes * 4.803349612e17);
    public double ToHubbleTimes() => seconds / 4.803349612e17;

    // Scientific units
    public static Duration FromPlanckTimes(double planckTimes) => new(planckTimes * 5.391247e-44);
    public double ToPlanckTimes() => seconds / 5.391247e-44;

    // Frequency (reciprocal relationship: T = 1 / f)
    public static Duration FromFrequency(Frequency frequency) => new(1 / frequency.ToHertz());
    public Frequency ToFrequency() => Frequency.FromHertz(1 / seconds);

    // Composite relationships
    public static Length operator *(Duration duration, Speed speed) => Length.FromMeters(duration.seconds * speed.ToMetersPerSecond());
    public static Speed operator *(Duration duration, Acceleration acceleration) => Speed.FromMetersPerSecond(duration.seconds * acceleration.ToMetersPerSecondSquared());
    public static Energy operator *(Duration duration, Power power) => Energy.FromJoules(duration.seconds * power.ToWatts());
    public static ElectricCharge operator *(Duration duration, ElectricCurrent current) => ElectricCharge.FromCoulombs(duration.seconds * current.ToAmperes());
    public static MagneticFlux operator *(Duration duration, Voltage voltage) => MagneticFlux.FromWebers(duration.seconds * voltage.ToVolts());

    // Composite relationships (derived)
    public static DynamicViscosity operator *(Duration duration, Pressure pressure) => DynamicViscosity.FromPascalSeconds(duration.ToSeconds() * pressure.ToPascals());
    public static Action operator *(Duration duration, Energy energy) => Action.FromJouleSeconds(duration.ToSeconds() * energy.ToJoules());
    public static LuminousEnergy operator *(Duration duration, LuminousFlux luminousFlux) => LuminousEnergy.FromLumenSeconds(duration.ToSeconds() * luminousFlux.ToLumens());
    public static LuminousExposure operator *(Duration duration, Illuminance illuminance) => LuminousExposure.FromLuxSeconds(duration.ToSeconds() * illuminance.ToLux());

    // Famous relations
    public static Momentum operator *(Duration duration, Force force) => Momentum.FromKilogramMetersPerSecond(duration.ToSeconds() * force.ToNewtons());

    public override string ToString() => $"{seconds} s";
}
