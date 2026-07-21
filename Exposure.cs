namespace com.hafthor.Measurement;

public sealed class Exposure : Measurement<Exposure> {

    private Exposure(double value) : base(value) { }

    protected override Exposure Create(double value) => new(value);
    protected override string Symbol => "C/kg";

    // Units
    public static Exposure FromCoulombsPerKilogram(double value) => new(value);
    public double ToCoulombsPerKilogram() => value;
    public static Exposure FromRoentgens(double roentgens) => new(roentgens * (2.58e-4));
    public double ToRoentgens() => value / (2.58e-4);

    // Composite relationships
    public static ElectricCharge operator *(Exposure exposure, Mass mass) => ElectricCharge.FromCoulombs(exposure.ToCoulombsPerKilogram() * mass.ToKilograms());
    public static ElectricCharge operator *(Mass mass, Exposure exposure) => ElectricCharge.FromCoulombs(mass.ToKilograms() * exposure.ToCoulombsPerKilogram());

}
