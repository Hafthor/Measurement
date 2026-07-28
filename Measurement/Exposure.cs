namespace com.hafthor.Measurement;

[Measurement("C/g", VariableName = "coulombsPerGram")]
public readonly partial struct Exposure {
    // Units
    public static Exposure FromCoulombsPerKilogram(double coulombsPerKilogram) => new(coulombsPerKilogram * 1e3);
    public double ToCoulombsPerKilogram() => coulombsPerGram / 1e3;
    public static Exposure FromCoulombsPerGram(double coulombsPerGram) => new(coulombsPerGram);
    public double ToCoulombsPerGram() => coulombsPerGram;
    public static Exposure FromRoentgens(double roentgens) => new(roentgens * (2.58e-1));
    public double ToRoentgens() => coulombsPerGram / (2.58e-1);

    // Composite relationships
    public static ElectricCharge operator *(Exposure exposure, Mass mass) => ElectricCharge.FromCoulombs(exposure.ToCoulombsPerKilogram() * mass.ToKilograms());
    public static ElectricCharge operator *(Mass mass, Exposure exposure) => ElectricCharge.FromCoulombs(mass.ToKilograms() * exposure.ToCoulombsPerKilogram());
}
