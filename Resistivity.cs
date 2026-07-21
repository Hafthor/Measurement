namespace com.hafthor.Measurement;

public sealed class Resistivity : Measurement<Resistivity> {

    private Resistivity(double value) : base(value) { }

    protected override Resistivity Create(double value) => new(value);
    protected override string Symbol => "Ω·m";

    // Units
    public static Resistivity FromOhmMeters(double value) => new(value);
    public double ToOhmMeters() => value;
    public static Resistivity FromOhmCentimeters(double ohmCentimeters) => new(ohmCentimeters * (1e-2));
    public double ToOhmCentimeters() => value / (1e-2);
    public static Resistivity FromMicroohmCentimeters(double microohmCentimeters) => new(microohmCentimeters * (1e-8));
    public double ToMicroohmCentimeters() => value / (1e-8);

    // Composite relationships
    public static ElectricResistance operator /(Resistivity resistivity, Length length) => ElectricResistance.FromOhms(resistivity.ToOhmMeters() / length.ToMeters());
    public static Length operator /(Resistivity resistivity, ElectricResistance electricResistance) => Length.FromMeters(resistivity.ToOhmMeters() / electricResistance.ToOhms());

}
