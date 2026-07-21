namespace com.hafthor.Measurement;

[Measurement("mol/kg")]
public readonly partial struct Molality {

    // Units
    public static Molality FromMolesPerKilogram(double molesPerKilogram) => new(molesPerKilogram);
    public double ToMolesPerKilogram() => value;
    public static Molality FromMillimolesPerKilogram(double millimolesPerKilogram) => new(millimolesPerKilogram * (1e-3));
    public double ToMillimolesPerKilogram() => value / (1e-3);

    // Composite relationships
    public static Quantity operator *(Molality molality, Mass mass) => Quantity.FromMoles(molality.ToMolesPerKilogram() * mass.ToKilograms());
    public static Quantity operator *(Mass mass, Molality molality) => Quantity.FromMoles(mass.ToKilograms() * molality.ToMolesPerKilogram());

}
