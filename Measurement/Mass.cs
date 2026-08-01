namespace com.hafthor.Measurement;

// The canonical (stored) unit is the microgram (VariableName), so microgram/milligram/gram-scale
// values land on exact integers in IEEE-754; ToString presents grams (DisplayFactor = 1e6). Unit
// From/To methods are generated from the [SiUnit]/[Unit] declarations; factors are micrograms per
// one of the unit.
[Measurement("g", VariableName = "micrograms", DisplayFactor = 1e6)]
[SiUnit("Grams", 6, "Kilo None Milli Micro Nano")]
// Imperial / US units
[Unit("Tonnes", 1e12)]
[Unit("LongTons", 1016046.9088e6)]
[Unit("ShortTons", 907184.74e6)]
[Unit("Stones", 6350.29318e6)]
[Unit("Pounds", 453.59237e6)]
[Unit("Ounces", 28.349523125e6)]
[Unit("Drams", 1.7718451953125e6)]
[Unit("Grains", 0.06479891e6)]
[Unit("Slugs", 14593.9029372e6)]
// Troy & jewellers' units
[Unit("TroyPounds", 373.2417216e6)]
[Unit("TroyOunces", 31.1034768e6)]
[Unit("Pennyweights", 1.55517384e6)]
[Unit("Carats", 0.2e6)]
// Atomic units
[Unit("Daltons", 1.66053906660e-18)]
[Unit("ProtonMasses", 1.67262192369e-18)]
[Unit("ElectronMasses", 9.1093837015e-22)]
[Unit("PlanckMasses", 2.176434e1)]
// Astronomical units
[Unit("SolarMasses", 1.98892e39)]
[Unit("JupiterMasses", 1.898e36)]
[Unit("EarthMasses", 5.9722e33)]
[Unit("LunarMasses", 7.342e31)]
public readonly partial struct Mass {
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
