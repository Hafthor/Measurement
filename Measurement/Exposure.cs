namespace com.hafthor.Measurement;

[Measurement("C/g", VariableName = "coulombsPerGram")]
[SiUnit("CoulombsPerKilogram", -3)]
[SiUnit("CoulombsPerGram", 0)]
[Unit("Roentgens", 2.58e-7)]
public readonly partial struct Exposure {
    // Composite relationships
    public static ElectricCharge operator *(Exposure exposure, Mass mass) => ElectricCharge.FromCoulombs(exposure.ToCoulombsPerKilogram() * mass.ToKilograms());
    public static ElectricCharge operator *(Mass mass, Exposure exposure) => ElectricCharge.FromCoulombs(mass.ToKilograms() * exposure.ToCoulombsPerKilogram());
}
