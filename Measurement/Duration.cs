namespace com.hafthor.Measurement;

[Measurement("s")]
public readonly partial struct Duration {

    // SI units
    public static Duration FromSeconds(double seconds) => new(seconds);
    public double ToSeconds() => value;
    public static Duration FromMilliseconds(double milliseconds) => new(milliseconds * 1e-3);
    public double ToMilliseconds() => value / 1e-3;
    public static Duration FromMicroseconds(double microseconds) => new(microseconds * 1e-6);
    public double ToMicroseconds() => value / 1e-6;
    public static Duration FromNanoseconds(double nanoseconds) => new(nanoseconds * 1e-9);
    public double ToNanoseconds() => value / 1e-9;
    public static Duration FromPicoseconds(double picoseconds) => new(picoseconds * 1e-12);
    public double ToPicoseconds() => value / 1e-12;
    public static Duration FromFemtoseconds(double femtoseconds) => new(femtoseconds * 1e-15);
    public double ToFemtoseconds() => value / 1e-15;

    // Common units
    public static Duration FromMinutes(double minutes) => new(minutes * 60);
    public double ToMinutes() => value / 60;
    public static Duration FromHours(double hours) => new(hours * 3600);
    public double ToHours() => value / 3600;
    public static Duration FromDays(double days) => new(days * 86400);
    public double ToDays() => value / 86400;
    public static Duration FromWeeks(double weeks) => new(weeks * 604800);
    public double ToWeeks() => value / 604800;
    public static Duration FromFortnights(double fortnights) => new(fortnights * 1209600);
    public double ToFortnights() => value / 1209600;

    // Calendar & astronomical units (based on the Julian year = 365.25 days)
    public static Duration FromCommonYears(double commonYears) => new(commonYears * 31536000);
    public double ToCommonYears() => value / 31536000;
    public static Duration FromJulianYears(double julianYears) => new(julianYears * 31557600);
    public double ToJulianYears() => value / 31557600;
    public static Duration FromTropicalYears(double tropicalYears) => new(tropicalYears * 31556925.216);
    public double ToTropicalYears() => value / 31556925.216;
    public static Duration FromSiderealYears(double siderealYears) => new(siderealYears * 31558149.7635);
    public double ToSiderealYears() => value / 31558149.7635;
    public static Duration FromSiderealDays(double siderealDays) => new(siderealDays * 86164.0905);
    public double ToSiderealDays() => value / 86164.0905;
    public static Duration FromDecades(double decades) => new(decades * 315576000);
    public double ToDecades() => value / 315576000;
    public static Duration FromCenturies(double centuries) => new(centuries * 3155760000);
    public double ToCenturies() => value / 3155760000;
    public static Duration FromMillennia(double millennia) => new(millennia * 31557600000);
    public double ToMillennia() => value / 31557600000;
    public static Duration FromAnnums(double annums) => new(annums * 31557600);
    public double ToAnnums() => value / 31557600;
    public static Duration FromMegaannums(double megaannums) => new(megaannums * 31557600000000);
    public double ToMegaannums() => value / 31557600000000;
    public static Duration FromGigaannums(double gigaannums) => new(gigaannums * 31557600000000000);
    public double ToGigaannums() => value / 31557600000000000;

    // Cosmological units (Hubble time = Hubble length / c)
    public static Duration FromHubbleTimes(double hubbleTimes) => new(hubbleTimes * 4.803349612e17);
    public double ToHubbleTimes() => value / 4.803349612e17;

    // Scientific units
    public static Duration FromPlanckTimes(double planckTimes) => new(planckTimes * 5.391247e-44);
    public double ToPlanckTimes() => value / 5.391247e-44;

    // Frequency (reciprocal relationship: T = 1 / f)
    public static Duration FromFrequency(Frequency frequency) => new(1 / frequency.ToHertz());
    public Frequency ToFrequency() => Frequency.FromHertz(1 / value);

    // Composite relationships
    public static Length operator *(Duration duration, Speed speed) => Length.FromMeters(duration.value * speed.ToMetersPerSecond());
    public static Speed operator *(Duration duration, Acceleration acceleration) => Speed.FromMetersPerSecond(duration.value * acceleration.ToMetersPerSecondSquared());
    public static Energy operator *(Duration duration, Power power) => Energy.FromJoules(duration.value * power.ToWatts());
    public static ElectricCharge operator *(Duration duration, ElectricCurrent current) => ElectricCharge.FromCoulombs(duration.value * current.ToAmperes());
    public static MagneticFlux operator *(Duration duration, Voltage voltage) => MagneticFlux.FromWebers(duration.value * voltage.ToVolts());

    // Composite relationships (derived)
    public static DynamicViscosity operator *(Duration duration, Pressure pressure) => DynamicViscosity.FromPascalSeconds(duration.ToSeconds() * pressure.ToPascals());
    public static Action operator *(Duration duration, Energy energy) => Action.FromJouleSeconds(duration.ToSeconds() * energy.ToJoules());
    public static LuminousEnergy operator *(Duration duration, LuminousFlux luminousFlux) => LuminousEnergy.FromLumenSeconds(duration.ToSeconds() * luminousFlux.ToLumens());
    public static LuminousExposure operator *(Duration duration, Illuminance illuminance) => LuminousExposure.FromLuxSeconds(duration.ToSeconds() * illuminance.ToLux());

    // Famous relations
    public static Momentum operator *(Duration duration, Force force) => Momentum.FromKilogramMetersPerSecond(duration.ToSeconds() * force.ToNewtons());

}
