namespace com.hafthor.Measurement;

public class Resistivity {
    private readonly double ohmMeters;

    private Resistivity(double ohmMeters) => this.ohmMeters = ohmMeters;

    // Arithmetic
    public static Resistivity operator +(Resistivity a, Resistivity b) => new(a.ohmMeters + b.ohmMeters);
    public static Resistivity operator -(Resistivity a, Resistivity b) => new(a.ohmMeters - b.ohmMeters);
    public static Resistivity operator -(Resistivity x) => new(-x.ohmMeters);

    // Units
    public static Resistivity FromOhmMeters(double ohmMeters) => new(ohmMeters);
    public double ToOhmMeters() => ohmMeters;
    public static Resistivity FromOhmCentimeters(double ohmCentimeters) => new(ohmCentimeters * (1e-2));
    public double ToOhmCentimeters() => ohmMeters / (1e-2);
    public static Resistivity FromMicroohmCentimeters(double microohmCentimeters) => new(microohmCentimeters * (1e-8));
    public double ToMicroohmCentimeters() => ohmMeters / (1e-8);

    // Composite relationships
    public static ElectricResistance operator /(Resistivity resistivity, Length length) => ElectricResistance.FromOhms(resistivity.ToOhmMeters() / length.ToMeters());
    public static Length operator /(Resistivity resistivity, ElectricResistance electricResistance) => Length.FromMeters(resistivity.ToOhmMeters() / electricResistance.ToOhms());

    public override string ToString() => $"{ohmMeters} Ω·m";
}
