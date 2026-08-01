namespace com.hafthor.Measurement;

[Measurement("A", VariableName = "microamperes", DisplayFactor = 1e6)]
[SiUnit("Amperes", 6, "None Kilo Milli Micro Nano Pico")]
[SiUnit("Abamperes", 7)]
[Unit("Statamperes", 3.335641e-4)]
public readonly partial struct ElectricCurrent {
    // Composite relationships
    public static ElectricCharge operator *(ElectricCurrent current, Duration duration) => ElectricCharge.FromCoulombs(current.ToAmperes() * duration.ToSeconds());
    public static Voltage operator *(ElectricCurrent current, ElectricResistance resistance) => Voltage.FromVolts(current.ToAmperes() * resistance.ToOhms());
    public static Power operator *(ElectricCurrent current, Voltage voltage) => Power.FromWatts(current.ToAmperes() * voltage.ToVolts());
    public static MagneticFlux operator *(ElectricCurrent current, Inductance inductance) => MagneticFlux.FromWebers(current.ToAmperes() * inductance.ToHenries());

    // Composite relationships (derived)
    public static CurrentDensity operator /(ElectricCurrent electricCurrent, Area area) => CurrentDensity.FromAmperesPerSquareMeter(electricCurrent.ToAmperes() / area.ToSquareMeters());
    public static MagneticFieldStrength operator /(ElectricCurrent electricCurrent, Length length) => MagneticFieldStrength.FromAmperesPerMeter(electricCurrent.ToAmperes() / length.ToMeters());
}
