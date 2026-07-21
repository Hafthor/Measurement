namespace com.hafthor.Measurement;

public class SpecificVolume {
    private readonly double cubicMetersPerKilogram;

    private SpecificVolume(double cubicMetersPerKilogram) => this.cubicMetersPerKilogram = cubicMetersPerKilogram;

    // Arithmetic
    public static SpecificVolume operator +(SpecificVolume a, SpecificVolume b) => new SpecificVolume(a.cubicMetersPerKilogram + b.cubicMetersPerKilogram);
    public static SpecificVolume operator -(SpecificVolume a, SpecificVolume b) => new SpecificVolume(a.cubicMetersPerKilogram - b.cubicMetersPerKilogram);
    public static SpecificVolume operator -(SpecificVolume x) => new SpecificVolume(-x.cubicMetersPerKilogram);

    // Units
    public static SpecificVolume FromCubicMetersPerKilogram(double cubicMetersPerKilogram) => new SpecificVolume(cubicMetersPerKilogram);
    public double ToCubicMetersPerKilogram() => cubicMetersPerKilogram;
    public static SpecificVolume FromLitersPerKilogram(double litersPerKilogram) => new SpecificVolume(litersPerKilogram * (1e-3));
    public double ToLitersPerKilogram() => cubicMetersPerKilogram / (1e-3);
    public static SpecificVolume FromCubicCentimetersPerGram(double cubicCentimetersPerGram) => new SpecificVolume(cubicCentimetersPerGram * (1e-3));
    public double ToCubicCentimetersPerGram() => cubicMetersPerKilogram / (1e-3);

    // Composite relationships
    public static Volume operator *(SpecificVolume specificVolume, Mass mass) => Volume.FromCubicMeters(specificVolume.ToCubicMetersPerKilogram() * mass.ToKilograms());
    public static Volume operator *(Mass mass, SpecificVolume specificVolume) => Volume.FromCubicMeters(mass.ToKilograms() * specificVolume.ToCubicMetersPerKilogram());
}
