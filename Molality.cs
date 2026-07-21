namespace com.hafthor.Measurement;

public sealed class Molality : Measurement<Molality> {

    private Molality(double value) : base(value) { }

    protected override Molality Create(double value) => new(value);
    protected override string Symbol => "mol/kg";

    // Units
    public static Molality FromMolesPerKilogram(double value) => new(value);
    public double ToMolesPerKilogram() => value;
    public static Molality FromMillimolesPerKilogram(double millimolesPerKilogram) => new(millimolesPerKilogram * (1e-3));
    public double ToMillimolesPerKilogram() => value / (1e-3);

    // Composite relationships
    public static Quantity operator *(Molality molality, Mass mass) => Quantity.FromMoles(molality.ToMolesPerKilogram() * mass.ToKilograms());
    public static Quantity operator *(Mass mass, Molality molality) => Quantity.FromMoles(mass.ToKilograms() * molality.ToMolesPerKilogram());

}
