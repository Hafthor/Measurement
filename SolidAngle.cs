namespace com.hafthor.Measurement;

public class SolidAngle {
    private readonly double steradians;

    private SolidAngle(double steradians) => this.steradians = steradians;

    // Arithmetic
    public static SolidAngle operator +(SolidAngle a, SolidAngle b) => new SolidAngle(a.steradians + b.steradians);
    public static SolidAngle operator -(SolidAngle a, SolidAngle b) => new SolidAngle(a.steradians - b.steradians);
    public static SolidAngle operator -(SolidAngle x) => new SolidAngle(-x.steradians);

    public static SolidAngle FromSteradians(double steradians) => new SolidAngle(steradians);
    public double ToSteradians() => steradians;
    public static SolidAngle FromSpats(double spats) => new SolidAngle(spats * 4 * Math.PI);
    public double ToSpats() => steradians / (4 * Math.PI);
    public static SolidAngle FromSquareDegrees(double squareDegrees) => new SolidAngle(squareDegrees * Math.PI * Math.PI / 32400);
    public double ToSquareDegrees() => steradians * 32400 / (Math.PI * Math.PI);

    // Composite relationships
    public static LuminousFlux operator *(SolidAngle solidAngle, LuminousIntensity intensity) => LuminousFlux.FromLumens(solidAngle.steradians * intensity.ToCandelas());
}
