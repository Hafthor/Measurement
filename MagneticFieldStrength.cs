namespace com.hafthor.Measurement;

public sealed class MagneticFieldStrength : Measurement<MagneticFieldStrength> {

    private MagneticFieldStrength(double value) : base(value) { }

    protected override MagneticFieldStrength Create(double value) => new(value);
    protected override string Symbol => "A/m";

    // Units
    public static MagneticFieldStrength FromAmperesPerMeter(double value) => new(value);
    public double ToAmperesPerMeter() => value;
    public static MagneticFieldStrength FromOersteds(double oersteds) => new(oersteds * (79.57747154594767));
    public double ToOersteds() => value / (79.57747154594767);

    // Composite relationships
    public static ElectricCurrent operator *(MagneticFieldStrength magneticFieldStrength, Length length) => ElectricCurrent.FromAmperes(magneticFieldStrength.ToAmperesPerMeter() * length.ToMeters());
    public static ElectricCurrent operator *(Length length, MagneticFieldStrength magneticFieldStrength) => ElectricCurrent.FromAmperes(length.ToMeters() * magneticFieldStrength.ToAmperesPerMeter());

}
