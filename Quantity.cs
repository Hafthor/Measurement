namespace com.hafthor.Measurement;

public class Quantity {
    private const double Avogadro = 6.02214076e23;

    private readonly double moles;

    private Quantity(double moles) => this.moles = moles;

    // Arithmetic
    public static Quantity operator +(Quantity a, Quantity b) => new(a.moles + b.moles);
    public static Quantity operator -(Quantity a, Quantity b) => new(a.moles - b.moles);
    public static Quantity operator -(Quantity x) => new(-x.moles);

    // SI units
    public static Quantity FromKilomoles(double kilomoles) => new(kilomoles * 1e3);
    public double ToKilomoles() => moles / 1e3;
    public static Quantity FromMoles(double moles) => new(moles);
    public double ToMoles() => moles;
    public static Quantity FromMillimoles(double millimoles) => new(millimoles * 1e-3);
    public double ToMillimoles() => moles / 1e-3;
    public static Quantity FromMicromoles(double micromoles) => new(micromoles * 1e-6);
    public double ToMicromoles() => moles / 1e-6;
    public static Quantity FromNanomoles(double nanomoles) => new(nanomoles * 1e-9);
    public double ToNanomoles() => moles / 1e-9;

    // Raw counts (related to moles via Avogadro's number)
    public static Quantity FromCount(double count) => new(count / Avogadro);
    public double ToCount() => moles * Avogadro;
    public static Quantity FromPairs(double pairs) => new(pairs * 2 / Avogadro);
    public double ToPairs() => moles * Avogadro / 2;
    public static Quantity FromDozens(double dozens) => new(dozens * 12 / Avogadro);
    public double ToDozens() => moles * Avogadro / 12;
    public static Quantity FromScores(double scores) => new(scores * 20 / Avogadro);
    public double ToScores() => moles * Avogadro / 20;
    public static Quantity FromGross(double gross) => new(gross * 144 / Avogadro);
    public double ToGross() => moles * Avogadro / 144;

    // Composite relationships (derived)
    public static Molality operator /(Quantity quantity, Mass mass) => Molality.FromMolesPerKilogram(quantity.ToMoles() / mass.ToKilograms());
    public static Concentration operator /(Quantity quantity, Volume volume) => Concentration.FromMolesPerCubicMeter(quantity.ToMoles() / volume.ToCubicMeters());
}
