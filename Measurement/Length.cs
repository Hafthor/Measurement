namespace com.hafthor.Measurement;

[Measurement("m", DisplayFactor = 1e9)]
public readonly partial struct Length {

    // Canonical (stored) unit is the nanometre, so nm/µm/mm-scale values land on exact
    // integers in IEEE-754; ToString presents metres (DisplayFactor = 1e9).
    public static Length FromKilometers(double kilometers) => new(kilometers * 1e12);
    public double ToKilometers() => value / 1e12;
    public static Length FromMeters(double meters) => new(meters * 1e9);
    public double ToMeters() => value / 1e9;
    public static Length FromCentimeters(double centimeters) => new(centimeters * 1e7);
    public double ToCentimeters() => value / 1e7;
    public static Length FromMillimeters(double millimeters) => new(millimeters * 1e6);
    public double ToMillimeters() => value / 1e6;
    public static Length FromMicrometers(double micrometers) => new(micrometers * 1e3);
    public double ToMicrometers() => value / 1e3;
    public static Length FromNanometers(double nanometers) => new(nanometers);
    public double ToNanometers() => value;
    public static Length FromAngstroms(double angstroms) => new(angstroms * 1e-1);
    public double ToAngstroms() => value / 1e-1;

    // Imperial units
    public static Length FromLeagues(double leagues) => new(leagues * 4828.032e9);
    public double ToLeagues() => value / 4828.032e9;
    public static Length FromNauticalMiles(double nauticalMiles) => new(nauticalMiles * 1852e9);
    public double ToNauticalMiles() => value / 1852e9;
    public static Length FromMiles(double miles) => new(miles * 1609.344e9);
    public double ToMiles() => value / 1609.344e9;
    public static Length FromFurlongs(double furlongs) => new(furlongs * 201.168e9);
    public double ToFurlongs() => value / 201.168e9;
    public static Length FromChains(double chains) => new(chains * 20.1168e9);
    public double ToChains() => value / 20.1168e9;
    public static Length FromRods(double rods) => new(rods * 5.0292e9);
    public double ToRods() => value / 5.0292e9;
    public static Length FromFathoms(double fathoms) => new(fathoms * 1.8288e9);
    public double ToFathoms() => value / 1.8288e9;
    public static Length FromYards(double yards) => new(yards * 0.9144e9);
    public double ToYards() => value / 0.9144e9;
    public static Length FromFeet(double feet) => new(feet * 0.3048e9);
    public double ToFeet() => value / 0.3048e9;
    public static Length FromInches(double inches) => new(inches * 0.0254e9);
    public double ToInches() => value / 0.0254e9;

    // Astronomical units
    public static Length FromHubbleLengths(double hubbleLengths) => new(hubbleLengths * 1.4400000000000002e35);
    public double ToHubbleLengths() => value / 1.4400000000000002e35;
    public static Length FromParsecs(double parsecs) => new(parsecs * 3.0856775814913673e25);
    public double ToParsecs() => value / 3.0856775814913673e25;
    public static Length FromLightYears(double lightYears) => new(lightYears * 9_460_730_472_580_800e9);
    public double ToLightYears() => value / 9_460_730_472_580_800e9;
    public static Length FromAstronomicalUnits(double astronomicalUnits) => new(astronomicalUnits * 149_597_870_700e9);
    public double ToAstronomicalUnits() => value / 149_597_870_700e9;

    // Scientific units
    public static Length FromPlanckLengths(double planckLengths) => new(planckLengths * 1.616255e-26);
    public double ToPlanckLengths() => value / 1.616255e-26;

    // Composite relationships
    public static Speed operator /(Length length, Duration duration) => Speed.FromMetersPerSecond(length.ToMeters() / duration.ToSeconds());
    public static Area operator *(Length a, Length b) => Area.FromSquareMeters(a.ToMeters() * b.ToMeters());
    public static Volume operator *(Length length, Area area) => Volume.FromCubicMeters(length.ToMeters() * area.ToSquareMeters());
    public static Energy operator *(Length length, Force force) => Energy.FromJoules(length.ToMeters() * force.ToNewtons());

    // Composite relationships (derived)
    public static Resistivity operator *(Length length, ElectricResistance electricResistance) => Resistivity.FromOhmMeters(length.ToMeters() * electricResistance.ToOhms());
    public static ElectricDipoleMoment operator *(Length length, ElectricCharge electricCharge) => ElectricDipoleMoment.FromCoulombMeters(length.ToMeters() * electricCharge.ToCoulombs());

    // Famous relations
    public static Speed operator *(Length length, Frequency frequency) => Speed.FromMetersPerSecond(length.ToMeters() * frequency.ToHertz());

}
