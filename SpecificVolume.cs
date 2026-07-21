namespace com.hafthor.Measurement;

public sealed class SpecificVolume : Measurement<SpecificVolume> {

    private SpecificVolume(double value) : base(value) { }

    protected override SpecificVolume Create(double value) => new(value);
    protected override string Symbol => "m³/kg";

    // Units
    public static SpecificVolume FromCubicMetersPerKilogram(double value) => new(value);
    public double ToCubicMetersPerKilogram() => value;
    public static SpecificVolume FromLitersPerKilogram(double litersPerKilogram) => new(litersPerKilogram * (1e-3));
    public double ToLitersPerKilogram() => value / (1e-3);
    public static SpecificVolume FromCubicCentimetersPerGram(double cubicCentimetersPerGram) => new(cubicCentimetersPerGram * (1e-3));
    public double ToCubicCentimetersPerGram() => value / (1e-3);

    // Composite relationships
    public static Volume operator *(SpecificVolume specificVolume, Mass mass) => Volume.FromCubicMeters(specificVolume.ToCubicMetersPerKilogram() * mass.ToKilograms());
    public static Volume operator *(Mass mass, SpecificVolume specificVolume) => Volume.FromCubicMeters(mass.ToKilograms() * specificVolume.ToCubicMetersPerKilogram());

}
