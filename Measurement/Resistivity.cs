namespace com.hafthor.Measurement;

[Measurement("Ω·m")]
public readonly partial struct Resistivity {

    // Units
    public static Resistivity FromOhmMeters(double ohmMeters) => new(ohmMeters);
    public double ToOhmMeters() => value;
    public static Resistivity FromOhmCentimeters(double ohmCentimeters) => new(ohmCentimeters * (1e-2));
    public double ToOhmCentimeters() => value / (1e-2);
    public static Resistivity FromMicroohmCentimeters(double microohmCentimeters) => new(microohmCentimeters * (1e-8));
    public double ToMicroohmCentimeters() => value / (1e-8);

    // Composite relationships
    public static ElectricResistance operator /(Resistivity resistivity, Length length) => ElectricResistance.FromOhms(resistivity.ToOhmMeters() / length.ToMeters());
    public static Length operator /(Resistivity resistivity, ElectricResistance electricResistance) => Length.FromMeters(resistivity.ToOhmMeters() / electricResistance.ToOhms());

    // Reciprocal quantity (conductivity σ = 1/ρ)
    public Conductivity ToConductivity() => Conductivity.FromSiemensPerMeter(1.0 / ToOhmMeters());

}
