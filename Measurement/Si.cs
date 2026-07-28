namespace com.hafthor.Measurement;

// Carries a scalar already scaled into a unit; prefixes stack, e.g. Measure.Of(1).Mega.Mega.Meters.
public readonly struct Prefixed {
    public readonly double Value;
    internal Prefixed(double value) => Value = value;

    public Prefixed Quetta => new(Value * 1e30);
    public Prefixed Ronna => new(Value * 1e27);
    public Prefixed Yotta => new(Value * 1e24);
    public Prefixed Zetta => new(Value * 1e21);
    public Prefixed Exa => new(Value * 1e18);
    public Prefixed Peta => new(Value * 1e15);
    public Prefixed Tera => new(Value * 1e12);
    public Prefixed Giga => new(Value * 1e9);
    public Prefixed Mega => new(Value * 1e6);
    public Prefixed Kilo => new(Value * 1e3);
    public Prefixed Hecto => new(Value * 1e2);
    public Prefixed Deca => new(Value * 1e1);
    public Prefixed Deci => new(Value * 1e-1);
    public Prefixed Centi => new(Value * 1e-2);
    public Prefixed Milli => new(Value * 1e-3);
    public Prefixed Micro => new(Value * 1e-6);
    public Prefixed Nano => new(Value * 1e-9);
    public Prefixed Pico => new(Value * 1e-12);
    public Prefixed Femto => new(Value * 1e-15);
    public Prefixed Atto => new(Value * 1e-18);
    public Prefixed Zepto => new(Value * 1e-21);
    public Prefixed Yocto => new(Value * 1e-24);
    public Prefixed Ronto => new(Value * 1e-27);
    public Prefixed Quecto => new(Value * 1e-30);
}

// Read-out builder: a measurement plus a running prefix factor, e.g. mass.To.Milli.Grams.
public readonly struct Reader<T> {
    public readonly T Value;
    public readonly double Factor;
    internal Reader(T value, double factor) { Value = value; Factor = factor; }

    public Reader<T> Quetta => new(Value, Factor * 1e30);
    public Reader<T> Ronna => new(Value, Factor * 1e27);
    public Reader<T> Yotta => new(Value, Factor * 1e24);
    public Reader<T> Zetta => new(Value, Factor * 1e21);
    public Reader<T> Exa => new(Value, Factor * 1e18);
    public Reader<T> Peta => new(Value, Factor * 1e15);
    public Reader<T> Tera => new(Value, Factor * 1e12);
    public Reader<T> Giga => new(Value, Factor * 1e9);
    public Reader<T> Mega => new(Value, Factor * 1e6);
    public Reader<T> Kilo => new(Value, Factor * 1e3);
    public Reader<T> Hecto => new(Value, Factor * 1e2);
    public Reader<T> Deca => new(Value, Factor * 1e1);
    public Reader<T> Deci => new(Value, Factor * 1e-1);
    public Reader<T> Centi => new(Value, Factor * 1e-2);
    public Reader<T> Milli => new(Value, Factor * 1e-3);
    public Reader<T> Micro => new(Value, Factor * 1e-6);
    public Reader<T> Nano => new(Value, Factor * 1e-9);
    public Reader<T> Pico => new(Value, Factor * 1e-12);
    public Reader<T> Femto => new(Value, Factor * 1e-15);
    public Reader<T> Atto => new(Value, Factor * 1e-18);
    public Reader<T> Zepto => new(Value, Factor * 1e-21);
    public Reader<T> Yocto => new(Value, Factor * 1e-24);
    public Reader<T> Ronto => new(Value, Factor * 1e-27);
    public Reader<T> Quecto => new(Value, Factor * 1e-30);
}

// Non-double entry point — always available (never extends double): Measure.Of(5).Kilo.Grams.
public static class Measure {
    public static Prefixed Of(double value) => new(value);
}

