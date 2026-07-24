namespace com.hafthor.Measurement;

[Measurement("g", DisplayFactor = 1e6)]
public readonly partial struct Mass {

    // The canonical (stored) unit is the microgram, so microgram/milligram/gram-scale values
    // land on exact integers in IEEE-754; ToString presents grams (DisplayFactor = 1e6).
    public static Mass FromTonnes(double tonnes) => new(tonnes * 1e12);
    public double ToTonnes() => value / 1e12;
    public static Mass FromKilograms(double kilograms) => new(kilograms * 1e9);
    public double ToKilograms() => value / 1e9;
    public static Mass FromGrams(double grams) => new(grams * 1e6);
    public double ToGrams() => value / 1e6;
    public static Mass FromMilligrams(double milligrams) => new(milligrams * 1e3);
    public double ToMilligrams() => value / 1e3;
    public static Mass FromMicrograms(double micrograms) => new(micrograms);
    public double ToMicrograms() => value;
    public static Mass FromNanograms(double nanograms) => new(nanograms * 1e-3);
    public double ToNanograms() => value / 1e-3;

    // Imperial / US units
    public static Mass FromLongTons(double longTons) => new(longTons * 1016046.9088e6);
    public double ToLongTons() => value / 1016046.9088e6;
    public static Mass FromShortTons(double shortTons) => new(shortTons * 907184.74e6);
    public double ToShortTons() => value / 907184.74e6;
    public static Mass FromStones(double stones) => new(stones * 6350.29318e6);
    public double ToStones() => value / 6350.29318e6;
    public static Mass FromPounds(double pounds) => new(pounds * 453.59237e6);
    public double ToPounds() => value / 453.59237e6;
    public static Mass FromOunces(double ounces) => new(ounces * 28.349523125e6);
    public double ToOunces() => value / 28.349523125e6;
    public static Mass FromDrams(double drams) => new(drams * 1.7718451953125e6);
    public double ToDrams() => value / 1.7718451953125e6;
    public static Mass FromGrains(double grains) => new(grains * 0.06479891e6);
    public double ToGrains() => value / 0.06479891e6;
    public static Mass FromSlugs(double slugs) => new(slugs * 14593.9029372e6);
    public double ToSlugs() => value / 14593.9029372e6;

    // Troy & jewellers' units
    public static Mass FromTroyPounds(double troyPounds) => new(troyPounds * 373.2417216e6);
    public double ToTroyPounds() => value / 373.2417216e6;
    public static Mass FromTroyOunces(double troyOunces) => new(troyOunces * 31.1034768e6);
    public double ToTroyOunces() => value / 31.1034768e6;
    public static Mass FromPennyweights(double pennyweights) => new(pennyweights * 1.55517384e6);
    public double ToPennyweights() => value / 1.55517384e6;
    public static Mass FromCarats(double carats) => new(carats * 0.2e6);
    public double ToCarats() => value / 0.2e6;

    // Atomic units
    public static Mass FromDaltons(double daltons) => new(daltons * 1.66053906660e-18);
    public double ToDaltons() => value / 1.66053906660e-18;
    public static Mass FromProtonMasses(double protonMasses) => new(protonMasses * 1.67262192369e-18);
    public double ToProtonMasses() => value / 1.67262192369e-18;
    public static Mass FromElectronMasses(double electronMasses) => new(electronMasses * 9.1093837015e-22);
    public double ToElectronMasses() => value / 9.1093837015e-22;
    public static Mass FromPlanckMasses(double planckMasses) => new(planckMasses * 2.176434e1);
    public double ToPlanckMasses() => value / 2.176434e1;

    // Astronomical units
    public static Mass FromSolarMasses(double solarMasses) => new(solarMasses * 1.98892e39);
    public double ToSolarMasses() => value / 1.98892e39;
    public static Mass FromJupiterMasses(double jupiterMasses) => new(jupiterMasses * 1.898e36);
    public double ToJupiterMasses() => value / 1.898e36;
    public static Mass FromEarthMasses(double earthMasses) => new(earthMasses * 5.9722e33);
    public double ToEarthMasses() => value / 5.9722e33;
    public static Mass FromLunarMasses(double lunarMasses) => new(lunarMasses * 7.342e31);
    public double ToLunarMasses() => value / 7.342e31;

    // Composite relationships
    public static Force operator *(Mass mass, Acceleration acceleration) => Force.FromNewtons(mass.ToKilograms() * acceleration.ToMetersPerSecondSquared());

    // Composite relationships (derived)
    public static Density operator /(Mass mass, Volume volume) => Density.FromKilogramsPerCubicMeter(mass.ToKilograms() / volume.ToCubicMeters());
    public static LinearDensity operator /(Mass mass, Length length) => LinearDensity.FromKilogramsPerMeter(mass.ToKilograms() / length.ToMeters());
    public static AreaDensity operator /(Mass mass, Area area) => AreaDensity.FromKilogramsPerSquareMeter(mass.ToKilograms() / area.ToSquareMeters());
    public static MassFlowRate operator /(Mass mass, Duration duration) => MassFlowRate.FromKilogramsPerSecond(mass.ToKilograms() / duration.ToSeconds());
    public static MolarMass operator /(Mass mass, Quantity quantity) => MolarMass.FromKilogramsPerMole(mass.ToKilograms() / quantity.ToMoles());
    public static Momentum operator *(Mass mass, Speed speed) => Momentum.FromKilogramMetersPerSecond(mass.ToKilograms() * speed.ToMetersPerSecond());
    public static MomentOfInertia operator *(Mass mass, Area area) => MomentOfInertia.FromKilogramSquareMeters(mass.ToKilograms() * area.ToSquareMeters());

}
