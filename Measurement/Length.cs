namespace com.hafthor.Measurement;

[Measurement("m", VariableName = "nanometers", DisplayFactor = 1e9)]
[SiUnit("Meters", 9, "None Kilo Centi Milli Micro Nano")]
[SiUnit("Angstroms", -1)]
[Unit("Leagues", 4828.032e9)]
[Unit("NauticalMiles", 1852e9)]
[Unit("Miles", 1609.344e9)]
[Unit("Furlongs", 201.168e9)]
[Unit("Chains", 20.1168e9)]
[Unit("Rods", 5.0292e9)]
[Unit("Fathoms", 1.8288e9)]
[Unit("Yards", 0.9144e9)]
[Unit("Feet", 0.3048e9)]
[Unit("Inches", 0.0254e9)]
[Unit("HubbleLengths", 1.4400000000000002e35)]
[Unit("Parsecs", 3.0856775814913673e25)]
[Unit("LightYears", 9_460_730_472_580_800e9)]
[Unit("AstronomicalUnits", 149_597_870_700e9)]
[Unit("PlanckLengths", 1.616255e-26)]
public readonly partial struct Length {
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
