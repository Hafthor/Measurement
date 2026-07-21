namespace com.hafthor.Measurement;

public sealed class ElectricFieldStrength : Measurement<ElectricFieldStrength> {

    private ElectricFieldStrength(double value) : base(value) { }

    protected override ElectricFieldStrength Create(double value) => new(value);
    protected override string Symbol => "V/m";

    // Units
    public static ElectricFieldStrength FromVoltsPerMeter(double value) => new(value);
    public double ToVoltsPerMeter() => value;
    public static ElectricFieldStrength FromKilovoltsPerMeter(double kilovoltsPerMeter) => new(kilovoltsPerMeter * (1e3));
    public double ToKilovoltsPerMeter() => value / (1e3);
    public static ElectricFieldStrength FromVoltsPerCentimeter(double voltsPerCentimeter) => new(voltsPerCentimeter * (100));
    public double ToVoltsPerCentimeter() => value / (100);

    // Composite relationships
    public static Voltage operator *(ElectricFieldStrength electricFieldStrength, Length length) => Voltage.FromVolts(electricFieldStrength.ToVoltsPerMeter() * length.ToMeters());
    public static Voltage operator *(Length length, ElectricFieldStrength electricFieldStrength) => Voltage.FromVolts(length.ToMeters() * electricFieldStrength.ToVoltsPerMeter());

}
