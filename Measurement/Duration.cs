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
}