// Measurement-side fluent members. Only the base SI unit and non-SI units get a direct hook;
// SI prefixes are expressed through the prefix chain (e.g. Measure.Of(1).Kilo.Grams).
public static class Units {
    extension(Prefixed p) {
        public Length Meters => Length.FromMeters(p.Value);
        public Length Angstroms => Length.FromAngstroms(p.Value);
        public Length Leagues => Length.FromLeagues(p.Value);
        public Length NauticalMiles => Length.FromNauticalMiles(p.Value);
        public Length Miles => Length.FromMiles(p.Value);
        public Length Furlongs => Length.FromFurlongs(p.Value);
        public Length Chains => Length.FromChains(p.Value);
        public Length Rods => Length.FromRods(p.Value);
        public Length Fathoms => Length.FromFathoms(p.Value);
        public Length Yards => Length.FromYards(p.Value);
        public Length Feet => Length.FromFeet(p.Value);
        public Length Inches => Length.FromInches(p.Value);
        public Length HubbleLengths => Length.FromHubbleLengths(p.Value);
        public Length Parsecs => Length.FromParsecs(p.Value);
        public Length LightYears => Length.FromLightYears(p.Value);
        public Length AstronomicalUnits => Length.FromAstronomicalUnits(p.Value);
        public Length PlanckLengths => Length.FromPlanckLengths(p.Value);
        public Mass Tonnes => Mass.FromTonnes(p.Value);
        public Mass Grams => Mass.FromGrams(p.Value);
        public Mass LongTons => Mass.FromLongTons(p.Value);
        public Mass ShortTons => Mass.FromShortTons(p.Value);
        public Mass Stones => Mass.FromStones(p.Value);
        public Mass Pounds => Mass.FromPounds(p.Value);
        public Mass Ounces => Mass.FromOunces(p.Value);
        public Mass Drams => Mass.FromDrams(p.Value);
        public Mass Grains => Mass.FromGrains(p.Value);
        public Mass Slugs => Mass.FromSlugs(p.Value);
        public Mass TroyPounds => Mass.FromTroyPounds(p.Value);
        public Mass TroyOunces => Mass.FromTroyOunces(p.Value);
        public Mass Pennyweights => Mass.FromPennyweights(p.Value);
        public Mass Carats => Mass.FromCarats(p.Value);
        public Mass Daltons => Mass.FromDaltons(p.Value);
        public Mass ProtonMasses => Mass.FromProtonMasses(p.Value);
        public Mass ElectronMasses => Mass.FromElectronMasses(p.Value);
        public Mass PlanckMasses => Mass.FromPlanckMasses(p.Value);
        public Mass SolarMasses => Mass.FromSolarMasses(p.Value);
        public Mass JupiterMasses => Mass.FromJupiterMasses(p.Value);
        public Mass EarthMasses => Mass.FromEarthMasses(p.Value);
        public Mass LunarMasses => Mass.FromLunarMasses(p.Value);
        public Duration Seconds => Duration.FromSeconds(p.Value);
        public Duration Minutes => Duration.FromMinutes(p.Value);
        public Duration Hours => Duration.FromHours(p.Value);
        public Duration Days => Duration.FromDays(p.Value);
        public Duration Weeks => Duration.FromWeeks(p.Value);
        public Duration Fortnights => Duration.FromFortnights(p.Value);
        public Duration CommonYears => Duration.FromCommonYears(p.Value);
        public Duration JulianYears => Duration.FromJulianYears(p.Value);
        public Duration TropicalYears => Duration.FromTropicalYears(p.Value);
        public Duration SiderealYears => Duration.FromSiderealYears(p.Value);
        public Duration SiderealDays => Duration.FromSiderealDays(p.Value);
        public Duration Decades => Duration.FromDecades(p.Value);
        public Duration Centuries => Duration.FromCenturies(p.Value);
        public Duration Millennia => Duration.FromMillennia(p.Value);
        public Duration Annums => Duration.FromAnnums(p.Value);
        public Duration HubbleTimes => Duration.FromHubbleTimes(p.Value);
        public Duration PlanckTimes => Duration.FromPlanckTimes(p.Value);
        public ElectricCurrent Amperes => ElectricCurrent.FromAmperes(p.Value);
        public ElectricCurrent Abamperes => ElectricCurrent.FromAbamperes(p.Value);
        public ElectricCurrent Statamperes => ElectricCurrent.FromStatamperes(p.Value);
        public Temperature Kelvin => Temperature.FromKelvin(p.Value);
        public Temperature Rankine => Temperature.FromRankine(p.Value);
        public Temperature Celsius => Temperature.FromCelsius(p.Value);
        public Temperature Fahrenheit => Temperature.FromFahrenheit(p.Value);
        public Temperature Reaumur => Temperature.FromReaumur(p.Value);
        public Temperature Delisle => Temperature.FromDelisle(p.Value);
        public Temperature Newton => Temperature.FromNewton(p.Value);
        public Temperature Romer => Temperature.FromRomer(p.Value);
        public Quantity Moles => Quantity.FromMoles(p.Value);
        public Quantity Count => Quantity.FromCount(p.Value);
        public Quantity Pairs => Quantity.FromPairs(p.Value);
        public Quantity Dozens => Quantity.FromDozens(p.Value);
        public Quantity Scores => Quantity.FromScores(p.Value);
        public Quantity Gross => Quantity.FromGross(p.Value);
        public LuminousIntensity Candelas => LuminousIntensity.FromCandelas(p.Value);
        public LuminousIntensity Candlepower => LuminousIntensity.FromCandlepower(p.Value);
        public LuminousIntensity Hefnerkerze => LuminousIntensity.FromHefnerkerze(p.Value);
        public LuminousIntensity Carcels => LuminousIntensity.FromCarcels(p.Value);
        public Area SquareKilometers => Area.FromSquareKilometers(p.Value);
        public Area Hectares => Area.FromHectares(p.Value);
        public Area Ares => Area.FromAres(p.Value);
        public Area SquareMeters => Area.FromSquareMeters(p.Value);
        public Area SquareCentimeters => Area.FromSquareCentimeters(p.Value);
        public Area SquareMillimeters => Area.FromSquareMillimeters(p.Value);
        public Area SquareMiles => Area.FromSquareMiles(p.Value);
        public Area Acres => Area.FromAcres(p.Value);
        public Area SquareYards => Area.FromSquareYards(p.Value);
        public Area SquareFeet => Area.FromSquareFeet(p.Value);
        public Area SquareInches => Area.FromSquareInches(p.Value);
        public Area Barns => Area.FromBarns(p.Value);
        public Volume CubicMeters => Volume.FromCubicMeters(p.Value);
        public Volume CubicCentimeters => Volume.FromCubicCentimeters(p.Value);
        public Volume CubicMillimeters => Volume.FromCubicMillimeters(p.Value);
        public Volume Liters => Volume.FromLiters(p.Value);
        public Volume Gallons => Volume.FromGallons(p.Value);
        public Volume Quarts => Volume.FromQuarts(p.Value);
        public Volume Pints => Volume.FromPints(p.Value);
        public Volume Cups => Volume.FromCups(p.Value);
        public Volume FluidOunces => Volume.FromFluidOunces(p.Value);
        public Volume Tablespoons => Volume.FromTablespoons(p.Value);
        public Volume Teaspoons => Volume.FromTeaspoons(p.Value);
        public Volume ImperialGallons => Volume.FromImperialGallons(p.Value);
        public Volume CubicYards => Volume.FromCubicYards(p.Value);
        public Volume CubicFeet => Volume.FromCubicFeet(p.Value);
        public Volume CubicInches => Volume.FromCubicInches(p.Value);
        public Volume OilBarrels => Volume.FromOilBarrels(p.Value);
        public Speed MetersPerSecond => Speed.FromMetersPerSecond(p.Value);
        public Speed KilometersPerHour => Speed.FromKilometersPerHour(p.Value);
        public Speed MilesPerHour => Speed.FromMilesPerHour(p.Value);
        public Speed FeetPerSecond => Speed.FromFeetPerSecond(p.Value);
        public Speed Knots => Speed.FromKnots(p.Value);
        public Speed Mach => Speed.FromMach(p.Value);
        public Speed SpeedOfLight => Speed.FromSpeedOfLight(p.Value);
        public Acceleration MetersPerSecondSquared => Acceleration.FromMetersPerSecondSquared(p.Value);
        public Acceleration KilometersPerHourPerSecond => Acceleration.FromKilometersPerHourPerSecond(p.Value);
        public Acceleration FeetPerSecondSquared => Acceleration.FromFeetPerSecondSquared(p.Value);
        public Acceleration StandardGravity => Acceleration.FromStandardGravity(p.Value);
        public Acceleration Gals => Acceleration.FromGals(p.Value);
        public Angle Radians => Angle.FromRadians(p.Value);
        public Angle Turns => Angle.FromTurns(p.Value);
        public Angle Degrees => Angle.FromDegrees(p.Value);
        public Angle Gradians => Angle.FromGradians(p.Value);
        public Angle Arcminutes => Angle.FromArcminutes(p.Value);
        public Angle Arcseconds => Angle.FromArcseconds(p.Value);
        public SolidAngle Steradians => SolidAngle.FromSteradians(p.Value);
        public SolidAngle Spats => SolidAngle.FromSpats(p.Value);
        public SolidAngle SquareDegrees => SolidAngle.FromSquareDegrees(p.Value);
        public Frequency Hertz => Frequency.FromHertz(p.Value);
        public Force Newtons => Force.FromNewtons(p.Value);
        public Force Dynes => Force.FromDynes(p.Value);
        public Force KilogramsForce => Force.FromKilogramsForce(p.Value);
        public Force PoundsForce => Force.FromPoundsForce(p.Value);
        public Force OuncesForce => Force.FromOuncesForce(p.Value);
        public Force Poundals => Force.FromPoundals(p.Value);
        public Pressure Pascals => Pressure.FromPascals(p.Value);
        public Pressure Bars => Pressure.FromBars(p.Value);
        public Pressure Atmospheres => Pressure.FromAtmospheres(p.Value);
        public Pressure Torr => Pressure.FromTorr(p.Value);
        public Pressure MillimetersOfMercury => Pressure.FromMillimetersOfMercury(p.Value);
        public Pressure InchesOfMercury => Pressure.FromInchesOfMercury(p.Value);
        public Pressure InchesOfWater => Pressure.FromInchesOfWater(p.Value);
        public Pressure PoundsPerSquareInch => Pressure.FromPoundsPerSquareInch(p.Value);
        public Energy Joules => Energy.FromJoules(p.Value);
        public Energy Ergs => Energy.FromErgs(p.Value);
        public Energy Calories => Energy.FromCalories(p.Value);
        public Energy WattHours => Energy.FromWattHours(p.Value);
        public Energy Electronvolts => Energy.FromElectronvolts(p.Value);
        public Energy BritishThermalUnits => Energy.FromBritishThermalUnits(p.Value);
        public Energy FootPounds => Energy.FromFootPounds(p.Value);
        public Energy TonsOfTnt => Energy.FromTonsOfTnt(p.Value);
        public Power Watts => Power.FromWatts(p.Value);
        public Power Horsepower => Power.FromHorsepower(p.Value);
        public Power MetricHorsepower => Power.FromMetricHorsepower(p.Value);
        public Power BritishThermalUnitsPerHour => Power.FromBritishThermalUnitsPerHour(p.Value);
        public Power FootPoundsPerSecond => Power.FromFootPoundsPerSecond(p.Value);
        public ElectricCharge Coulombs => ElectricCharge.FromCoulombs(p.Value);
        public ElectricCharge AmpereHours => ElectricCharge.FromAmpereHours(p.Value);
        public ElectricCharge Faradays => ElectricCharge.FromFaradays(p.Value);
        public ElectricCharge ElementaryCharges => ElectricCharge.FromElementaryCharges(p.Value);
        public ElectricCharge Abcoulombs => ElectricCharge.FromAbcoulombs(p.Value);
        public ElectricCharge Statcoulombs => ElectricCharge.FromStatcoulombs(p.Value);
        public Voltage Volts => Voltage.FromVolts(p.Value);
        public Voltage Abvolts => Voltage.FromAbvolts(p.Value);
        public Voltage Statvolts => Voltage.FromStatvolts(p.Value);
        public Capacitance Farads => Capacitance.FromFarads(p.Value);
        public Capacitance Abfarads => Capacitance.FromAbfarads(p.Value);
        public Capacitance Statfarads => Capacitance.FromStatfarads(p.Value);
        public ElectricResistance Ohms => ElectricResistance.FromOhms(p.Value);
        public ElectricConductance Siemens => ElectricConductance.FromSiemens(p.Value);
        public ElectricConductance Mhos => ElectricConductance.FromMhos(p.Value);
        public MagneticFlux Webers => MagneticFlux.FromWebers(p.Value);
        public MagneticFlux Maxwells => MagneticFlux.FromMaxwells(p.Value);
        public MagneticFluxDensity Teslas => MagneticFluxDensity.FromTeslas(p.Value);
        public MagneticFluxDensity Gauss => MagneticFluxDensity.FromGauss(p.Value);
        public LinearMagneticFluxDensity WebersPerMeter => LinearMagneticFluxDensity.FromWebersPerMeter(p.Value);
        public Inductance Henries => Inductance.FromHenries(p.Value);
        public Inductance Abhenries => Inductance.FromAbhenries(p.Value);
        public Inductance Stathenries => Inductance.FromStathenries(p.Value);
        public LuminousFlux Lumens => LuminousFlux.FromLumens(p.Value);
        public Illuminance Lux => Illuminance.FromLux(p.Value);
        public Illuminance Phots => Illuminance.FromPhots(p.Value);
        public Illuminance Footcandles => Illuminance.FromFootcandles(p.Value);
        public Radioactivity Becquerels => Radioactivity.FromBecquerels(p.Value);
        public Radioactivity Curies => Radioactivity.FromCuries(p.Value);
        public Radioactivity Rutherfords => Radioactivity.FromRutherfords(p.Value);
        public AbsorbedDose Grays => AbsorbedDose.FromGrays(p.Value);
        public AbsorbedDose Rads => AbsorbedDose.FromRads(p.Value);
        public EquivalentDose Sieverts => EquivalentDose.FromSieverts(p.Value);
        public EquivalentDose Rems => EquivalentDose.FromRems(p.Value);
        public CatalyticActivity Katals => CatalyticActivity.FromKatals(p.Value);
        public CatalyticActivity EnzymeUnits => CatalyticActivity.FromEnzymeUnits(p.Value);
        public Jerk MetersPerSecondCubed => Jerk.FromMetersPerSecondCubed(p.Value);
        public Jerk FeetPerSecondCubed => Jerk.FromFeetPerSecondCubed(p.Value);
        public Jerk GalsPerSecond => Jerk.FromGalsPerSecond(p.Value);
        public AngularVelocity RadiansPerSecond => AngularVelocity.FromRadiansPerSecond(p.Value);
        public AngularVelocity DegreesPerSecond => AngularVelocity.FromDegreesPerSecond(p.Value);
        public AngularVelocity RevolutionsPerSecond => AngularVelocity.FromRevolutionsPerSecond(p.Value);
        public AngularAcceleration RadiansPerSecondSquared => AngularAcceleration.FromRadiansPerSecondSquared(p.Value);
        public AngularAcceleration DegreesPerSecondSquared => AngularAcceleration.FromDegreesPerSecondSquared(p.Value);
        public AngularAcceleration RevolutionsPerMinutePerSecond => AngularAcceleration.FromRevolutionsPerMinutePerSecond(p.Value);
        public VolumetricFlowRate CubicMetersPerSecond => VolumetricFlowRate.FromCubicMetersPerSecond(p.Value);
        public VolumetricFlowRate LitersPerSecond => VolumetricFlowRate.FromLitersPerSecond(p.Value);
        public VolumetricFlowRate LitersPerMinute => VolumetricFlowRate.FromLitersPerMinute(p.Value);
        public VolumetricFlowRate CubicFeetPerSecond => VolumetricFlowRate.FromCubicFeetPerSecond(p.Value);
        public VolumetricFlowRate GallonsPerMinute => VolumetricFlowRate.FromGallonsPerMinute(p.Value);
        public Wavenumber PerMeter => Wavenumber.FromPerMeter(p.Value);
        public Wavenumber PerCentimeter => Wavenumber.FromPerCentimeter(p.Value);
        public Density KilogramsPerCubicMeter => Density.FromKilogramsPerCubicMeter(p.Value);
        public Density GramsPerCubicCentimeter => Density.FromGramsPerCubicCentimeter(p.Value);
        public Density KilogramsPerLiter => Density.FromKilogramsPerLiter(p.Value);
        public Density GramsPerMilliliter => Density.FromGramsPerMilliliter(p.Value);
        public Density PoundsPerCubicFoot => Density.FromPoundsPerCubicFoot(p.Value);
        public Density PoundsPerGallon => Density.FromPoundsPerGallon(p.Value);
        public SpecificVolume CubicMetersPerKilogram => SpecificVolume.FromCubicMetersPerKilogram(p.Value);
        public SpecificVolume LitersPerKilogram => SpecificVolume.FromLitersPerKilogram(p.Value);
        public SpecificVolume CubicCentimetersPerGram => SpecificVolume.FromCubicCentimetersPerGram(p.Value);
        public LinearDensity GramsPerMeter => LinearDensity.FromGramsPerMeter(p.Value);
        public LinearDensity GramsPerCentimeter => LinearDensity.FromGramsPerCentimeter(p.Value);
        public LinearDensity Tex => LinearDensity.FromTex(p.Value);
        public LinearDensity Denier => LinearDensity.FromDenier(p.Value);
        public AreaDensity GramsPerSquareMeter => AreaDensity.FromGramsPerSquareMeter(p.Value);
        public MassFlowRate GramsPerSecond => MassFlowRate.FromGramsPerSecond(p.Value);
        public MassFlowRate KilogramsPerHour => MassFlowRate.FromKilogramsPerHour(p.Value);
        public MassFlowRate PoundsPerSecond => MassFlowRate.FromPoundsPerSecond(p.Value);
        public MassFlowRate PoundsPerHour => MassFlowRate.FromPoundsPerHour(p.Value);
        public MassFlowRate TonnesPerHour => MassFlowRate.FromTonnesPerHour(p.Value);
        public Molality MolesPerKilogram => Molality.FromMolesPerKilogram(p.Value);
        public Molality MolesPerGram => Molality.FromMolesPerGram(p.Value);
        public MolarMass GramsPerMole => MolarMass.FromGramsPerMole(p.Value);
        public Momentum KilogramMetersPerSecond => Momentum.FromKilogramMetersPerSecond(p.Value);
        public Momentum GramMetersPerSecond => Momentum.FromGramMetersPerSecond(p.Value);
        public Momentum NewtonSeconds => Momentum.FromNewtonSeconds(p.Value);
        public MomentOfInertia KilogramSquareMeters => MomentOfInertia.FromKilogramSquareMeters(p.Value);
        public MomentOfInertia GramSquareMeters => MomentOfInertia.FromGramSquareMeters(p.Value);
        public MomentOfInertia GramSquareCentimeters => MomentOfInertia.FromGramSquareCentimeters(p.Value);
        public MomentOfInertia PoundSquareFeet => MomentOfInertia.FromPoundSquareFeet(p.Value);
        public AngularMomentum GramSquareMetersPerSecond => AngularMomentum.FromGramSquareMetersPerSecond(p.Value);
        public AngularMomentum NewtonMeterSeconds => AngularMomentum.FromNewtonMeterSeconds(p.Value);
        public Torque NewtonMeters => Torque.FromNewtonMeters(p.Value);
        public Torque NewtonMillimeters => Torque.FromNewtonMillimeters(p.Value);
        public Torque KilogramForceMeters => Torque.FromKilogramForceMeters(p.Value);
        public Torque PoundFeet => Torque.FromPoundFeet(p.Value);
        public Torque PoundInches => Torque.FromPoundInches(p.Value);
        public SurfaceTension NewtonsPerMeter => SurfaceTension.FromNewtonsPerMeter(p.Value);
        public SurfaceTension DynesPerCentimeter => SurfaceTension.FromDynesPerCentimeter(p.Value);
        public DynamicViscosity PascalSeconds => DynamicViscosity.FromPascalSeconds(p.Value);
        public DynamicViscosity Poise => DynamicViscosity.FromPoise(p.Value);
        public KinematicViscosity SquareMetersPerSecond => KinematicViscosity.FromSquareMetersPerSecond(p.Value);
        public KinematicViscosity Stokes => KinematicViscosity.FromStokes(p.Value);
        public Action ErgSeconds => Action.FromErgSeconds(p.Value);
        public Action PlanckConstants => Action.FromPlanckConstants(p.Value);
        public HeatCapacity JoulesPerKelvin => HeatCapacity.FromJoulesPerKelvin(p.Value);
        public HeatCapacity CaloriesPerKelvin => HeatCapacity.FromCaloriesPerKelvin(p.Value);
        public SpecificHeatCapacity JoulesPerKilogramKelvin => SpecificHeatCapacity.FromJoulesPerKilogramKelvin(p.Value);
        public SpecificHeatCapacity JoulesPerGramKelvin => SpecificHeatCapacity.FromJoulesPerGramKelvin(p.Value);
        public SpecificHeatCapacity CaloriesPerGramKelvin => SpecificHeatCapacity.FromCaloriesPerGramKelvin(p.Value);
        public MolarHeatCapacity JoulesPerMoleKelvin => MolarHeatCapacity.FromJoulesPerMoleKelvin(p.Value);
        public MolarHeatCapacity CaloriesPerMoleKelvin => MolarHeatCapacity.FromCaloriesPerMoleKelvin(p.Value);
        public ThermalConductivity WattsPerMeterKelvin => ThermalConductivity.FromWattsPerMeterKelvin(p.Value);
        public ThermalConductivity BtuPerHourFootFahrenheit => ThermalConductivity.FromBtuPerHourFootFahrenheit(p.Value);
        public ThermalResistance KelvinsPerWatt => ThermalResistance.FromKelvinsPerWatt(p.Value);
        public HeatFluxDensity WattsPerSquareMeter => HeatFluxDensity.FromWattsPerSquareMeter(p.Value);
        public HeatFluxDensity WattsPerSquareCentimeter => HeatFluxDensity.FromWattsPerSquareCentimeter(p.Value);
        public ElectricFieldStrength VoltsPerMeter => ElectricFieldStrength.FromVoltsPerMeter(p.Value);
        public ElectricFieldStrength VoltsPerCentimeter => ElectricFieldStrength.FromVoltsPerCentimeter(p.Value);
        public ChargeDensity CoulombsPerCubicMeter => ChargeDensity.FromCoulombsPerCubicMeter(p.Value);
        public SurfaceChargeDensity CoulombsPerSquareMeter => SurfaceChargeDensity.FromCoulombsPerSquareMeter(p.Value);
        public CurrentDensity AmperesPerSquareMeter => CurrentDensity.FromAmperesPerSquareMeter(p.Value);
        public CurrentDensity AmperesPerSquareCentimeter => CurrentDensity.FromAmperesPerSquareCentimeter(p.Value);
        public Permittivity FaradsPerMeter => Permittivity.FromFaradsPerMeter(p.Value);
        public Permeability HenriesPerMeter => Permeability.FromHenriesPerMeter(p.Value);
        public MagneticFieldStrength AmperesPerMeter => MagneticFieldStrength.FromAmperesPerMeter(p.Value);
        public MagneticFieldStrength Oersteds => MagneticFieldStrength.FromOersteds(p.Value);
        public Resistivity OhmMeters => Resistivity.FromOhmMeters(p.Value);
        public Resistivity OhmCentimeters => Resistivity.FromOhmCentimeters(p.Value);
        public Conductivity SiemensPerMeter => Conductivity.FromSiemensPerMeter(p.Value);
        public Conductivity SiemensPerCentimeter => Conductivity.FromSiemensPerCentimeter(p.Value);
        public ElectricDipoleMoment CoulombMeters => ElectricDipoleMoment.FromCoulombMeters(p.Value);
        public ElectricDipoleMoment Debyes => ElectricDipoleMoment.FromDebyes(p.Value);
        public Luminance CandelasPerSquareMeter => Luminance.FromCandelasPerSquareMeter(p.Value);
        public Luminance Nits => Luminance.FromNits(p.Value);
        public Luminance Stilbs => Luminance.FromStilbs(p.Value);
        public Luminance FootLamberts => Luminance.FromFootLamberts(p.Value);
        public LuminousEnergy LumenSeconds => LuminousEnergy.FromLumenSeconds(p.Value);
        public LuminousEnergy LumenHours => LuminousEnergy.FromLumenHours(p.Value);
        public LuminousEnergy Talbots => LuminousEnergy.FromTalbots(p.Value);
        public LuminousExposure LuxSeconds => LuminousExposure.FromLuxSeconds(p.Value);
        public LuminousExposure LuxHours => LuminousExposure.FromLuxHours(p.Value);
        public Radiance WattsPerSquareMeterSteradian => Radiance.FromWattsPerSquareMeterSteradian(p.Value);
        public RadiantIntensity WattsPerSteradian => RadiantIntensity.FromWattsPerSteradian(p.Value);
        public Exposure CoulombsPerGram => Exposure.FromCoulombsPerGram(p.Value);
        public Exposure CoulombsPerKilogram => Exposure.FromCoulombsPerKilogram(p.Value);
        public Exposure Roentgens => Exposure.FromRoentgens(p.Value);
        public DoseRate GraysPerSecond => DoseRate.FromGraysPerSecond(p.Value);
        public DoseRate GraysPerHour => DoseRate.FromGraysPerHour(p.Value);
        public Concentration MolesPerCubicMeter => Concentration.FromMolesPerCubicMeter(p.Value);
        public Concentration MolesPerLiter => Concentration.FromMolesPerLiter(p.Value);
        public CatalyticConcentration KatalsPerCubicMeter => CatalyticConcentration.FromKatalsPerCubicMeter(p.Value);
        public CatalyticConcentration KatalsPerLiter => CatalyticConcentration.FromKatalsPerLiter(p.Value);
        public ReactionRate MolesPerCubicMeterSecond => ReactionRate.FromMolesPerCubicMeterSecond(p.Value);
        public ReactionRate MolesPerLiterSecond => ReactionRate.FromMolesPerLiterSecond(p.Value);
        public Ratio Ratio => Ratio.FromRatio(p.Value);
        public Ratio Percent => Ratio.FromPercent(p.Value);
        public Ratio PerMille => Ratio.FromPerMille(p.Value);
        public Ratio PartsPerMillion => Ratio.FromPartsPerMillion(p.Value);
        public Ratio PartsPerBillion => Ratio.FromPartsPerBillion(p.Value);
        public Ratio PartsPerTrillion => Ratio.FromPartsPerTrillion(p.Value);
        public Ratio Decibels => Ratio.FromDecibels(p.Value);
    }

