namespace com.hafthor.Measurement;

[Measurement("sr")]
public readonly partial struct SolidAngle {

    public static SolidAngle FromSteradians(double steradians) => new(steradians);
    public double ToSteradians() => value;
    public static SolidAngle FromSpats(double spats) => new(spats * 4 * Math.PI);
    public double ToSpats() => value / (4 * Math.PI);
    public static SolidAngle FromSquareDegrees(double squareDegrees) => new(squareDegrees * Math.PI * Math.PI / 32400);
    public double ToSquareDegrees() => value * 32400 / (Math.PI * Math.PI);

    // Composite relationships
    public static LuminousFlux operator *(SolidAngle solidAngle, LuminousIntensity intensity) => LuminousFlux.FromLumens(solidAngle.value * intensity.ToCandelas());

}
