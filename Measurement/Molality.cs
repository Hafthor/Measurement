namespace com.hafthor.Measurement;

[Measurement("mol/g", VariableName = "molesPerGram")]
public readonly partial struct Molality {
    // Units. Canonical is mol/g; mass is in the denominator so 1 mol/kg = 1e-3 mol/g.
    public static Molality FromMolesPerKilogram(double molesPerKilogram) => new(molesPerKilogram * 1e-3);
    public double ToMolesPerKilogram() => molesPerGram / 1e-3;
    public static Molality FromMillimolesPerKilogram(double millimolesPerKilogram) => new(millimolesPerKilogram * 1e-6);
    public double ToMillimolesPerKilogram() => molesPerGram / 1e-6;
    public static Molality FromMolesPerGram(double molesPerGram) => new(molesPerGram);
    public double ToMolesPerGram() => molesPerGram;

    // Composite relationships
    public static Quantity operator *(Molality molality, Mass mass) => Quantity.FromMoles(molality.ToMolesPerKilogram() * mass.ToKilograms());
    public static Quantity operator *(Mass mass, Molality molality) => Quantity.FromMoles(mass.ToKilograms() * molality.ToMolesPerKilogram());
}
