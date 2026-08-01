namespace com.hafthor.Measurement;

[Measurement("A/m", VariableName = "amperesPerMeter")]
[SiUnit("AmperesPerMeter", 0)]
[Unit("Oersteds", 79.57747154594767)]
public readonly partial struct MagneticFieldStrength {
    // Composite relationships
    public static ElectricCurrent operator *(MagneticFieldStrength magneticFieldStrength, Length length) => ElectricCurrent.FromAmperes(magneticFieldStrength.ToAmperesPerMeter() * length.ToMeters());
    public static ElectricCurrent operator *(Length length, MagneticFieldStrength magneticFieldStrength) => ElectricCurrent.FromAmperes(length.ToMeters() * magneticFieldStrength.ToAmperesPerMeter());
}
