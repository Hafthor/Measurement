namespace com.hafthor.Measurement;

public class Energy {
    private readonly double joules;

    private Energy(double joules) => this.joules = joules;

    // Arithmetic
    public static Energy operator +(Energy a, Energy b) => new(a.joules + b.joules);
    public static Energy operator -(Energy a, Energy b) => new(a.joules - b.joules);
    public static Energy operator -(Energy x) => new(-x.joules);

    // SI units
    public static Energy FromGigajoules(double gigajoules) => new(gigajoules * 1e9);
    public double ToGigajoules() => joules / 1e9;
    public static Energy FromMegajoules(double megajoules) => new(megajoules * 1e6);
    public double ToMegajoules() => joules / 1e6;
    public static Energy FromKilojoules(double kilojoules) => new(kilojoules * 1e3);
    public double ToKilojoules() => joules / 1e3;
    public static Energy FromJoules(double joules) => new(joules);
    public double ToJoules() => joules;
    public static Energy FromMillijoules(double millijoules) => new(millijoules * 1e-3);
    public double ToMillijoules() => joules / 1e-3;
    public static Energy FromErgs(double ergs) => new(ergs * 1e-7);
    public double ToErgs() => joules / 1e-7;

    // Calorie units
    public static Energy FromKilocalories(double kilocalories) => new(kilocalories * 4184);
    public double ToKilocalories() => joules / 4184;
    public static Energy FromCalories(double calories) => new(calories * 4.184);
    public double ToCalories() => joules / 4.184;

    // Electrical units
    public static Energy FromKilowattHours(double kilowattHours) => new(kilowattHours * 3.6e6);
    public double ToKilowattHours() => joules / 3.6e6;
    public static Energy FromWattHours(double wattHours) => new(wattHours * 3600);
    public double ToWattHours() => joules / 3600;
    public static Energy FromElectronvolts(double electronvolts) => new(electronvolts * 1.602176634e-19);
    public double ToElectronvolts() => joules / 1.602176634e-19;

    // Imperial & other units
    public static Energy FromBritishThermalUnits(double britishThermalUnits) => new(britishThermalUnits * 1055.05585262);
    public double ToBritishThermalUnits() => joules / 1055.05585262;
    public static Energy FromFootPounds(double footPounds) => new(footPounds * 1.3558179483314004);
    public double ToFootPounds() => joules / 1.3558179483314004;
    public static Energy FromTonsOfTnt(double tonsOfTnt) => new(tonsOfTnt * 4.184e9);
    public double ToTonsOfTnt() => joules / 4.184e9;

    // Composite relationships
    public static Force operator /(Energy energy, Length length) => Force.FromNewtons(energy.joules / length.ToMeters());
    public static Power operator /(Energy energy, Duration duration) => Power.FromWatts(energy.joules / duration.ToSeconds());

    // Composite relationships (derived)
    public static Action operator *(Energy energy, Duration duration) => Action.FromJouleSeconds(energy.ToJoules() * duration.ToSeconds());
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

    public override string ToString() => $"{joules} J";

    public override bool Equals(object obj) => obj is Energy other && other.joules == joules;
    public override int GetHashCode() => joules.GetHashCode();
}
