namespace com.hafthor.Measurement;

public sealed class MolarMass : Measurement<MolarMass> {

    private MolarMass(double value) : base(value) { }

    protected override MolarMass Create(double value) => new(value);
    protected override string Symbol => "kg/mol";

    // Units
    public static MolarMass FromKilogramsPerMole(double value) => new(value);
    public double ToKilogramsPerMole() => value;
    public static MolarMass FromGramsPerMole(double gramsPerMole) => new(gramsPerMole * (1e-3));
    public double ToGramsPerMole() => value / (1e-3);

    // Composite relationships
    public static Mass operator *(MolarMass molarMass, Quantity quantity) => Mass.FromKilograms(molarMass.ToKilogramsPerMole() * quantity.ToMoles());
    public static Mass operator *(Quantity quantity, MolarMass molarMass) => Mass.FromKilograms(quantity.ToMoles() * molarMass.ToKilogramsPerMole());

}
