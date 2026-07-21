namespace com.hafthor.Measurement;

public sealed class SolidAngle : Measurement<SolidAngle> {

    private SolidAngle(double value) : base(value) { }

    protected override SolidAngle Create(double value) => new(value);
    protected override string Symbol => "sr";

    public static SolidAngle FromSteradians(double value) => new(value);
    public double ToSteradians() => value;
    public static SolidAngle FromSpats(double spats) => new(spats * 4 * Math.PI);
    public double ToSpats() => value / (4 * Math.PI);
    public static SolidAngle FromSquareDegrees(double squareDegrees) => new(squareDegrees * Math.PI * Math.PI / 32400);
    public double ToSquareDegrees() => value * 32400 / (Math.PI * Math.PI);

    // Composite relationships
    public static LuminousFlux operator *(SolidAngle solidAngle, LuminousIntensity intensity) => LuminousFlux.FromLumens(solidAngle.value * intensity.ToCandelas());

}
