namespace com.hafthor.Measurement;

public sealed class Permeability : Measurement<Permeability> {

    private Permeability(double value) : base(value) { }

    protected override Permeability Create(double value) => new(value);
    protected override string Symbol => "H/m";

    // Units
    public static Permeability FromHenriesPerMeter(double value) => new(value);
    public double ToHenriesPerMeter() => value;

    // Composite relationships
    public static Inductance operator *(Permeability permeability, Length length) => Inductance.FromHenries(permeability.ToHenriesPerMeter() * length.ToMeters());
    public static Inductance operator *(Length length, Permeability permeability) => Inductance.FromHenries(length.ToMeters() * permeability.ToHenriesPerMeter());

}
