namespace com.hafthor.Measurement;

public class Area {
    private readonly double squareMeters;

    private Area(double squareMeters) => this.squareMeters = squareMeters;

    // Arithmetic
    public static Area operator +(Area a, Area b) => new(a.squareMeters + b.squareMeters);
    public static Area operator -(Area a, Area b) => new(a.squareMeters - b.squareMeters);
    public static Area operator -(Area x) => new(-x.squareMeters);

    // SI units
    public static Area FromSquareKilometers(double squareKilometers) => new(squareKilometers * 1e6);
    public double ToSquareKilometers() => squareMeters / 1e6;
    public static Area FromHectares(double hectares) => new(hectares * 1e4);
    public double ToHectares() => squareMeters / 1e4;
    public static Area FromAres(double ares) => new(ares * 1e2);
    public double ToAres() => squareMeters / 1e2;
    public static Area FromSquareMeters(double squareMeters) => new(squareMeters);
    public double ToSquareMeters() => squareMeters;
    public static Area FromSquareCentimeters(double squareCentimeters) => new(squareCentimeters * 1e-4);
    public double ToSquareCentimeters() => squareMeters / 1e-4;
    public static Area FromSquareMillimeters(double squareMillimeters) => new(squareMillimeters * 1e-6);
    public double ToSquareMillimeters() => squareMeters / 1e-6;

    // Imperial / US units
    public static Area FromSquareMiles(double squareMiles) => new(squareMiles * 2589988.110336);
    public double ToSquareMiles() => squareMeters / 2589988.110336;
    public static Area FromAcres(double acres) => new(acres * 4046.8564224);
    public double ToAcres() => squareMeters / 4046.8564224;
    public static Area FromSquareYards(double squareYards) => new(squareYards * 0.83612736);
    public double ToSquareYards() => squareMeters / 0.83612736;
    public static Area FromSquareFeet(double squareFeet) => new(squareFeet * 0.09290304);
    public double ToSquareFeet() => squareMeters / 0.09290304;
    public static Area FromSquareInches(double squareInches) => new(squareInches * 0.00064516);
    public double ToSquareInches() => squareMeters / 0.00064516;

    // Scientific units
    public static Area FromBarns(double barns) => new(barns * 1e-28);
    public double ToBarns() => squareMeters / 1e-28;

    // Composite relationships
    public static Length operator /(Area area, Length length) => Length.FromMeters(area.squareMeters / length.ToMeters());
    public static Volume operator *(Area area, Length length) => Volume.FromCubicMeters(area.squareMeters * length.ToMeters());
    public static Force operator *(Area area, Pressure pressure) => Force.FromNewtons(area.squareMeters * pressure.ToPascals());

    // Composite relationships (derived)
    public static MomentOfInertia operator *(Area area, Mass mass) => MomentOfInertia.FromKilogramSquareMeters(area.ToSquareMeters() * mass.ToKilograms());
    public static KinematicViscosity operator /(Area area, Duration duration) => KinematicViscosity.FromSquareMetersPerSecond(area.ToSquareMeters() / duration.ToSeconds());
}
