namespace com.hafthor.Measurement;

public sealed class AreaDensity : Measurement<AreaDensity> {

    private AreaDensity(double value) : base(value) { }

    protected override AreaDensity Create(double value) => new(value);
    protected override string Symbol => "kg/m²";

    // Units
    public static AreaDensity FromKilogramsPerSquareMeter(double value) => new(value);
    public double ToKilogramsPerSquareMeter() => value;
    public static AreaDensity FromGramsPerSquareMeter(double gramsPerSquareMeter) => new(gramsPerSquareMeter * (1e-3));
    public double ToGramsPerSquareMeter() => value / (1e-3);

    // Composite relationships
    public static Mass operator *(AreaDensity areaDensity, Area area) => Mass.FromKilograms(areaDensity.ToKilogramsPerSquareMeter() * area.ToSquareMeters());
    public static Mass operator *(Area area, AreaDensity areaDensity) => Mass.FromKilograms(area.ToSquareMeters() * areaDensity.ToKilogramsPerSquareMeter());

}