    extension(Length x) { public Reader<Length> To => new(x, 1.0); }
    extension(Mass x) { public Reader<Mass> To => new(x, 1.0); }
    extension(Duration x) { public Reader<Duration> To => new(x, 1.0); }
    extension(ElectricCurrent x) { public Reader<ElectricCurrent> To => new(x, 1.0); }
    extension(Temperature x) { public Reader<Temperature> To => new(x, 1.0); }
    extension(Quantity x) { public Reader<Quantity> To => new(x, 1.0); }
    extension(LuminousIntensity x) { public Reader<LuminousIntensity> To => new(x, 1.0); }
    extension(Area x) { public Reader<Area> To => new(x, 1.0); }
    extension(Volume x) { public Reader<Volume> To => new(x, 1.0); }
    extension(Speed x) { public Reader<Speed> To => new(x, 1.0); }
    extension(Acceleration x) { public Reader<Acceleration> To => new(x, 1.0); }
    extension(Angle x) { public Reader<Angle> To => new(x, 1.0); }
    extension(SolidAngle x) { public Reader<SolidAngle> To => new(x, 1.0); }
    extension(Frequency x) { public Reader<Frequency> To => new(x, 1.0); }
    extension(Force x) { public Reader<Force> To => new(x, 1.0); }
    extension(Pressure x) { public Reader<Pressure> To => new(x, 1.0); }
    extension(Energy x) { public Reader<Energy> To => new(x, 1.0); }
    extension(Power x) { public Reader<Power> To => new(x, 1.0); }
    extension(ElectricCharge x) { public Reader<ElectricCharge> To => new(x, 1.0); }
    extension(Voltage x) { public Reader<Voltage> To => new(x, 1.0); }
    extension(Capacitance x) { public Reader<Capacitance> To => new(x, 1.0); }
    extension(ElectricResistance x) { public Reader<ElectricResistance> To => new(x, 1.0); }
    extension(ElectricConductance x) { public Reader<ElectricConductance> To => new(x, 1.0); }
    extension(MagneticFlux x) { public Reader<MagneticFlux> To => new(x, 1.0); }
    extension(MagneticFluxDensity x) { public Reader<MagneticFluxDensity> To => new(x, 1.0); }
    extension(LinearMagneticFluxDensity x) { public Reader<LinearMagneticFluxDensity> To => new(x, 1.0); }
    extension(Inductance x) { public Reader<Inductance> To => new(x, 1.0); }
    extension(LuminousFlux x) { public Reader<LuminousFlux> To => new(x, 1.0); }
    extension(Illuminance x) { public Reader<Illuminance> To => new(x, 1.0); }
    extension(Radioactivity x) { public Reader<Radioactivity> To => new(x, 1.0); }
    extension(AbsorbedDose x) { public Reader<AbsorbedDose> To => new(x, 1.0); }
    extension(EquivalentDose x) { public Reader<EquivalentDose> To => new(x, 1.0); }
    extension(CatalyticActivity x) { public Reader<CatalyticActivity> To => new(x, 1.0); }
    extension(Jerk x) { public Reader<Jerk> To => new(x, 1.0); }
    extension(AngularVelocity x) { public Reader<AngularVelocity> To => new(x, 1.0); }
    extension(AngularAcceleration x) { public Reader<AngularAcceleration> To => new(x, 1.0); }
    extension(VolumetricFlowRate x) { public Reader<VolumetricFlowRate> To => new(x, 1.0); }
    extension(Wavenumber x) { public Reader<Wavenumber> To => new(x, 1.0); }
    extension(Density x) { public Reader<Density> To => new(x, 1.0); }
    extension(SpecificVolume x) { public Reader<SpecificVolume> To => new(x, 1.0); }
    extension(LinearDensity x) { public Reader<LinearDensity> To => new(x, 1.0); }
    extension(AreaDensity x) { public Reader<AreaDensity> To => new(x, 1.0); }
    extension(MassFlowRate x) { public Reader<MassFlowRate> To => new(x, 1.0); }
    extension(Molality x) { public Reader<Molality> To => new(x, 1.0); }
    extension(MolarMass x) { public Reader<MolarMass> To => new(x, 1.0); }
    extension(Momentum x) { public Reader<Momentum> To => new(x, 1.0); }
    extension(MomentOfInertia x) { public Reader<MomentOfInertia> To => new(x, 1.0); }
    extension(AngularMomentum x) { public Reader<AngularMomentum> To => new(x, 1.0); }
    extension(Torque x) { public Reader<Torque> To => new(x, 1.0); }
    extension(SurfaceTension x) { public Reader<SurfaceTension> To => new(x, 1.0); }
    extension(DynamicViscosity x) { public Reader<DynamicViscosity> To => new(x, 1.0); }
    extension(KinematicViscosity x) { public Reader<KinematicViscosity> To => new(x, 1.0); }
    extension(Action x) { public Reader<Action> To => new(x, 1.0); }
    extension(HeatCapacity x) { public Reader<HeatCapacity> To => new(x, 1.0); }
    extension(SpecificHeatCapacity x) { public Reader<SpecificHeatCapacity> To => new(x, 1.0); }
    extension(MolarHeatCapacity x) { public Reader<MolarHeatCapacity> To => new(x, 1.0); }
    extension(ThermalConductivity x) { public Reader<ThermalConductivity> To => new(x, 1.0); }
    extension(ThermalResistance x) { public Reader<ThermalResistance> To => new(x, 1.0); }
    extension(HeatFluxDensity x) { public Reader<HeatFluxDensity> To => new(x, 1.0); }
    extension(ElectricFieldStrength x) { public Reader<ElectricFieldStrength> To => new(x, 1.0); }
    extension(ChargeDensity x) { public Reader<ChargeDensity> To => new(x, 1.0); }
    extension(SurfaceChargeDensity x) { public Reader<SurfaceChargeDensity> To => new(x, 1.0); }
    extension(CurrentDensity x) { public Reader<CurrentDensity> To => new(x, 1.0); }
    extension(Permittivity x) { public Reader<Permittivity> To => new(x, 1.0); }
    extension(Permeability x) { public Reader<Permeability> To => new(x, 1.0); }
    extension(MagneticFieldStrength x) { public Reader<MagneticFieldStrength> To => new(x, 1.0); }
    extension(Resistivity x) { public Reader<Resistivity> To => new(x, 1.0); }
    extension(Conductivity x) { public Reader<Conductivity> To => new(x, 1.0); }
    extension(ElectricDipoleMoment x) { public Reader<ElectricDipoleMoment> To => new(x, 1.0); }
    extension(Luminance x) { public Reader<Luminance> To => new(x, 1.0); }
    extension(LuminousEnergy x) { public Reader<LuminousEnergy> To => new(x, 1.0); }
    extension(LuminousExposure x) { public Reader<LuminousExposure> To => new(x, 1.0); }
    extension(Radiance x) { public Reader<Radiance> To => new(x, 1.0); }
    extension(RadiantIntensity x) { public Reader<RadiantIntensity> To => new(x, 1.0); }
    extension(Exposure x) { public Reader<Exposure> To => new(x, 1.0); }
    extension(DoseRate x) { public Reader<DoseRate> To => new(x, 1.0); }
    extension(Concentration x) { public Reader<Concentration> To => new(x, 1.0); }
    extension(CatalyticConcentration x) { public Reader<CatalyticConcentration> To => new(x, 1.0); }
    extension(ReactionRate x) { public Reader<ReactionRate> To => new(x, 1.0); }
    extension(Ratio x) { public Reader<Ratio> To => new(x, 1.0); }

