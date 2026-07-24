namespace com.hafthor.Measurement;

[Measurement("A", DisplayFactor = 1e6)]
public readonly partial struct ElectricCurrent {

    // Canonical (stored) unit is the microampere, so µA/mA-scale values land on exact
    // integers in IEEE-754; ToString presents amperes (DisplayFactor = 1e6).
    public static ElectricCurrent FromKiloamperes(double kiloamperes) => new(kiloamperes * 1e9);
    public double ToKiloamperes() => value / 1e9;
    public static ElectricCurrent FromAmperes(double amperes) => new(amperes * 1e6);
    public double ToAmperes() => value / 1e6;
    public static ElectricCurrent FromMilliamperes(double milliamperes) => new(milliamperes * 1e3);
    public double ToMilliamperes() => value / 1e3;
    public static ElectricCurrent FromMicroamperes(double microamperes) => new(microamperes);
    public double ToMicroamperes() => value;
    public static ElectricCurrent FromNanoamperes(double nanoamperes) => new(nanoamperes * 1e-3);
    public double ToNanoamperes() => value / 1e-3;
    public static ElectricCurrent FromPicoamperes(double picoamperes) => new(picoamperes * 1e-6);
    public double ToPicoamperes() => value / 1e-6;

    // Electromagnetic (CGS) units
    public static ElectricCurrent FromAbamperes(double abamperes) => new(abamperes * 1e7);
    public double ToAbamperes() => value / 1e7;
    public static ElectricCurrent FromStatamperes(double statamperes) => new(statamperes * 3.335641e-4);
    public double ToStatamperes() => value / 3.335641e-4;

    // Composite relationships
    public static ElectricCharge operator *(ElectricCurrent current, Duration duration) => ElectricCharge.FromCoulombs(current.ToAmperes() * duration.ToSeconds());
    public static Voltage operator *(ElectricCurrent current, ElectricResistance resistance) => Voltage.FromVolts(current.ToAmperes() * resistance.ToOhms());
    public static Power operator *(ElectricCurrent current, Voltage voltage) => Power.FromWatts(current.ToAmperes() * voltage.ToVolts());
    public static MagneticFlux operator *(ElectricCurrent current, Inductance inductance) => MagneticFlux.FromWebers(current.ToAmperes() * inductance.ToHenries());

    // Composite relationships (derived)
    public static CurrentDensity operator /(ElectricCurrent electricCurrent, Area area) => CurrentDensity.FromAmperesPerSquareMeter(electricCurrent.ToAmperes() / area.ToSquareMeters());
    public static MagneticFieldStrength operator /(ElectricCurrent electricCurrent, Length length) => MagneticFieldStrength.FromAmperesPerMeter(electricCurrent.ToAmperes() / length.ToMeters());

}
