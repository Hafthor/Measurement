namespace com.hafthor.Measurement;

[Measurement("A/m", VariableName = "amperesPerMeter")]
public readonly partial struct MagneticFieldStrength {
    // Units
    public static MagneticFieldStrength FromAmperesPerMeter(double amperesPerMeter) => new(amperesPerMeter);
    public double ToAmperesPerMeter() => amperesPerMeter;
    public static MagneticFieldStrength FromOersteds(double oersteds) => new(oersteds * (79.57747154594767));
    public double ToOersteds() => amperesPerMeter / (79.57747154594767);

    // Composite relationships
    public static ElectricCurrent operator *(MagneticFieldStrength magneticFieldStrength, Length length) => ElectricCurrent.FromAmperes(magneticFieldStrength.ToAmperesPerMeter() * length.ToMeters());
    public static ElectricCurrent operator *(Length length, MagneticFieldStrength magneticFieldStrength) => ElectricCurrent.FromAmperes(length.ToMeters() * magneticFieldStrength.ToAmperesPerMeter());
}
