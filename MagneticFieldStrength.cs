namespace com.hafthor.Measurement;

public class MagneticFieldStrength {
    private readonly double amperesPerMeter;

    private MagneticFieldStrength(double amperesPerMeter) => this.amperesPerMeter = amperesPerMeter;

    // Arithmetic
    public static MagneticFieldStrength operator +(MagneticFieldStrength a, MagneticFieldStrength b) => new MagneticFieldStrength(a.amperesPerMeter + b.amperesPerMeter);
    public static MagneticFieldStrength operator -(MagneticFieldStrength a, MagneticFieldStrength b) => new MagneticFieldStrength(a.amperesPerMeter - b.amperesPerMeter);
    public static MagneticFieldStrength operator -(MagneticFieldStrength x) => new MagneticFieldStrength(-x.amperesPerMeter);

    // Units
    public static MagneticFieldStrength FromAmperesPerMeter(double amperesPerMeter) => new MagneticFieldStrength(amperesPerMeter);
    public double ToAmperesPerMeter() => amperesPerMeter;
    public static MagneticFieldStrength FromOersteds(double oersteds) => new MagneticFieldStrength(oersteds * (79.57747154594767));
    public double ToOersteds() => amperesPerMeter / (79.57747154594767);

    // Composite relationships
    public static ElectricCurrent operator *(MagneticFieldStrength magneticFieldStrength, Length length) => ElectricCurrent.FromAmperes(magneticFieldStrength.ToAmperesPerMeter() * length.ToMeters());
    public static ElectricCurrent operator *(Length length, MagneticFieldStrength magneticFieldStrength) => ElectricCurrent.FromAmperes(length.ToMeters() * magneticFieldStrength.ToAmperesPerMeter());
}
