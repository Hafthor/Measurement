namespace com.hafthor.Measurement;

[Measurement("A")]
public readonly partial struct ElectricCurrent {

    // SI units
    public static ElectricCurrent FromKiloamperes(double kiloamperes) => new(kiloamperes * 1e3);
    public double ToKiloamperes() => value / 1e3;
    public static ElectricCurrent FromAmperes(double amperes) => new(amperes);
    public double ToAmperes() => value;
    public static ElectricCurrent FromMilliamperes(double milliamperes) => new(milliamperes * 1e-3);
    public double ToMilliamperes() => value / 1e-3;
    public static ElectricCurrent FromMicroamperes(double microamperes) => new(microamperes * 1e-6);
    public double ToMicroamperes() => value / 1e-6;
    public static ElectricCurrent FromNanoamperes(double nanoamperes) => new(nanoamperes * 1e-9);
    public double ToNanoamperes() => value / 1e-9;
    public static ElectricCurrent FromPicoamperes(double picoamperes) => new(picoamperes * 1e-12);
    public double ToPicoamperes() => value / 1e-12;

    // Electromagnetic (CGS) units
    public static ElectricCurrent FromAbamperes(double abamperes) => new(abamperes * 10);
    public double ToAbamperes() => value / 10;
    public static ElectricCurrent FromStatamperes(double statamperes) => new(statamperes * 3.335641e-10);
    public double ToStatamperes() => value / 3.335641e-10;

    // Composite relationships
    public static ElectricCharge operator *(ElectricCurrent current, Duration duration) => ElectricCharge.FromCoulombs(current.value * duration.ToSeconds());
    public static Voltage operator *(ElectricCurrent current, ElectricResistance resistance) => Voltage.FromVolts(current.value * resistance.ToOhms());
    public static Power operator *(ElectricCurrent current, Voltage voltage) => Power.FromWatts(current.value * voltage.ToVolts());
    public static MagneticFlux operator *(ElectricCurrent current, Inductance inductance) => MagneticFlux.FromWebers(current.value * inductance.ToHenries());

    // Composite relationships (derived)
    public static CurrentDensity operator /(ElectricCurrent electricCurrent, Area area) => CurrentDensity.FromAmperesPerSquareMeter(electricCurrent.ToAmperes() / area.ToSquareMeters());
    public static MagneticFieldStrength operator /(ElectricCurrent electricCurrent, Length length) => MagneticFieldStrength.FromAmperesPerMeter(electricCurrent.ToAmperes() / length.ToMeters());

}
