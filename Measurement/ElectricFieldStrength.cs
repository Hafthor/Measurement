namespace com.hafthor.Measurement;

[Measurement("V/m", VariableName = "voltsPerMeter")]
[SiUnit("VoltsPerMeter", 0, "None Kilo")]
[SiUnit("VoltsPerCentimeter", 2)]
public readonly partial struct ElectricFieldStrength {
    // Composite relationships
    public static Voltage operator *(ElectricFieldStrength electricFieldStrength, Length length) => Voltage.FromVolts(electricFieldStrength.ToVoltsPerMeter() * length.ToMeters());
    public static Voltage operator *(Length length, ElectricFieldStrength electricFieldStrength) => Voltage.FromVolts(length.ToMeters() * electricFieldStrength.ToVoltsPerMeter());
}
