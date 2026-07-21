namespace com.hafthor.Measurement;

[Measurement("H/m")]
public readonly partial struct Permeability {

    // Units
    public static Permeability FromHenriesPerMeter(double henriesPerMeter) => new(henriesPerMeter);
    public double ToHenriesPerMeter() => value;

    // Composite relationships
    public static Inductance operator *(Permeability permeability, Length length) => Inductance.FromHenries(permeability.ToHenriesPerMeter() * length.ToMeters());
    public static Inductance operator *(Length length, Permeability permeability) => Inductance.FromHenries(length.ToMeters() * permeability.ToHenriesPerMeter());

}
