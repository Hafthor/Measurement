namespace com.hafthor.Measurement;

[Measurement("mol/g", VariableName = "molesPerGram")]
[SiUnit("MolesPerKilogram", -3, "None Milli")]
[SiUnit("MolesPerGram", 0)]
public readonly partial struct Molality {
    public static Quantity operator *(Molality molality, Mass mass) => Quantity.FromMoles(molality.ToMolesPerKilogram() * mass.ToKilograms());
    public static Quantity operator *(Mass mass, Molality molality) => Quantity.FromMoles(mass.ToKilograms() * molality.ToMolesPerKilogram());
}
