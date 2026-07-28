namespace com.hafthor.Measurement;

[Measurement("sr", VariableName = "steradians")]
public readonly partial struct SolidAngle {
    public static SolidAngle FromSteradians(double steradians) => new(steradians);
    public double ToSteradians() => steradians;
    public static SolidAngle FromSpats(double spats) => new(spats * 4 * Math.PI);
    public double ToSpats() => steradians / (4 * Math.PI);
    public static SolidAngle FromSquareDegrees(double squareDegrees) => new(squareDegrees * Math.PI * Math.PI / 32400);
    public double ToSquareDegrees() => steradians * 32400 / (Math.PI * Math.PI);

    // Composite relationships
    public static LuminousFlux operator *(SolidAngle solidAngle, LuminousIntensity intensity) => LuminousFlux.FromLumens(solidAngle.ToSteradians() * intensity.ToCandelas());
}
