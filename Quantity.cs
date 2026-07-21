namespace com.hafthor.Measurement;

[Measurement("mol")]
public readonly partial struct Quantity {
    private const double Avogadro = 6.02214076e23;

    // SI units
    public static Quantity FromKilomoles(double kilomoles) => new(kilomoles * 1e3);
    public double ToKilomoles() => value / 1e3;
    public static Quantity FromMoles(double moles) => new(moles);
    public double ToMoles() => value;
    public static Quantity FromMillimoles(double millimoles) => new(millimoles * 1e-3);
    public double ToMillimoles() => value / 1e-3;
    public static Quantity FromMicromoles(double micromoles) => new(micromoles * 1e-6);
    public double ToMicromoles() => value / 1e-6;
    public static Quantity FromNanomoles(double nanomoles) => new(nanomoles * 1e-9);
    public double ToNanomoles() => value / 1e-9;

    // Raw counts (related to moles via Avogadro's number)
    public static Quantity FromCount(double count) => new(count / Avogadro);
    public double ToCount() => value * Avogadro;
    public static Quantity FromPairs(double pairs) => new(pairs * 2 / Avogadro);
    public double ToPairs() => value * Avogadro / 2;
    public static Quantity FromDozens(double dozens) => new(dozens * 12 / Avogadro);
    public double ToDozens() => value * Avogadro / 12;
    public static Quantity FromScores(double scores) => new(scores * 20 / Avogadro);
    public double ToScores() => value * Avogadro / 20;
    public static Quantity FromGross(double gross) => new(gross * 144 / Avogadro);
    public double ToGross() => value * Avogadro / 144;

    // Composite relationships (derived)
    public static Molality operator /(Quantity quantity, Mass mass) => Molality.FromMolesPerKilogram(quantity.ToMoles() / mass.ToKilograms());
    public static Concentration operator /(Quantity quantity, Volume volume) => Concentration.FromMolesPerCubicMeter(quantity.ToMoles() / volume.ToCubicMeters());

}
