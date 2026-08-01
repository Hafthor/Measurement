namespace com.hafthor.Measurement;

[Measurement("J", VariableName = "joules")]
[SiUnit("Joules", 0, "None Giga Mega Kilo Milli")]
[SiUnit("Ergs", -7)]
[Unit("Kilocalories", 4184)]
[Unit("Calories", 4.184)]
[Unit("KilowattHours", 3.6e6)]
[Unit("WattHours", 3600)]
[Unit("Electronvolts", 1.602176634e-19)]
[Unit("BritishThermalUnits", 1055.05585262)]
[Unit("FootPounds", 1.3558179483314004)]
[Unit("TonsOfTnt", 4.184e9)]
public readonly partial struct Energy {
    // Composite relationships
    public static Force operator /(Energy energy, Length length) => Force.FromNewtons(energy.joules / length.ToMeters());
    public static Power operator /(Energy energy, Duration duration) => Power.FromWatts(energy.joules / duration.ToSeconds());

    // Composite relationships (derived)
    public static Action operator *(Energy energy, Duration duration) => Action.FromJouleSeconds(energy.joules * duration.ToSeconds());
    public static Momentum operator /(Energy energy, Speed speed) => Momentum.FromKilogramMetersPerSecond(energy.joules / speed.ToMetersPerSecond());
    public static Speed operator /(Energy energy, Momentum momentum) => Speed.FromMetersPerSecond(energy.joules / momentum.ToKilogramMetersPerSecond());
    public static Action operator /(Energy energy, Frequency frequency) => Action.FromJouleSeconds(energy.joules / frequency.ToHertz());
    public static Frequency operator /(Energy energy, Action action) => Frequency.FromHertz(energy.joules / action.ToJouleSeconds());

    // Famous relations
    public static ElectricCharge operator /(Energy energy, Voltage voltage) => ElectricCharge.FromCoulombs(energy.joules / voltage.ToVolts());
    public static Voltage operator /(Energy energy, ElectricCharge charge) => Voltage.FromVolts(energy.joules / charge.ToCoulombs());
    public static Volume operator /(Energy energy, Pressure pressure) => Volume.FromCubicMeters(energy.joules / pressure.ToPascals());
    public static Pressure operator /(Energy energy, Volume volume) => Pressure.FromPascals(energy.joules / volume.ToCubicMeters());
    public static HeatCapacity operator /(Energy energy, Temperature temperatureChange) => HeatCapacity.FromJoulesPerKelvin(energy.joules / temperatureChange.ToKelvin());
    public static Temperature operator /(Energy energy, HeatCapacity heatCapacity) => Temperature.FromKelvin(energy.joules / heatCapacity.ToJoulesPerKelvin());
}
