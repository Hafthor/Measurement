namespace com.hafthor.Measurement;

public sealed class Conductivity : Measurement<Conductivity> {

    private Conductivity(double value) : base(value) { }

    protected override Conductivity Create(double value) => new(value);
    protected override string Symbol => "S/m";

    // Units
    public static Conductivity FromSiemensPerMeter(double value) => new(value);
    public double ToSiemensPerMeter() => value;
    public static Conductivity FromSiemensPerCentimeter(double siemensPerCentimeter) => new(siemensPerCentimeter * (100));
    public double ToSiemensPerCentimeter() => value / (100);
    public static Conductivity FromMillisiemensPerCentimeter(double millisiemensPerCentimeter) => new(millisiemensPerCentimeter * (0.1));
    public double ToMillisiemensPerCentimeter() => value / (0.1);

    // Composite relationships
    public static ElectricConductance operator *(Conductivity conductivity, Length length) => ElectricConductance.FromSiemens(conductivity.ToSiemensPerMeter() * length.ToMeters());
    public static ElectricConductance operator *(Length length, Conductivity conductivity) => ElectricConductance.FromSiemens(length.ToMeters() * conductivity.ToSiemensPerMeter());

}
