namespace com.hafthor.Measurement;

[Measurement("H/m", VariableName = "henriesPerMeter")]
[SiUnit("HenriesPerMeter", 0)]
public readonly partial struct Permeability {
    // Composite relationships
    public static Inductance operator *(Permeability permeability, Length length) => Inductance.FromHenries(permeability.ToHenriesPerMeter() * length.ToMeters());
    public static Inductance operator *(Length length, Permeability permeability) => Inductance.FromHenries(length.ToMeters() * permeability.ToHenriesPerMeter());
}
