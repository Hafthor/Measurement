namespace com.hafthor.Measurement;

[Measurement("g/mol", VariableName = "gramsPerMole")]
public readonly partial struct MolarMass {
    // Units
    public static MolarMass FromKilogramsPerMole(double kilogramsPerMole) => new(kilogramsPerMole * 1e3);
    public double ToKilogramsPerMole() => gramsPerMole / 1e3;
    public static MolarMass FromGramsPerMole(double gramsPerMole) => new(gramsPerMole);
    public double ToGramsPerMole() => gramsPerMole;

    // Composite relationships
    public static Mass operator *(MolarMass molarMass, Quantity quantity) => Mass.FromKilograms(molarMass.ToKilogramsPerMole() * quantity.ToMoles());
    public static Mass operator *(Quantity quantity, MolarMass molarMass) => Mass.FromKilograms(quantity.ToMoles() * molarMass.ToKilogramsPerMole());
}