    extension(Reader<Length> r) {
        public double Meters => r.Value.ToMeters() / r.Factor;
        public double Angstroms => r.Value.ToAngstroms() / r.Factor;
        public double Leagues => r.Value.ToLeagues() / r.Factor;
        public double NauticalMiles => r.Value.ToNauticalMiles() / r.Factor;
        public double Miles => r.Value.ToMiles() / r.Factor;
        public double Furlongs => r.Value.ToFurlongs() / r.Factor;
        public double Chains => r.Value.ToChains() / r.Factor;
        public double Rods => r.Value.ToRods() / r.Factor;
        public double Fathoms => r.Value.ToFathoms() / r.Factor;
        public double Yards => r.Value.ToYards() / r.Factor;
        public double Feet => r.Value.ToFeet() / r.Factor;
        public double Inches => r.Value.ToInches() / r.Factor;
        public double HubbleLengths => r.Value.ToHubbleLengths() / r.Factor;
        public double Parsecs => r.Value.ToParsecs() / r.Factor;
        public double LightYears => r.Value.ToLightYears() / r.Factor;
        public double AstronomicalUnits => r.Value.ToAstronomicalUnits() / r.Factor;
        public double PlanckLengths => r.Value.ToPlanckLengths() / r.Factor;
    }
    extension(Reader<Mass> r) {
        public double Tonnes => r.Value.ToTonnes() / r.Factor;
        public double Grams => r.Value.ToGrams() / r.Factor;
        public double LongTons => r.Value.ToLongTons() / r.Factor;
        public double ShortTons => r.Value.ToShortTons() / r.Factor;
        public double Stones => r.Value.ToStones() / r.Factor;
        public double Pounds => r.Value.ToPounds() / r.Factor;
        public double Ounces => r.Value.ToOunces() / r.Factor;
        public double Drams => r.Value.ToDrams() / r.Factor;
        public double Grains => r.Value.ToGrains() / r.Factor;
        public double Slugs => r.Value.ToSlugs() / r.Factor;
        public double TroyPounds => r.Value.ToTroyPounds() / r.Factor;
        public double TroyOunces => r.Value.ToTroyOunces() / r.Factor;
        public double Pennyweights => r.Value.ToPennyweights() / r.Factor;
        public double Carats => r.Value.ToCarats() / r.Factor;
        public double Daltons => r.Value.ToDaltons() / r.Factor;
        public double ProtonMasses => r.Value.ToProtonMasses() / r.Factor;
        public double ElectronMasses => r.Value.ToElectronMasses() / r.Factor;
        public double PlanckMasses => r.Value.ToPlanckMasses() / r.Factor;
        public double SolarMasses => r.Value.ToSolarMasses() / r.Factor;
        public double JupiterMasses => r.Value.ToJupiterMasses() / r.Factor;
        public double EarthMasses => r.Value.ToEarthMasses() / r.Factor;
        public double LunarMasses => r.Value.ToLunarMasses() / r.Factor;
    }
    extension(Reader<Duration> r) {
        public double Seconds => r.Value.ToSeconds() / r.Factor;
        public double Minutes => r.Value.ToMinutes() / r.Factor;
        public double Hours => r.Value.ToHours() / r.Factor;
        public double Days => r.Value.ToDays() / r.Factor;
        public double Weeks => r.Value.ToWeeks() / r.Factor;
        public double Fortnights => r.Value.ToFortnights() / r.Factor;
        public double CommonYears => r.Value.ToCommonYears() / r.Factor;
        public double JulianYears => r.Value.ToJulianYears() / r.Factor;
        public double TropicalYears => r.Value.ToTropicalYears() / r.Factor;
        public double SiderealYears => r.Value.ToSiderealYears() / r.Factor;
        public double SiderealDays => r.Value.ToSiderealDays() / r.Factor;
        public double Decades => r.Value.ToDecades() / r.Factor;
        public double Centuries => r.Value.ToCenturies() / r.Factor;
        public double Millennia => r.Value.ToMillennia() / r.Factor;
        public double Annums => r.Value.ToAnnums() / r.Factor;
        public double HubbleTimes => r.Value.ToHubbleTimes() / r.Factor;
        public double PlanckTimes => r.Value.ToPlanckTimes() / r.Factor;
    }
    extension(Reader<ElectricCurrent> r) {
        public double Amperes => r.Value.ToAmperes() / r.Factor;
        public double Abamperes => r.Value.ToAbamperes() / r.Factor;
        public double Statamperes => r.Value.ToStatamperes() / r.Factor;
    }
    extension(Reader<Temperature> r) {
        public double Kelvin => r.Value.ToKelvin() / r.Factor;
        public double Rankine => r.Value.ToRankine() / r.Factor;
        public double Celsius => r.Value.ToCelsius() / r.Factor;
        public double Fahrenheit => r.Value.ToFahrenheit() / r.Factor;
        public double Reaumur => r.Value.ToReaumur() / r.Factor;
        public double Delisle => r.Value.ToDelisle() / r.Factor;
        public double Newton => r.Value.ToNewton() / r.Factor;
        public double Romer => r.Value.ToRomer() / r.Factor;
    }
    extension(Reader<Quantity> r) {
        public double Moles => r.Value.ToMoles() / r.Factor;
        public double Count => r.Value.ToCount() / r.Factor;
        public double Pairs => r.Value.ToPairs() / r.Factor;
        public double Dozens => r.Value.ToDozens() / r.Factor;
        public double Scores => r.Value.ToScores() / r.Factor;
        public double Gross => r.Value.ToGross() / r.Factor;
    }
    extension(Reader<LuminousIntensity> r) {
        public double Candelas => r.Value.ToCandelas() / r.Factor;
        public double Candlepower => r.Value.ToCandlepower() / r.Factor;
        public double Hefnerkerze => r.Value.ToHefnerkerze() / r.Factor;
        public double Carcels => r.Value.ToCarcels() / r.Factor;
    }
    extension(Reader<Area> r) {
        public double SquareKilometers => r.Value.ToSquareKilometers() / r.Factor;
        public double Hectares => r.Value.ToHectares() / r.Factor;
        public double Ares => r.Value.ToAres() / r.Factor;
        public double SquareMeters => r.Value.ToSquareMeters() / r.Factor;
        public double SquareCentimeters => r.Value.ToSquareCentimeters() / r.Factor;
        public double SquareMillimeters => r.Value.ToSquareMillimeters() / r.Factor;
        public double SquareMiles => r.Value.ToSquareMiles() / r.Factor;
        public double Acres => r.Value.ToAcres() / r.Factor;
        public double SquareYards => r.Value.ToSquareYards() / r.Factor;
        public double SquareFeet => r.Value.ToSquareFeet() / r.Factor;
        public double SquareInches => r.Value.ToSquareInches() / r.Factor;
        public double Barns => r.Value.ToBarns() / r.Factor;
    }
    extension(Reader<Volume> r) {
        public double CubicMeters => r.Value.ToCubicMeters() / r.Factor;
        public double CubicCentimeters => r.Value.ToCubicCentimeters() / r.Factor;
        public double CubicMillimeters => r.Value.ToCubicMillimeters() / r.Factor;
        public double Liters => r.Value.ToLiters() / r.Factor;
        public double Gallons => r.Value.ToGallons() / r.Factor;
        public double Quarts => r.Value.ToQuarts() / r.Factor;
        public double Pints => r.Value.ToPints() / r.Factor;
        public double Cups => r.Value.ToCups() / r.Factor;
        public double FluidOunces => r.Value.ToFluidOunces() / r.Factor;
        public double Tablespoons => r.Value.ToTablespoons() / r.Factor;
        public double Teaspoons => r.Value.ToTeaspoons() / r.Factor;
        public double ImperialGallons => r.Value.ToImperialGallons() / r.Factor;
        public double CubicYards => r.Value.ToCubicYards() / r.Factor;
        public double CubicFeet => r.Value.ToCubicFeet() / r.Factor;
        public double CubicInches => r.Value.ToCubicInches() / r.Factor;
        public double OilBarrels => r.Value.ToOilBarrels() / r.Factor;
    }
    extension(Reader<Speed> r) {
        public double MetersPerSecond => r.Value.ToMetersPerSecond() / r.Factor;
        public double KilometersPerHour => r.Value.ToKilometersPerHour() / r.Factor;
        public double MilesPerHour => r.Value.ToMilesPerHour() / r.Factor;
        public double FeetPerSecond => r.Value.ToFeetPerSecond() / r.Factor;
        public double Knots => r.Value.ToKnots() / r.Factor;
        public double Mach => r.Value.ToMach() / r.Factor;
        public double SpeedOfLight => r.Value.ToSpeedOfLight() / r.Factor;
    }
    extension(Reader<Acceleration> r) {
        public double MetersPerSecondSquared => r.Value.ToMetersPerSecondSquared() / r.Factor;
        public double KilometersPerHourPerSecond => r.Value.ToKilometersPerHourPerSecond() / r.Factor;
        public double FeetPerSecondSquared => r.Value.ToFeetPerSecondSquared() / r.Factor;
        public double StandardGravity => r.Value.ToStandardGravity() / r.Factor;
        public double Gals => r.Value.ToGals() / r.Factor;
    }
    extension(Reader<Angle> r) {
        public double Radians => r.Value.ToRadians() / r.Factor;
        public double Turns => r.Value.ToTurns() / r.Factor;
        public double Degrees => r.Value.ToDegrees() / r.Factor;
        public double Gradians => r.Value.ToGradians() / r.Factor;
        public double Arcminutes => r.Value.ToArcminutes() / r.Factor;
        public double Arcseconds => r.Value.ToArcseconds() / r.Factor;
    }
    extension(Reader<SolidAngle> r) {
        public double Steradians => r.Value.ToSteradians() / r.Factor;
        public double Spats => r.Value.ToSpats() / r.Factor;
        public double SquareDegrees => r.Value.ToSquareDegrees() / r.Factor;
    }
    extension(Reader<Frequency> r) {
        public double Hertz => r.Value.ToHertz() / r.Factor;
        public double RevolutionsPerMinute => r.Value.ToRevolutionsPerMinute() / r.Factor;
    }
    extension(Reader<Force> r) {
        public double Newtons => r.Value.ToNewtons() / r.Factor;
        public double Dynes => r.Value.ToDynes() / r.Factor;
        public double KilogramsForce => r.Value.ToKilogramsForce() / r.Factor;
        public double PoundsForce => r.Value.ToPoundsForce() / r.Factor;
        public double OuncesForce => r.Value.ToOuncesForce() / r.Factor;
        public double Poundals => r.Value.ToPoundals() / r.Factor;
    }
    extension(Reader<Pressure> r) {
        public double Pascals => r.Value.ToPascals() / r.Factor;
        public double Bars => r.Value.ToBars() / r.Factor;
        public double Atmospheres => r.Value.ToAtmospheres() / r.Factor;
        public double Torr => r.Value.ToTorr() / r.Factor;
        public double MillimetersOfMercury => r.Value.ToMillimetersOfMercury() / r.Factor;
        public double InchesOfMercury => r.Value.ToInchesOfMercury() / r.Factor;
        public double InchesOfWater => r.Value.ToInchesOfWater() / r.Factor;
        public double PoundsPerSquareInch => r.Value.ToPoundsPerSquareInch() / r.Factor;
    }
    extension(Reader<Energy> r) {
        public double Joules => r.Value.ToJoules() / r.Factor;
        public double Ergs => r.Value.ToErgs() / r.Factor;
        public double Calories => r.Value.ToCalories() / r.Factor;
        public double WattHours => r.Value.ToWattHours() / r.Factor;
        public double Electronvolts => r.Value.ToElectronvolts() / r.Factor;
        public double BritishThermalUnits => r.Value.ToBritishThermalUnits() / r.Factor;
        public double FootPounds => r.Value.ToFootPounds() / r.Factor;
        public double TonsOfTnt => r.Value.ToTonsOfTnt() / r.Factor;
    }
    extension(Reader<Power> r) {
        public double Watts => r.Value.ToWatts() / r.Factor;
        public double Horsepower => r.Value.ToHorsepower() / r.Factor;
        public double MetricHorsepower => r.Value.ToMetricHorsepower() / r.Factor;
        public double BritishThermalUnitsPerHour => r.Value.ToBritishThermalUnitsPerHour() / r.Factor;
        public double FootPoundsPerSecond => r.Value.ToFootPoundsPerSecond() / r.Factor;
    }
    extension(Reader<ElectricCharge> r) {
        public double Coulombs => r.Value.ToCoulombs() / r.Factor;
        public double AmpereHours => r.Value.ToAmpereHours() / r.Factor;
        public double MilliampereHours => r.Value.ToMilliampereHours() / r.Factor;
        public double Faradays => r.Value.ToFaradays() / r.Factor;
        public double ElementaryCharges => r.Value.ToElementaryCharges() / r.Factor;
        public double Abcoulombs => r.Value.ToAbcoulombs() / r.Factor;
        public double Statcoulombs => r.Value.ToStatcoulombs() / r.Factor;
    }
    extension(Reader<Voltage> r) {
        public double Volts => r.Value.ToVolts() / r.Factor;
        public double Abvolts => r.Value.ToAbvolts() / r.Factor;
        public double Statvolts => r.Value.ToStatvolts() / r.Factor;
    }
    extension(Reader<Capacitance> r) {
        public double Farads => r.Value.ToFarads() / r.Factor;
        public double Abfarads => r.Value.ToAbfarads() / r.Factor;
        public double Statfarads => r.Value.ToStatfarads() / r.Factor;
    }
    extension(Reader<ElectricResistance> r) {
        public double Ohms => r.Value.ToOhms() / r.Factor;
    }
    extension(Reader<ElectricConductance> r) {
        public double Siemens => r.Value.ToSiemens() / r.Factor;
        public double Mhos => r.Value.ToMhos() / r.Factor;
    }
    extension(Reader<MagneticFlux> r) {
        public double Webers => r.Value.ToWebers() / r.Factor;
        public double Maxwells => r.Value.ToMaxwells() / r.Factor;
    }
    extension(Reader<MagneticFluxDensity> r) {
        public double Teslas => r.Value.ToTeslas() / r.Factor;
        public double Gauss => r.Value.ToGauss() / r.Factor;
    }
    extension(Reader<LinearMagneticFluxDensity> r) {
        public double WebersPerMeter => r.Value.ToWebersPerMeter() / r.Factor;
    }
    extension(Reader<Inductance> r) {
        public double Henries => r.Value.ToHenries() / r.Factor;
        public double Abhenries => r.Value.ToAbhenries() / r.Factor;
        public double Stathenries => r.Value.ToStathenries() / r.Factor;
    }
    extension(Reader<LuminousFlux> r) {
        public double Lumens => r.Value.ToLumens() / r.Factor;
    }
    extension(Reader<Illuminance> r) {
        public double Lux => r.Value.ToLux() / r.Factor;
        public double Phots => r.Value.ToPhots() / r.Factor;
        public double Footcandles => r.Value.ToFootcandles() / r.Factor;
    }
    extension(Reader<Radioactivity> r) {
        public double Becquerels => r.Value.ToBecquerels() / r.Factor;
        public double Curies => r.Value.ToCuries() / r.Factor;
        public double Rutherfords => r.Value.ToRutherfords() / r.Factor;
    }
    extension(Reader<AbsorbedDose> r) {
        public double Grays => r.Value.ToGrays() / r.Factor;
        public double Rads => r.Value.ToRads() / r.Factor;
    }
    extension(Reader<EquivalentDose> r) {
        public double Sieverts => r.Value.ToSieverts() / r.Factor;
        public double Rems => r.Value.ToRems() / r.Factor;
    }
    extension(Reader<CatalyticActivity> r) {
        public double Katals => r.Value.ToKatals() / r.Factor;
        public double EnzymeUnits => r.Value.ToEnzymeUnits() / r.Factor;
    }
    extension(Reader<Jerk> r) {
        public double MetersPerSecondCubed => r.Value.ToMetersPerSecondCubed() / r.Factor;
        public double FeetPerSecondCubed => r.Value.ToFeetPerSecondCubed() / r.Factor;
        public double GalsPerSecond => r.Value.ToGalsPerSecond() / r.Factor;
    }
    extension(Reader<AngularVelocity> r) {
        public double RadiansPerSecond => r.Value.ToRadiansPerSecond() / r.Factor;
        public double DegreesPerSecond => r.Value.ToDegreesPerSecond() / r.Factor;
        public double RevolutionsPerSecond => r.Value.ToRevolutionsPerSecond() / r.Factor;
        public double RevolutionsPerMinute => r.Value.ToRevolutionsPerMinute() / r.Factor;
    }
    extension(Reader<AngularAcceleration> r) {
        public double RadiansPerSecondSquared => r.Value.ToRadiansPerSecondSquared() / r.Factor;
        public double DegreesPerSecondSquared => r.Value.ToDegreesPerSecondSquared() / r.Factor;
        public double RevolutionsPerMinutePerSecond => r.Value.ToRevolutionsPerMinutePerSecond() / r.Factor;
    }
    extension(Reader<VolumetricFlowRate> r) {
        public double CubicMetersPerSecond => r.Value.ToCubicMetersPerSecond() / r.Factor;
        public double LitersPerSecond => r.Value.ToLitersPerSecond() / r.Factor;
        public double LitersPerMinute => r.Value.ToLitersPerMinute() / r.Factor;
        public double CubicFeetPerSecond => r.Value.ToCubicFeetPerSecond() / r.Factor;
        public double GallonsPerMinute => r.Value.ToGallonsPerMinute() / r.Factor;
    }
    extension(Reader<Wavenumber> r) {
        public double PerMeter => r.Value.ToPerMeter() / r.Factor;
        public double PerCentimeter => r.Value.ToPerCentimeter() / r.Factor;
    }
    extension(Reader<Density> r) {
        public double KilogramsPerCubicMeter => r.Value.ToKilogramsPerCubicMeter() / r.Factor;
        public double GramsPerCubicCentimeter => r.Value.ToGramsPerCubicCentimeter() / r.Factor;
        public double KilogramsPerLiter => r.Value.ToKilogramsPerLiter() / r.Factor;
        public double GramsPerMilliliter => r.Value.ToGramsPerMilliliter() / r.Factor;
        public double PoundsPerCubicFoot => r.Value.ToPoundsPerCubicFoot() / r.Factor;
        public double PoundsPerGallon => r.Value.ToPoundsPerGallon() / r.Factor;
    }
    extension(Reader<SpecificVolume> r) {
        public double CubicMetersPerKilogram => r.Value.ToCubicMetersPerKilogram() / r.Factor;
        public double LitersPerKilogram => r.Value.ToLitersPerKilogram() / r.Factor;
        public double CubicCentimetersPerGram => r.Value.ToCubicCentimetersPerGram() / r.Factor;
    }
    extension(Reader<LinearDensity> r) {
        public double GramsPerMeter => r.Value.ToGramsPerMeter() / r.Factor;
        public double GramsPerCentimeter => r.Value.ToGramsPerCentimeter() / r.Factor;
        public double Tex => r.Value.ToTex() / r.Factor;
        public double Denier => r.Value.ToDenier() / r.Factor;
    }
    extension(Reader<AreaDensity> r) {
        public double GramsPerSquareMeter => r.Value.ToGramsPerSquareMeter() / r.Factor;
    }
    extension(Reader<MassFlowRate> r) {
        public double GramsPerSecond => r.Value.ToGramsPerSecond() / r.Factor;
        public double KilogramsPerHour => r.Value.ToKilogramsPerHour() / r.Factor;
        public double PoundsPerSecond => r.Value.ToPoundsPerSecond() / r.Factor;
        public double PoundsPerHour => r.Value.ToPoundsPerHour() / r.Factor;
        public double TonnesPerHour => r.Value.ToTonnesPerHour() / r.Factor;
    }
    extension(Reader<Molality> r) {
        public double MolesPerKilogram => r.Value.ToMolesPerKilogram() / r.Factor;
        public double MolesPerGram => r.Value.ToMolesPerGram() / r.Factor;
    }
    extension(Reader<MolarMass> r) {
        public double GramsPerMole => r.Value.ToGramsPerMole() / r.Factor;
    }
    extension(Reader<Momentum> r) {
        public double GramMetersPerSecond => r.Value.ToGramMetersPerSecond() / r.Factor;
        public double NewtonSeconds => r.Value.ToNewtonSeconds() / r.Factor;
    }
    extension(Reader<MomentOfInertia> r) {
        public double KilogramSquareMeters => r.Value.ToKilogramSquareMeters() / r.Factor;
        public double GramSquareMeters => r.Value.ToGramSquareMeters() / r.Factor;
        public double GramSquareCentimeters => r.Value.ToGramSquareCentimeters() / r.Factor;
        public double PoundSquareFeet => r.Value.ToPoundSquareFeet() / r.Factor;
    }
    extension(Reader<AngularMomentum> r) {
        public double GramSquareMetersPerSecond => r.Value.ToGramSquareMetersPerSecond() / r.Factor;
        public double JouleSeconds => r.Value.ToJouleSeconds() / r.Factor;
        public double NewtonMeterSeconds => r.Value.ToNewtonMeterSeconds() / r.Factor;
    }
    extension(Reader<Torque> r) {
        public double NewtonMeters => r.Value.ToNewtonMeters() / r.Factor;
        public double NewtonMillimeters => r.Value.ToNewtonMillimeters() / r.Factor;
        public double KilogramForceMeters => r.Value.ToKilogramForceMeters() / r.Factor;
        public double PoundFeet => r.Value.ToPoundFeet() / r.Factor;
        public double PoundInches => r.Value.ToPoundInches() / r.Factor;
    }
    extension(Reader<SurfaceTension> r) {
        public double NewtonsPerMeter => r.Value.ToNewtonsPerMeter() / r.Factor;
        public double DynesPerCentimeter => r.Value.ToDynesPerCentimeter() / r.Factor;
    }
    extension(Reader<DynamicViscosity> r) {
        public double PascalSeconds => r.Value.ToPascalSeconds() / r.Factor;
        public double Poise => r.Value.ToPoise() / r.Factor;
    }
    extension(Reader<KinematicViscosity> r) {
        public double SquareMetersPerSecond => r.Value.ToSquareMetersPerSecond() / r.Factor;
        public double Stokes => r.Value.ToStokes() / r.Factor;
    }
    extension(Reader<Action> r) {
        public double JouleSeconds => r.Value.ToJouleSeconds() / r.Factor;
        public double ErgSeconds => r.Value.ToErgSeconds() / r.Factor;
        public double PlanckConstants => r.Value.ToPlanckConstants() / r.Factor;
    }
    extension(Reader<HeatCapacity> r) {
        public double JoulesPerKelvin => r.Value.ToJoulesPerKelvin() / r.Factor;
        public double CaloriesPerKelvin => r.Value.ToCaloriesPerKelvin() / r.Factor;
    }
    extension(Reader<SpecificHeatCapacity> r) {
        public double JoulesPerKilogramKelvin => r.Value.ToJoulesPerKilogramKelvin() / r.Factor;
        public double JoulesPerGramKelvin => r.Value.ToJoulesPerGramKelvin() / r.Factor;
        public double CaloriesPerGramKelvin => r.Value.ToCaloriesPerGramKelvin() / r.Factor;
    }
    extension(Reader<MolarHeatCapacity> r) {
        public double JoulesPerMoleKelvin => r.Value.ToJoulesPerMoleKelvin() / r.Factor;
        public double CaloriesPerMoleKelvin => r.Value.ToCaloriesPerMoleKelvin() / r.Factor;
    }
    extension(Reader<ThermalConductivity> r) {
        public double WattsPerMeterKelvin => r.Value.ToWattsPerMeterKelvin() / r.Factor;
        public double BtuPerHourFootFahrenheit => r.Value.ToBtuPerHourFootFahrenheit() / r.Factor;
    }
    extension(Reader<ThermalResistance> r) {
        public double KelvinsPerWatt => r.Value.ToKelvinsPerWatt() / r.Factor;
    }
    extension(Reader<HeatFluxDensity> r) {
        public double WattsPerSquareMeter => r.Value.ToWattsPerSquareMeter() / r.Factor;
        public double WattsPerSquareCentimeter => r.Value.ToWattsPerSquareCentimeter() / r.Factor;
    }
    extension(Reader<ElectricFieldStrength> r) {
        public double VoltsPerMeter => r.Value.ToVoltsPerMeter() / r.Factor;
        public double VoltsPerCentimeter => r.Value.ToVoltsPerCentimeter() / r.Factor;
    }
    extension(Reader<ChargeDensity> r) {
        public double CoulombsPerCubicMeter => r.Value.ToCoulombsPerCubicMeter() / r.Factor;
    }
    extension(Reader<SurfaceChargeDensity> r) {
        public double CoulombsPerSquareMeter => r.Value.ToCoulombsPerSquareMeter() / r.Factor;
    }
    extension(Reader<CurrentDensity> r) {
        public double AmperesPerSquareMeter => r.Value.ToAmperesPerSquareMeter() / r.Factor;
        public double AmperesPerSquareCentimeter => r.Value.ToAmperesPerSquareCentimeter() / r.Factor;
    }
    extension(Reader<Permittivity> r) {
        public double FaradsPerMeter => r.Value.ToFaradsPerMeter() / r.Factor;
    }
    extension(Reader<Permeability> r) {
        public double HenriesPerMeter => r.Value.ToHenriesPerMeter() / r.Factor;
    }
    extension(Reader<MagneticFieldStrength> r) {
        public double AmperesPerMeter => r.Value.ToAmperesPerMeter() / r.Factor;
        public double Oersteds => r.Value.ToOersteds() / r.Factor;
    }
    extension(Reader<Resistivity> r) {
        public double OhmMeters => r.Value.ToOhmMeters() / r.Factor;
        public double OhmCentimeters => r.Value.ToOhmCentimeters() / r.Factor;
    }
    extension(Reader<Conductivity> r) {
        public double SiemensPerMeter => r.Value.ToSiemensPerMeter() / r.Factor;
        public double SiemensPerCentimeter => r.Value.ToSiemensPerCentimeter() / r.Factor;
    }
    extension(Reader<ElectricDipoleMoment> r) {
        public double CoulombMeters => r.Value.ToCoulombMeters() / r.Factor;
        public double Debyes => r.Value.ToDebyes() / r.Factor;
    }
    extension(Reader<Luminance> r) {
        public double CandelasPerSquareMeter => r.Value.ToCandelasPerSquareMeter() / r.Factor;
        public double Nits => r.Value.ToNits() / r.Factor;
        public double Stilbs => r.Value.ToStilbs() / r.Factor;
        public double FootLamberts => r.Value.ToFootLamberts() / r.Factor;
    }
    extension(Reader<LuminousEnergy> r) {
        public double LumenSeconds => r.Value.ToLumenSeconds() / r.Factor;
        public double LumenHours => r.Value.ToLumenHours() / r.Factor;
        public double Talbots => r.Value.ToTalbots() / r.Factor;
    }
    extension(Reader<LuminousExposure> r) {
        public double LuxSeconds => r.Value.ToLuxSeconds() / r.Factor;
        public double LuxHours => r.Value.ToLuxHours() / r.Factor;
    }
    extension(Reader<Radiance> r) {
        public double WattsPerSquareMeterSteradian => r.Value.ToWattsPerSquareMeterSteradian() / r.Factor;
    }
    extension(Reader<RadiantIntensity> r) {
        public double WattsPerSteradian => r.Value.ToWattsPerSteradian() / r.Factor;
    }
    extension(Reader<Exposure> r) {
        public double CoulombsPerKilogram => r.Value.ToCoulombsPerKilogram() / r.Factor;
        public double CoulombsPerGram => r.Value.ToCoulombsPerGram() / r.Factor;
        public double Roentgens => r.Value.ToRoentgens() / r.Factor;
    }
    extension(Reader<DoseRate> r) {
        public double GraysPerSecond => r.Value.ToGraysPerSecond() / r.Factor;
        public double GraysPerHour => r.Value.ToGraysPerHour() / r.Factor;
    }
    extension(Reader<Concentration> r) {
        public double MolesPerCubicMeter => r.Value.ToMolesPerCubicMeter() / r.Factor;
        public double MolesPerLiter => r.Value.ToMolesPerLiter() / r.Factor;
    }
    extension(Reader<CatalyticConcentration> r) {
        public double KatalsPerCubicMeter => r.Value.ToKatalsPerCubicMeter() / r.Factor;
        public double KatalsPerLiter => r.Value.ToKatalsPerLiter() / r.Factor;
    }
    extension(Reader<ReactionRate> r) {
        public double MolesPerCubicMeterSecond => r.Value.ToMolesPerCubicMeterSecond() / r.Factor;
        public double MolesPerLiterSecond => r.Value.ToMolesPerLiterSecond() / r.Factor;
    }
    extension(Reader<Ratio> r) {
        public double Ratio => r.Value.ToRatio() / r.Factor;
        public double Percent => r.Value.ToPercent() / r.Factor;
        public double PerMille => r.Value.ToPerMille() / r.Factor;
        public double PartsPerMillion => r.Value.ToPartsPerMillion() / r.Factor;
        public double PartsPerBillion => r.Value.ToPartsPerBillion() / r.Factor;
        public double PartsPerTrillion => r.Value.ToPartsPerTrillion() / r.Factor;
        public double Decibels => r.Value.ToDecibels() / r.Factor;
    }
}
