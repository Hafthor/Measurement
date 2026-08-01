namespace com.hafthor.Measurement;

[Measurement("g/mol", VariableName = "gramsPerMole")]
[SiUnit("GramsPerMole", 0, "None Kilo")]
public readonly partial struct MolarMass {
    public static Mass operator *(MolarMass molarMass, Quantity quantity) => Mass.FromKilograms(molarMass.ToKilogramsPerMole() * quantity.ToMoles());
    public static Mass operator *(Quantity quantity, MolarMass molarMass) => Mass.FromKilograms(quantity.ToMoles() * molarMass.ToKilogramsPerMole());
}
