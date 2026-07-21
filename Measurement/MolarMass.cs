namespace com.hafthor.Measurement;

[Measurement("kg/mol")]
public readonly partial struct MolarMass {

    // Units
    public static MolarMass FromKilogramsPerMole(double kilogramsPerMole) => new(kilogramsPerMole);
    public double ToKilogramsPerMole() => value;
    public static MolarMass FromGramsPerMole(double gramsPerMole) => new(gramsPerMole * (1e-3));
    public double ToGramsPerMole() => value / (1e-3);

    // Composite relationships
    public static Mass operator *(MolarMass molarMass, Quantity quantity) => Mass.FromKilograms(molarMass.ToKilogramsPerMole() * quantity.ToMoles());
    public static Mass operator *(Quantity quantity, MolarMass molarMass) => Mass.FromKilograms(quantity.ToMoles() * molarMass.ToKilogramsPerMole());

}
