namespace com.hafthor.Measurement;

[Measurement("m²", VariableName = "squareMillimeters", DisplayFactor = 1e6)]
[SiUnit("SquareKilometers", 12)]
[SiUnit("Hectares", 10)]
[SiUnit("Ares", 8)]
[SiUnit("SquareMeters", 6)]
[SiUnit("SquareCentimeters", 2)]
[SiUnit("SquareMillimeters", 0)]
[Unit("SquareMiles", 2589988.110336e6)]
[Unit("Acres", 4046.8564224e6)]
[Unit("SquareYards", 0.83612736e6)]
[Unit("SquareFeet", 0.09290304e6)]
[Unit("SquareInches", 0.00064516e6)]
[SiUnit("Barns", -22)]
public readonly partial struct Area {
    // Composite relationships
    public static Length operator /(Area area, Length length) => Length.FromMeters(area.ToSquareMeters() / length.ToMeters());
    public static Volume operator *(Area area, Length length) => Volume.FromCubicMeters(area.ToSquareMeters() * length.ToMeters());
    public static Force operator *(Area area, Pressure pressure) => Force.FromNewtons(area.ToSquareMeters() * pressure.ToPascals());

    // Composite relationships (derived)
    public static MomentOfInertia operator *(Area area, Mass mass) => MomentOfInertia.FromKilogramSquareMeters(area.ToSquareMeters() * mass.ToKilograms());
    public static KinematicViscosity operator /(Area area, Duration duration) => KinematicViscosity.FromSquareMetersPerSecond(area.ToSquareMeters() / duration.ToSeconds());
}
