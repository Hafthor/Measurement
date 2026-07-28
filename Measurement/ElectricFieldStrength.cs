namespace com.hafthor.Measurement;

[Measurement("V/m", VariableName = "voltsPerMeter")]
public readonly partial struct ElectricFieldStrength {
    // Units
    public static ElectricFieldStrength FromVoltsPerMeter(double voltsPerMeter) => new(voltsPerMeter);
    public double ToVoltsPerMeter() => voltsPerMeter;
    public static ElectricFieldStrength FromKilovoltsPerMeter(double kilovoltsPerMeter) => new(kilovoltsPerMeter * (1e3));
    public double ToKilovoltsPerMeter() => voltsPerMeter / (1e3);
    public static ElectricFieldStrength FromVoltsPerCentimeter(double voltsPerCentimeter) => new(voltsPerCentimeter * (100));
    public double ToVoltsPerCentimeter() => voltsPerMeter / (100);

    // Composite relationships
    public static Voltage operator *(ElectricFieldStrength electricFieldStrength, Length length) => Voltage.FromVolts(electricFieldStrength.ToVoltsPerMeter() * length.ToMeters());
    public static Voltage operator *(Length length, ElectricFieldStrength electricFieldStrength) => Voltage.FromVolts(length.ToMeters() * electricFieldStrength.ToVoltsPerMeter());
}
