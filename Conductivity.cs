namespace com.hafthor.Measurement;

public class Conductivity {
    private readonly double siemensPerMeter;

    private Conductivity(double siemensPerMeter) => this.siemensPerMeter = siemensPerMeter;

    // Arithmetic
    public static Conductivity operator +(Conductivity a, Conductivity b) => new(a.siemensPerMeter + b.siemensPerMeter);
    public static Conductivity operator -(Conductivity a, Conductivity b) => new(a.siemensPerMeter - b.siemensPerMeter);
    public static Conductivity operator -(Conductivity x) => new(-x.siemensPerMeter);

    // Units
    public static Conductivity FromSiemensPerMeter(double siemensPerMeter) => new(siemensPerMeter);
    public double ToSiemensPerMeter() => siemensPerMeter;
    public static Conductivity FromSiemensPerCentimeter(double siemensPerCentimeter) => new(siemensPerCentimeter * (100));
    public double ToSiemensPerCentimeter() => siemensPerMeter / (100);
    public static Conductivity FromMillisiemensPerCentimeter(double millisiemensPerCentimeter) => new(millisiemensPerCentimeter * (0.1));
    public double ToMillisiemensPerCentimeter() => siemensPerMeter / (0.1);

    // Composite relationships
    public static ElectricConductance operator *(Conductivity conductivity, Length length) => ElectricConductance.FromSiemens(conductivity.ToSiemensPerMeter() * length.ToMeters());
    public static ElectricConductance operator *(Length length, Conductivity conductivity) => ElectricConductance.FromSiemens(length.ToMeters() * conductivity.ToSiemensPerMeter());

    public override string ToString() => $"{siemensPerMeter} S/m";
}
