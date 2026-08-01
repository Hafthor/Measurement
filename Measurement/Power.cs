namespace com.hafthor.Measurement;

[Measurement("W", VariableName = "watts")]
[SiUnit("Watts", 0, "None Giga Mega Kilo Milli")]
[Unit("Horsepower", 745.6998715822702)]
[Unit("MetricHorsepower", 735.49875)]
[Unit("BritishThermalUnitsPerHour", 0.29307107017)]
[Unit("FootPoundsPerSecond", 1.3558179483314004)]
public readonly partial struct Power {
    // Composite relationships
    public static Energy operator *(Power power, Duration duration) => Energy.FromJoules(power.watts * duration.ToSeconds());
    public static Force operator /(Power power, Speed speed) => Force.FromNewtons(power.watts / speed.ToMetersPerSecond());
    public static Speed operator /(Power power, Force force) => Speed.FromMetersPerSecond(power.watts / force.ToNewtons());
    public static Voltage operator /(Power power, ElectricCurrent current) => Voltage.FromVolts(power.watts / current.ToAmperes());
    public static ElectricCurrent operator /(Power power, Voltage voltage) => ElectricCurrent.FromAmperes(power.watts / voltage.ToVolts());

    // Composite relationships (derived)
    public static HeatFluxDensity operator /(Power power, Area area) => HeatFluxDensity.FromWattsPerSquareMeter(power.ToWatts() / area.ToSquareMeters());
    public static RadiantIntensity operator /(Power power, SolidAngle solidAngle) => RadiantIntensity.FromWattsPerSteradian(power.ToWatts() / solidAngle.ToSteradians());
}
