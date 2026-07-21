namespace com.hafthor.Measurement;

[Measurement("J")]
public readonly partial struct Energy {

    // SI units
    public static Energy FromGigajoules(double gigajoules) => new(gigajoules * 1e9);
    public double ToGigajoules() => value / 1e9;
    public static Energy FromMegajoules(double megajoules) => new(megajoules * 1e6);
    public double ToMegajoules() => value / 1e6;
    public static Energy FromKilojoules(double kilojoules) => new(kilojoules * 1e3);
    public double ToKilojoules() => value / 1e3;
    public static Energy FromJoules(double joules) => new(joules);
    public double ToJoules() => value;
    public static Energy FromMillijoules(double millijoules) => new(millijoules * 1e-3);
    public double ToMillijoules() => value / 1e-3;
    public static Energy FromErgs(double ergs) => new(ergs * 1e-7);
    public double ToErgs() => value / 1e-7;

    // Calorie units
    public static Energy FromKilocalories(double kilocalories) => new(kilocalories * 4184);
    public double ToKilocalories() => value / 4184;
    public static Energy FromCalories(double calories) => new(calories * 4.184);
    public double ToCalories() => value / 4.184;

    // Electrical units
    public static Energy FromKilowattHours(double kilowattHours) => new(kilowattHours * 3.6e6);
    public double ToKilowattHours() => value / 3.6e6;
    public static Energy FromWattHours(double wattHours) => new(wattHours * 3600);
    public double ToWattHours() => value / 3600;
    public static Energy FromElectronvolts(double electronvolts) => new(electronvolts * 1.602176634e-19);
    public double ToElectronvolts() => value / 1.602176634e-19;

    // Imperial & other units
    public static Energy FromBritishThermalUnits(double britishThermalUnits) => new(britishThermalUnits * 1055.05585262);
    public double ToBritishThermalUnits() => value / 1055.05585262;
    public static Energy FromFootPounds(double footPounds) => new(footPounds * 1.3558179483314004);
    public double ToFootPounds() => value / 1.3558179483314004;
    public static Energy FromTonsOfTnt(double tonsOfTnt) => new(tonsOfTnt * 4.184e9);
    public double ToTonsOfTnt() => value / 4.184e9;

    // Composite relationships
    public static Force operator /(Energy energy, Length length) => Force.FromNewtons(energy.value / length.ToMeters());
    public static Power operator /(Energy energy, Duration duration) => Power.FromWatts(energy.value / duration.ToSeconds());

    // Composite relationships (derived)
    public static Action operator *(Energy energy, Duration duration) => Action.FromJouleSeconds(energy.ToJoules() * duration.ToSeconds());
    public static Momentum operator /(Energy energy, Speed speed) => Momentum.FromKilogramMetersPerSecond(energy.value / speed.ToMetersPerSecond());
    public static Speed operator /(Energy energy, Momentum momentum) => Speed.FromMetersPerSecond(energy.value / momentum.ToKilogramMetersPerSecond());
    public static Action operator /(Energy energy, Frequency frequency) => Action.FromJouleSeconds(energy.value / frequency.ToHertz());
    public static Frequency operator /(Energy energy, Action action) => Frequency.FromHertz(energy.value / action.ToJouleSeconds());

    // Famous relations
    public static ElectricCharge operator /(Energy energy, Voltage voltage) => ElectricCharge.FromCoulombs(energy.value / voltage.ToVolts());
    public static Voltage operator /(Energy energy, ElectricCharge charge) => Voltage.FromVolts(energy.value / charge.ToCoulombs());
    public static Volume operator /(Energy energy, Pressure pressure) => Volume.FromCubicMeters(energy.value / pressure.ToPascals());
    public static Pressure operator /(Energy energy, Volume volume) => Pressure.FromPascals(energy.value / volume.ToCubicMeters());
    public static HeatCapacity operator /(Energy energy, Temperature temperatureChange) => HeatCapacity.FromJoulesPerKelvin(energy.value / temperatureChange.ToKelvin());
    public static Temperature operator /(Energy energy, HeatCapacity heatCapacity) => Temperature.FromKelvin(energy.value / heatCapacity.ToJoulesPerKelvin());

}
