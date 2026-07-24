namespace com.hafthor.Measurement;

[Measurement("S/m")]
public readonly partial struct Conductivity {

    // Units
    public static Conductivity FromSiemensPerMeter(double siemensPerMeter) => new(siemensPerMeter);
    public double ToSiemensPerMeter() => value;
    public static Conductivity FromSiemensPerCentimeter(double siemensPerCentimeter) => new(siemensPerCentimeter * (100));
    public double ToSiemensPerCentimeter() => value / (100);
    public static Conductivity FromMillisiemensPerCentimeter(double millisiemensPerCentimeter) => new(millisiemensPerCentimeter * (0.1));
    public double ToMillisiemensPerCentimeter() => value / (0.1);

    // Composite relationships
    public static ElectricConductance operator *(Conductivity conductivity, Length length) => ElectricConductance.FromSiemens(conductivity.ToSiemensPerMeter() * length.ToMeters());
    public static ElectricConductance operator *(Length length, Conductivity conductivity) => ElectricConductance.FromSiemens(length.ToMeters() * conductivity.ToSiemensPerMeter());

    // Reciprocal quantity (resistivity ρ = 1/σ)
    public Resistivity ToResistivity() => Resistivity.FromOhmMeters(1.0 / ToSiemensPerMeter());

}
