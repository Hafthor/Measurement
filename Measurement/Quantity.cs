namespace com.hafthor.Measurement;

[Measurement("", VariableName = "quantity")]
public readonly partial struct Quantity {
    private const double Avogadro = 6.02214076e23;

    // Canonical (stored) unit is the raw count of entities, so integer counts — and whole
    // dozens, gross, etc. — are exact in IEEE-754 up to 2^53, and ToString presents the bare
    // number. Moles are carried as count / Avogadro and are therefore the approximate side
    // (count and moles cannot both be exact; the count is preferred).
    public static Quantity FromKilomoles(double kilomoles) => new(kilomoles * 1e3 * Avogadro);
    public double ToKilomoles() => quantity / Avogadro / 1e3;
    public static Quantity FromMoles(double moles) => new(moles * Avogadro);
    public double ToMoles() => quantity / Avogadro;
    public static Quantity FromMillimoles(double millimoles) => new(millimoles * 1e-3 * Avogadro);
    public double ToMillimoles() => quantity / Avogadro * 1e3;
    public static Quantity FromMicromoles(double micromoles) => new(micromoles * 1e-6 * Avogadro);
    public double ToMicromoles() => quantity / Avogadro * 1e6;
    public static Quantity FromNanomoles(double nanomoles) => new(nanomoles * 1e-9 * Avogadro);
    public double ToNanomoles() => quantity / Avogadro * 1e9;

    // Raw counts (canonical)
    public static Quantity FromCount(double count) => new(count);
    public double ToCount() => quantity;
    public static Quantity FromPairs(double pairs) => new(pairs * 2);
    public double ToPairs() => quantity / 2;
    public static Quantity FromDozens(double dozens) => new(dozens * 12);
    public double ToDozens() => quantity / 12;
    public static Quantity FromScores(double scores) => new(scores * 20);
    public double ToScores() => quantity / 20;
    public static Quantity FromGross(double gross) => new(gross * 144);
    public double ToGross() => quantity / 144;

    // Composite relationships (derived)
    public static Molality operator /(Quantity quantity, Mass mass) => Molality.FromMolesPerKilogram(quantity.ToMoles() / mass.ToKilograms());
    public static Concentration operator /(Quantity quantity, Volume volume) => Concentration.FromMolesPerCubicMeter(quantity.ToMoles() / volume.ToCubicMeters());
}
