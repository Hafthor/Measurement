namespace com.hafthor.Measurement;

public class Power {
    private readonly double watts;

    private Power(double watts) => this.watts = watts;

    // Arithmetic
    public static Power operator +(Power a, Power b) => new(a.watts + b.watts);
    public static Power operator -(Power a, Power b) => new(a.watts - b.watts);
    public static Power operator -(Power x) => new(-x.watts);

    // SI units
    public static Power FromGigawatts(double gigawatts) => new(gigawatts * 1e9);
    public double ToGigawatts() => watts / 1e9;
    public static Power FromMegawatts(double megawatts) => new(megawatts * 1e6);
    public double ToMegawatts() => watts / 1e6;
    public static Power FromKilowatts(double kilowatts) => new(kilowatts * 1e3);
    public double ToKilowatts() => watts / 1e3;
    public static Power FromWatts(double watts) => new(watts);
    public double ToWatts() => watts;
    public static Power FromMilliwatts(double milliwatts) => new(milliwatts * 1e-3);
    public double ToMilliwatts() => watts / 1e-3;

    // Horsepower units
    public static Power FromHorsepower(double horsepower) => new(horsepower * 745.6998715822702);
    public double ToHorsepower() => watts / 745.6998715822702;
    public static Power FromMetricHorsepower(double metricHorsepower) => new(metricHorsepower * 735.49875);
    public double ToMetricHorsepower() => watts / 735.49875;

    // Thermal & other units
    public static Power FromBritishThermalUnitsPerHour(double britishThermalUnitsPerHour) => new(britishThermalUnitsPerHour * 0.29307107017);
    public double ToBritishThermalUnitsPerHour() => watts / 0.29307107017;
    public static Power FromFootPoundsPerSecond(double footPoundsPerSecond) => new(footPoundsPerSecond * 1.3558179483314004);
    public double ToFootPoundsPerSecond() => watts / 1.3558179483314004;

    // Composite relationships
    public static Energy operator *(Power power, Duration duration) => Energy.FromJoules(power.watts * duration.ToSeconds());
    public static Force operator /(Power power, Speed speed) => Force.FromNewtons(power.watts / speed.ToMetersPerSecond());
    public static Speed operator /(Power power, Force force) => Speed.FromMetersPerSecond(power.watts / force.ToNewtons());
    public static Voltage operator /(Power power, ElectricCurrent current) => Voltage.FromVolts(power.watts / current.ToAmperes());
    public static ElectricCurrent operator /(Power power, Voltage voltage) => ElectricCurrent.FromAmperes(power.watts / voltage.ToVolts());

    // Composite relationships (derived)
    public static HeatFluxDensity operator /(Power power, Area area) => HeatFluxDensity.FromWattsPerSquareMeter(power.ToWatts() / area.ToSquareMeters());
    public static RadiantIntensity operator /(Power power, SolidAngle solidAngle) => RadiantIntensity.FromWattsPerSteradian(power.ToWatts() / solidAngle.ToSteradians());

    public override string ToString() => $"{watts} W";
}
