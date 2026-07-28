namespace com.hafthor.Measurement;

[Measurement("m²", VariableName = "squareMillimeters", DisplayFactor = 1e6)]
public readonly partial struct Area {
    // SI units
    public static Area FromSquareKilometers(double squareKilometers) => new(squareKilometers * 1e12);
    public double ToSquareKilometers() => squareMillimeters / 1e12;
    public static Area FromHectares(double hectares) => new(hectares * 1e10);
    public double ToHectares() => squareMillimeters / 1e10;
    public static Area FromAres(double ares) => new(ares * 1e8);
    public double ToAres() => squareMillimeters / 1e8;
    public static Area FromSquareMeters(double squareMeters) => new(squareMeters * 1e6);
    public double ToSquareMeters() => squareMillimeters / 1e6;
    public static Area FromSquareCentimeters(double squareCentimeters) => new(squareCentimeters * 1e2);
    public double ToSquareCentimeters() => squareMillimeters / 1e2;
    public static Area FromSquareMillimeters(double squareMillimeters) => new(squareMillimeters);
    public double ToSquareMillimeters() => squareMillimeters;

    // Imperial / US units
    public static Area FromSquareMiles(double squareMiles) => new(squareMiles * 2589988.110336e6);
    public double ToSquareMiles() => squareMillimeters / 2589988.110336e6;
    public static Area FromAcres(double acres) => new(acres * 4046.8564224e6);
    public double ToAcres() => squareMillimeters / 4046.8564224e6;
    public static Area FromSquareYards(double squareYards) => new(squareYards * 0.83612736e6);
    public double ToSquareYards() => squareMillimeters / 0.83612736e6;
    public static Area FromSquareFeet(double squareFeet) => new(squareFeet * 0.09290304e6);
    public double ToSquareFeet() => squareMillimeters / 0.09290304e6;
    public static Area FromSquareInches(double squareInches) => new(squareInches * 0.00064516e6);
    public double ToSquareInches() => squareMillimeters / 0.00064516e6;

    // Scientific units
    public static Area FromBarns(double barns) => new(barns * 1e-22);
    public double ToBarns() => squareMillimeters / 1e-22;

    // Composite relationships
    public static Length operator /(Area area, Length length) => Length.FromMeters(area.ToSquareMeters() / length.ToMeters());
    public static Volume operator *(Area area, Length length) => Volume.FromCubicMeters(area.ToSquareMeters() * length.ToMeters());
    public static Force operator *(Area area, Pressure pressure) => Force.FromNewtons(area.ToSquareMeters() * pressure.ToPascals());

    // Composite relationships (derived)
    public static MomentOfInertia operator *(Area area, Mass mass) => MomentOfInertia.FromKilogramSquareMeters(area.ToSquareMeters() * mass.ToKilograms());
    public static KinematicViscosity operator /(Area area, Duration duration) => KinematicViscosity.FromSquareMetersPerSecond(area.ToSquareMeters() / duration.ToSeconds());
}
