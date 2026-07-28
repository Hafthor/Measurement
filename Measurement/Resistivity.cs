namespace com.hafthor.Measurement;

[Measurement("Ω·m", VariableName = "microohmCentimeters", DisplayFactor = 1e8)]
public readonly partial struct Resistivity {
    // Units
    public static Resistivity FromOhmMeters(double ohmMeters) => new(ohmMeters * 1e8);
    public double ToOhmMeters() => microohmCentimeters / 1e8;
    public static Resistivity FromOhmCentimeters(double ohmCentimeters) => new(ohmCentimeters * (1e6));
    public double ToOhmCentimeters() => microohmCentimeters / (1e6);
    public static Resistivity FromMicroohmCentimeters(double microohmCentimeters) => new(microohmCentimeters);
    public double ToMicroohmCentimeters() => microohmCentimeters;

    // Composite relationships
    public static ElectricResistance operator /(Resistivity resistivity, Length length) => ElectricResistance.FromOhms(resistivity.ToOhmMeters() / length.ToMeters());
    public static Length operator /(Resistivity resistivity, ElectricResistance electricResistance) => Length.FromMeters(resistivity.ToOhmMeters() / electricResistance.ToOhms());

    // Reciprocal quantity (conductivity σ = 1/ρ)
    public Conductivity ToConductivity() => Conductivity.FromSiemensPerMeter(1.0 / ToOhmMeters());
}
