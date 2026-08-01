namespace com.hafthor.Measurement;

[Measurement("", VariableName = "quantity")]
[Unit("Kilomoles", 1e3 * Avogadro)]
[Unit("Moles", Avogadro)]
[Unit("Millimoles", 1e-3 * Avogadro)]
[Unit("Micromoles", 1e-6 * Avogadro)]
[Unit("Nanomoles", 1e-9 * Avogadro)]
[SiUnit("Count", 0)]
[Unit("Pairs", 2)]
[Unit("Dozens", 12)]
[Unit("Scores", 20)]
[Unit("Gross", 144)]
public readonly partial struct Quantity {
    private const double Avogadro = 6.02214076e23;

    public static Molality operator /(Quantity quantity, Mass mass) => Molality.FromMolesPerKilogram(quantity.ToMoles() / mass.ToKilograms());
    public static Concentration operator /(Quantity quantity, Volume volume) => Concentration.FromMolesPerCubicMeter(quantity.ToMoles() / volume.ToCubicMeters());
}
