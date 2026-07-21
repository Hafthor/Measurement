namespace com.hafthor.Measurement;

public class ElectricCurrent {
    private readonly double amperes;

    private ElectricCurrent(double amperes) => this.amperes = amperes;

    // Arithmetic
    public static ElectricCurrent operator +(ElectricCurrent a, ElectricCurrent b) => new(a.amperes + b.amperes);
    public static ElectricCurrent operator -(ElectricCurrent a, ElectricCurrent b) => new(a.amperes - b.amperes);
    public static ElectricCurrent operator -(ElectricCurrent x) => new(-x.amperes);

    // SI units
    public static ElectricCurrent FromKiloamperes(double kiloamperes) => new(kiloamperes * 1e3);
    public double ToKiloamperes() => amperes / 1e3;
    public static ElectricCurrent FromAmperes(double amperes) => new(amperes);
    public double ToAmperes() => amperes;
    public static ElectricCurrent FromMilliamperes(double milliamperes) => new(milliamperes * 1e-3);
    public double ToMilliamperes() => amperes / 1e-3;
    public static ElectricCurrent FromMicroamperes(double microamperes) => new(microamperes * 1e-6);
    public double ToMicroamperes() => amperes / 1e-6;
    public static ElectricCurrent FromNanoamperes(double nanoamperes) => new(nanoamperes * 1e-9);
    public double ToNanoamperes() => amperes / 1e-9;
    public static ElectricCurrent FromPicoamperes(double picoamperes) => new(picoamperes * 1e-12);
    public double ToPicoamperes() => amperes / 1e-12;

    // Electromagnetic (CGS) units
    public static ElectricCurrent FromAbamperes(double abamperes) => new(abamperes * 10);
    public double ToAbamperes() => amperes / 10;
    public static ElectricCurrent FromStatamperes(double statamperes) => new(statamperes * 3.335641e-10);
    public double ToStatamperes() => amperes / 3.335641e-10;

    // Composite relationships
    public static ElectricCharge operator *(ElectricCurrent current, Duration duration) => ElectricCharge.FromCoulombs(current.amperes * duration.ToSeconds());
    public static Voltage operator *(ElectricCurrent current, ElectricResistance resistance) => Voltage.FromVolts(current.amperes * resistance.ToOhms());
    public static Power operator *(ElectricCurrent current, Voltage voltage) => Power.FromWatts(current.amperes * voltage.ToVolts());
    public static MagneticFlux operator *(ElectricCurrent current, Inductance inductance) => MagneticFlux.FromWebers(current.amperes * inductance.ToHenries());

    // Composite relationships (derived)
    public static CurrentDensity operator /(ElectricCurrent electricCurrent, Area area) => CurrentDensity.FromAmperesPerSquareMeter(electricCurrent.ToAmperes() / area.ToSquareMeters());
    public static MagneticFieldStrength operator /(ElectricCurrent electricCurrent, Length length) => MagneticFieldStrength.FromAmperesPerMeter(electricCurrent.ToAmperes() / length.ToMeters());

    public override string ToString() => $"{amperes} A";
}
