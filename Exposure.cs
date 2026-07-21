namespace com.hafthor.Measurement;

public class Exposure {
    private readonly double coulombsPerKilogram;

    private Exposure(double coulombsPerKilogram) => this.coulombsPerKilogram = coulombsPerKilogram;

    // Arithmetic
    public static Exposure operator +(Exposure a, Exposure b) => new(a.coulombsPerKilogram + b.coulombsPerKilogram);
    public static Exposure operator -(Exposure a, Exposure b) => new(a.coulombsPerKilogram - b.coulombsPerKilogram);
    public static Exposure operator -(Exposure x) => new(-x.coulombsPerKilogram);

    // Units
    public static Exposure FromCoulombsPerKilogram(double coulombsPerKilogram) => new(coulombsPerKilogram);
    public double ToCoulombsPerKilogram() => coulombsPerKilogram;
    public static Exposure FromRoentgens(double roentgens) => new(roentgens * (2.58e-4));
    public double ToRoentgens() => coulombsPerKilogram / (2.58e-4);

    // Composite relationships
    public static ElectricCharge operator *(Exposure exposure, Mass mass) => ElectricCharge.FromCoulombs(exposure.ToCoulombsPerKilogram() * mass.ToKilograms());
    public static ElectricCharge operator *(Mass mass, Exposure exposure) => ElectricCharge.FromCoulombs(mass.ToKilograms() * exposure.ToCoulombsPerKilogram());

    public override string ToString() => $"{coulombsPerKilogram} C/kg";
}
