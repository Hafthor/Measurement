namespace com.hafthor.Measurement;

[Measurement("m")]
public readonly partial struct Length {

    // SI units
    public static Length FromKilometers(double kilometers) => new(kilometers * 1e3);
    public double ToKilometers() => value / 1e3;
    public static Length FromMeters(double meters) => new(meters);
    public double ToMeters() => value;
    public static Length FromCentimeters(double centimeters) => new(centimeters * 1e-2);
    public double ToCentimeters() => value / 1e-2;
    public static Length FromMillimeters(double millimeters) => new(millimeters * 1e-3);
    public double ToMillimeters() => value / 1e-3;
    public static Length FromMicrometers(double micrometers) => new(micrometers * 1e-6);
    public double ToMicrometers() => value / 1e-6;
    public static Length FromNanometers(double nanometers) => new(nanometers * 1e-9);
    public double ToNanometers() => value / 1e-9;
    public static Length FromAngstroms(double angstroms) => new(angstroms * 1e-10);
    public double ToAngstroms() => value / 1e-10;

    // Imperial units
    public static Length FromLeagues(double leagues) => new(leagues * 4828.032);
    public double ToLeagues() => value / 4828.032;
    public static Length FromNauticalMiles(double nauticalMiles) => new(nauticalMiles * 1852);
    public double ToNauticalMiles() => value / 1852;
    public static Length FromMiles(double miles) => new(miles * 1609.344);
    public double ToMiles() => value / 1609.344;
    public static Length FromFurlongs(double furlongs) => new(furlongs * 201.168);
    public double ToFurlongs() => value / 201.168;
    public static Length FromChains(double chains) => new(chains * 20.1168);
    public double ToChains() => value / 20.1168;
    public static Length FromRods(double rods) => new(rods * 5.0292);
    public double ToRods() => value / 5.0292;
    public static Length FromFathoms(double fathoms) => new(fathoms * 1.8288);
    public double ToFathoms() => value / 1.8288;
    public static Length FromYards(double yards) => new(yards * 0.9144);
    public double ToYards() => value / 0.9144;
    public static Length FromFeet(double feet) => new(feet * 0.3048);
    public double ToFeet() => value / 0.3048;
    public static Length FromInches(double inches) => new(inches * 0.0254);
    public double ToInches() => value / 0.0254;

    // Astronomical units
    public static Length FromHubbleLengths(double hubbleLengths) => new(hubbleLengths * 1.4400000000000002e26);
    public double ToHubbleLengths() => value / 1.4400000000000002e26;
    public static Length FromParsecs(double parsecs) => new(parsecs * 3.0856775814913673e16);
    public double ToParsecs() => value / 3.0856775814913673e16;
    public static Length FromLightYears(double lightYears) => new(lightYears * 9_460_730_472_580_800);
    public double ToLightYears() => value / 9_460_730_472_580_800;
    public static Length FromAstronomicalUnits(double astronomicalUnits) => new(astronomicalUnits * 149_597_870_700);
    public double ToAstronomicalUnits() => value / 149_597_870_700;

    // Scientific units
    public static Length FromPlanckLengths(double planckLengths) => new(planckLengths * 1.616255e-35);
    public double ToPlanckLengths() => value / 1.616255e-35;

    // Composite relationships
    public static Speed operator /(Length length, Duration duration) => Speed.FromMetersPerSecond(length.value / duration.ToSeconds());
    public static Area operator *(Length a, Length b) => Area.FromSquareMeters(a.value * b.value);
    public static Volume operator *(Length length, Area area) => Volume.FromCubicMeters(length.value * area.ToSquareMeters());
    public static Energy operator *(Length length, Force force) => Energy.FromJoules(length.value * force.ToNewtons());

    // Composite relationships (derived)
    public static Resistivity operator *(Length length, ElectricResistance electricResistance) => Resistivity.FromOhmMeters(length.ToMeters() * electricResistance.ToOhms());
    public static ElectricDipoleMoment operator *(Length length, ElectricCharge electricCharge) => ElectricDipoleMoment.FromCoulombMeters(length.ToMeters() * electricCharge.ToCoulombs());

    // Famous relations
    public static Speed operator *(Length length, Frequency frequency) => Speed.FromMetersPerSecond(length.ToMeters() * frequency.ToHertz());

}
