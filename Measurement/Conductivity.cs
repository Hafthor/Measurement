namespace com.hafthor.Measurement;

[Measurement("S/m", VariableName = "millisiemensPerCentimeter", DisplayFactor = 10)]
public readonly partial struct Conductivity {
    // Units
    public static Conductivity FromSiemensPerMeter(double siemensPerMeter) => new(siemensPerMeter * 10);
    public double ToSiemensPerMeter() => millisiemensPerCentimeter / 10;
    public static Conductivity FromSiemensPerCentimeter(double siemensPerCentimeter) => new(siemensPerCentimeter * 1e3);
    public double ToSiemensPerCentimeter() => millisiemensPerCentimeter / 1e3;
    public static Conductivity FromMillisiemensPerCentimeter(double millisiemensPerCentimeter) => new(millisiemensPerCentimeter);
    public double ToMillisiemensPerCentimeter() => millisiemensPerCentimeter;

    // Composite relationships
    public static ElectricConductance operator *(Conductivity conductivity, Length length) => ElectricConductance.FromSiemens(conductivity.ToSiemensPerMeter() * length.ToMeters());
    public static ElectricConductance operator *(Length length, Conductivity conductivity) => ElectricConductance.FromSiemens(length.ToMeters() * conductivity.ToSiemensPerMeter());

    // Reciprocal quantity (resistivity ρ = 1/σ)
    public Resistivity ToResistivity() => Resistivity.FromOhmMeters(1.0 / ToSiemensPerMeter());
}
