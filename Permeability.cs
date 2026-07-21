namespace com.hafthor.Measurement;

public class Permeability {
    private readonly double henriesPerMeter;

    private Permeability(double henriesPerMeter) => this.henriesPerMeter = henriesPerMeter;

    // Arithmetic
    public static Permeability operator +(Permeability a, Permeability b) => new(a.henriesPerMeter + b.henriesPerMeter);
    public static Permeability operator -(Permeability a, Permeability b) => new(a.henriesPerMeter - b.henriesPerMeter);
    public static Permeability operator -(Permeability x) => new(-x.henriesPerMeter);

    // Units
    public static Permeability FromHenriesPerMeter(double henriesPerMeter) => new(henriesPerMeter);
    public double ToHenriesPerMeter() => henriesPerMeter;

    // Composite relationships
    public static Inductance operator *(Permeability permeability, Length length) => Inductance.FromHenries(permeability.ToHenriesPerMeter() * length.ToMeters());
    public static Inductance operator *(Length length, Permeability permeability) => Inductance.FromHenries(length.ToMeters() * permeability.ToHenriesPerMeter());

    public override string ToString() => $"{henriesPerMeter} H/m";
}
