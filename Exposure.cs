namespace com.hafthor.Measurement;

[Measurement("C/kg")]
public readonly partial struct Exposure {

    // Units
    public static Exposure FromCoulombsPerKilogram(double coulombsPerKilogram) => new(coulombsPerKilogram);
    public double ToCoulombsPerKilogram() => value;
    public static Exposure FromRoentgens(double roentgens) => new(roentgens * (2.58e-4));
    public double ToRoentgens() => value / (2.58e-4);

    // Composite relationships
    public static ElectricCharge operator *(Exposure exposure, Mass mass) => ElectricCharge.FromCoulombs(exposure.ToCoulombsPerKilogram() * mass.ToKilograms());
    public static ElectricCharge operator *(Mass mass, Exposure exposure) => ElectricCharge.FromCoulombs(mass.ToKilograms() * exposure.ToCoulombsPerKilogram());

}
