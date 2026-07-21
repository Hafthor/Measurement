namespace com.hafthor.Measurement;

public class Molality {
    private readonly double molesPerKilogram;

    private Molality(double molesPerKilogram) => this.molesPerKilogram = molesPerKilogram;

    // Arithmetic
    public static Molality operator +(Molality a, Molality b) => new(a.molesPerKilogram + b.molesPerKilogram);
    public static Molality operator -(Molality a, Molality b) => new(a.molesPerKilogram - b.molesPerKilogram);
    public static Molality operator -(Molality x) => new(-x.molesPerKilogram);

    // Units
    public static Molality FromMolesPerKilogram(double molesPerKilogram) => new(molesPerKilogram);
    public double ToMolesPerKilogram() => molesPerKilogram;
    public static Molality FromMillimolesPerKilogram(double millimolesPerKilogram) => new(millimolesPerKilogram * (1e-3));
    public double ToMillimolesPerKilogram() => molesPerKilogram / (1e-3);

    // Composite relationships
    public static Quantity operator *(Molality molality, Mass mass) => Quantity.FromMoles(molality.ToMolesPerKilogram() * mass.ToKilograms());
    public static Quantity operator *(Mass mass, Molality molality) => Quantity.FromMoles(mass.ToKilograms() * molality.ToMolesPerKilogram());

    public override string ToString() => $"{molesPerKilogram} mol/kg";

    public override bool Equals(object obj) => obj is Molality other && other.molesPerKilogram == molesPerKilogram;
    public override int GetHashCode() => molesPerKilogram.GetHashCode();
}
