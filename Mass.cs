namespace com.hafthor.Measurement;

public class Mass {
    private readonly double kilograms;

    private Mass(double kilograms) => this.kilograms = kilograms;

    // Arithmetic
    public static Mass operator +(Mass a, Mass b) => new Mass(a.kilograms + b.kilograms);
    public static Mass operator -(Mass a, Mass b) => new Mass(a.kilograms - b.kilograms);
    public static Mass operator -(Mass x) => new Mass(-x.kilograms);

    // SI units
    public static Mass FromTonnes(double tonnes) => new Mass(tonnes * 1e3);
    public double ToTonnes() => kilograms / 1e3;
    public static Mass FromKilograms(double kilograms) => new Mass(kilograms);
    public double ToKilograms() => kilograms;
    public static Mass FromGrams(double grams) => new Mass(grams * 1e-3);
    public double ToGrams() => kilograms / 1e-3;
    public static Mass FromMilligrams(double milligrams) => new Mass(milligrams * 1e-6);
    public double ToMilligrams() => kilograms / 1e-6;
    public static Mass FromMicrograms(double micrograms) => new Mass(micrograms * 1e-9);
    public double ToMicrograms() => kilograms / 1e-9;
    public static Mass FromNanograms(double nanograms) => new Mass(nanograms * 1e-12);
    public double ToNanograms() => kilograms / 1e-12;

    // Imperial / US units
    public static Mass FromLongTons(double longTons) => new Mass(longTons * 1016.0469088);
    public double ToLongTons() => kilograms / 1016.0469088;
    public static Mass FromShortTons(double shortTons) => new Mass(shortTons * 907.18474);
    public double ToShortTons() => kilograms / 907.18474;
    public static Mass FromStones(double stones) => new Mass(stones * 6.35029318);
    public double ToStones() => kilograms / 6.35029318;
    public static Mass FromPounds(double pounds) => new Mass(pounds * 0.45359237);
    public double ToPounds() => kilograms / 0.45359237;
    public static Mass FromOunces(double ounces) => new Mass(ounces * 0.028349523125);
    public double ToOunces() => kilograms / 0.028349523125;
    public static Mass FromDrams(double drams) => new Mass(drams * 0.0017718451953125);
    public double ToDrams() => kilograms / 0.0017718451953125;
    public static Mass FromGrains(double grains) => new Mass(grains * 6.479891e-5);
    public double ToGrains() => kilograms / 6.479891e-5;
    public static Mass FromSlugs(double slugs) => new Mass(slugs * 14.5939029372);
    public double ToSlugs() => kilograms / 14.5939029372;

    // Troy & jewellers' units
    public static Mass FromTroyPounds(double troyPounds) => new Mass(troyPounds * 0.3732417216);
    public double ToTroyPounds() => kilograms / 0.3732417216;
    public static Mass FromTroyOunces(double troyOunces) => new Mass(troyOunces * 0.0311034768);
    public double ToTroyOunces() => kilograms / 0.0311034768;
    public static Mass FromPennyweights(double pennyweights) => new Mass(pennyweights * 0.00155517384);
    public double ToPennyweights() => kilograms / 0.00155517384;
    public static Mass FromCarats(double carats) => new Mass(carats * 0.0002);
    public double ToCarats() => kilograms / 0.0002;

    // Atomic units
    public static Mass FromDaltons(double daltons) => new Mass(daltons * 1.66053906660e-27);
    public double ToDaltons() => kilograms / 1.66053906660e-27;
    public static Mass FromProtonMasses(double protonMasses) => new Mass(protonMasses * 1.67262192369e-27);
    public double ToProtonMasses() => kilograms / 1.67262192369e-27;
    public static Mass FromElectronMasses(double electronMasses) => new Mass(electronMasses * 9.1093837015e-31);
    public double ToElectronMasses() => kilograms / 9.1093837015e-31;
    public static Mass FromPlanckMasses(double planckMasses) => new Mass(planckMasses * 2.176434e-8);
    public double ToPlanckMasses() => kilograms / 2.176434e-8;

    // Astronomical units
    public static Mass FromSolarMasses(double solarMasses) => new Mass(solarMasses * 1.98892e30);
    public double ToSolarMasses() => kilograms / 1.98892e30;
    public static Mass FromJupiterMasses(double jupiterMasses) => new Mass(jupiterMasses * 1.898e27);
    public double ToJupiterMasses() => kilograms / 1.898e27;
    public static Mass FromEarthMasses(double earthMasses) => new Mass(earthMasses * 5.9722e24);
    public double ToEarthMasses() => kilograms / 5.9722e24;
    public static Mass FromLunarMasses(double lunarMasses) => new Mass(lunarMasses * 7.342e22);
    public double ToLunarMasses() => kilograms / 7.342e22;

    // Composite relationships
    public static Force operator *(Mass mass, Acceleration acceleration) => Force.FromNewtons(mass.kilograms * acceleration.ToMetersPerSecondSquared());

    // Composite relationships (derived)
    public static Density operator /(Mass mass, Volume volume) => Density.FromKilogramsPerCubicMeter(mass.ToKilograms() / volume.ToCubicMeters());
    public static LinearDensity operator /(Mass mass, Length length) => LinearDensity.FromKilogramsPerMeter(mass.ToKilograms() / length.ToMeters());
    public static AreaDensity operator /(Mass mass, Area area) => AreaDensity.FromKilogramsPerSquareMeter(mass.ToKilograms() / area.ToSquareMeters());
    public static MassFlowRate operator /(Mass mass, Duration duration) => MassFlowRate.FromKilogramsPerSecond(mass.ToKilograms() / duration.ToSeconds());
    public static MolarMass operator /(Mass mass, Quantity quantity) => MolarMass.FromKilogramsPerMole(mass.ToKilograms() / quantity.ToMoles());
    public static Momentum operator *(Mass mass, Speed speed) => Momentum.FromKilogramMetersPerSecond(mass.ToKilograms() * speed.ToMetersPerSecond());
    public static MomentOfInertia operator *(Mass mass, Area area) => MomentOfInertia.FromKilogramSquareMeters(mass.ToKilograms() * area.ToSquareMeters());
}
