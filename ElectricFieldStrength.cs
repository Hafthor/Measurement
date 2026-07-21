namespace com.hafthor.Measurement;

public class ElectricFieldStrength {
    private readonly double voltsPerMeter;

    private ElectricFieldStrength(double voltsPerMeter) => this.voltsPerMeter = voltsPerMeter;

    // Arithmetic
    public static ElectricFieldStrength operator +(ElectricFieldStrength a, ElectricFieldStrength b) => new ElectricFieldStrength(a.voltsPerMeter + b.voltsPerMeter);
    public static ElectricFieldStrength operator -(ElectricFieldStrength a, ElectricFieldStrength b) => new ElectricFieldStrength(a.voltsPerMeter - b.voltsPerMeter);
    public static ElectricFieldStrength operator -(ElectricFieldStrength x) => new ElectricFieldStrength(-x.voltsPerMeter);

    // Units
    public static ElectricFieldStrength FromVoltsPerMeter(double voltsPerMeter) => new ElectricFieldStrength(voltsPerMeter);
    public double ToVoltsPerMeter() => voltsPerMeter;
    public static ElectricFieldStrength FromKilovoltsPerMeter(double kilovoltsPerMeter) => new ElectricFieldStrength(kilovoltsPerMeter * (1e3));
    public double ToKilovoltsPerMeter() => voltsPerMeter / (1e3);
    public static ElectricFieldStrength FromVoltsPerCentimeter(double voltsPerCentimeter) => new ElectricFieldStrength(voltsPerCentimeter * (100));
    public double ToVoltsPerCentimeter() => voltsPerMeter / (100);

    // Composite relationships
    public static Voltage operator *(ElectricFieldStrength electricFieldStrength, Length length) => Voltage.FromVolts(electricFieldStrength.ToVoltsPerMeter() * length.ToMeters());
    public static Voltage operator *(Length length, ElectricFieldStrength electricFieldStrength) => Voltage.FromVolts(length.ToMeters() * electricFieldStrength.ToVoltsPerMeter());
}
