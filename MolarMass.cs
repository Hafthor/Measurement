namespace com.hafthor.Measurement;

public class MolarMass {
    private readonly double kilogramsPerMole;

    private MolarMass(double kilogramsPerMole) => this.kilogramsPerMole = kilogramsPerMole;

    // Arithmetic
    public static MolarMass operator +(MolarMass a, MolarMass b) => new(a.kilogramsPerMole + b.kilogramsPerMole);
    public static MolarMass operator -(MolarMass a, MolarMass b) => new(a.kilogramsPerMole - b.kilogramsPerMole);
    public static MolarMass operator -(MolarMass x) => new(-x.kilogramsPerMole);

    // Units
    public static MolarMass FromKilogramsPerMole(double kilogramsPerMole) => new(kilogramsPerMole);
    public double ToKilogramsPerMole() => kilogramsPerMole;
    public static MolarMass FromGramsPerMole(double gramsPerMole) => new(gramsPerMole * (1e-3));
    public double ToGramsPerMole() => kilogramsPerMole / (1e-3);

    // Composite relationships
    public static Mass operator *(MolarMass molarMass, Quantity quantity) => Mass.FromKilograms(molarMass.ToKilogramsPerMole() * quantity.ToMoles());
    public static Mass operator *(Quantity quantity, MolarMass molarMass) => Mass.FromKilograms(quantity.ToMoles() * molarMass.ToKilogramsPerMole());
}
