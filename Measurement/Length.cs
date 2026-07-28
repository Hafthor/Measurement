namespace com.hafthor.Measurement;

[Measurement("m", VariableName = "nanometers", DisplayFactor = 1e9)]
public readonly partial struct Length {
    // Canonical (stored) unit is the nanometre, so nm/µm/mm-scale values land on exact
    // integers in IEEE-754; ToString presents metres (DisplayFactor = 1e9).
    public static Length FromKilometers(double kilometers) => new(kilometers * 1e12);
    public double ToKilometers() => nanometers / 1e12;
    public static Length FromMeters(double meters) => new(meters * 1e9);
    public double ToMeters() => nanometers / 1e9;
    public static Length FromCentimeters(double centimeters) => new(centimeters * 1e7);
    public double ToCentimeters() => nanometers / 1e7;
    public static Length FromMillimeters(double millimeters) => new(millimeters * 1e6);
    public double ToMillimeters() => nanometers / 1e6;
    public static Length FromMicrometers(double micrometers) => new(micrometers * 1e3);
    public double ToMicrometers() => nanometers / 1e3;
    public static Length FromNanometers(double nanometers) => new(nanometers);
    public double ToNanometers() => nanometers;
    public static Length FromAngstroms(double angstroms) => new(angstroms * 1e-1);
    public double ToAngstroms() => nanometers / 1e-1;

    // Imperial units
    public static Length FromLeagues(double leagues) => new(leagues * 4828.032e9);
    public double ToLeagues() => nanometers / 4828.032e9;
    public static Length FromNauticalMiles(double nauticalMiles) => new(nauticalMiles * 1852e9);
    public double ToNauticalMiles() => nanometers / 1852e9;
    public static Length FromMiles(double miles) => new(miles * 1609.344e9);
    public double ToMiles() => nanometers / 1609.344e9;
    public static Length FromFurlongs(double furlongs) => new(furlongs * 201.168e9);
    public double ToFurlongs() => nanometers / 201.168e9;
    public static Length FromChains(double chains) => new(chains * 20.1168e9);
    public double ToChains() => nanometers / 20.1168e9;
    public static Length FromRods(double rods) => new(rods * 5.0292e9);
    public double ToRods() => nanometers / 5.0292e9;
    public static Length FromFathoms(double fathoms) => new(fathoms * 1.8288e9);
    public double ToFathoms() => nanometers / 1.8288e9;
    public static Length FromYards(double yards) => new(yards * 0.9144e9);
    public double ToYards() => nanometers / 0.9144e9;
    public static Length FromFeet(double feet) => new(feet * 0.3048e9);
    public double ToFeet() => nanometers / 0.3048e9;
    public static Length FromInches(double inches) => new(inches * 0.0254e9);
    public double ToInches() => nanometers / 0.0254e9;

    // Astronomical units
    public static Length FromHubbleLengths(double hubbleLengths) => new(hubbleLengths * 1.4400000000000002e35);
    public double ToHubbleLengths() => nanometers / 1.4400000000000002e35;
    public static Length FromParsecs(double parsecs) => new(parsecs * 3.0856775814913673e25);
    public double ToParsecs() => nanometers / 3.0856775814913673e25;
    public static Length FromLightYears(double lightYears) => new(lightYears * 9_460_730_472_580_800e9);
    public double ToLightYears() => nanometers / 9_460_730_472_580_800e9;
    public static Length FromAstronomicalUnits(double astronomicalUnits) => new(astronomicalUnits * 149_597_870_700e9);
    public double ToAstronomicalUnits() => nanometers / 149_597_870_700e9;

    // Scientific units
    public static Length FromPlanckLengths(double planckLengths) => new(planckLengths * 1.616255e-26);
    public double ToPlanckLengths() => nanometers / 1.616255e-26;

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
