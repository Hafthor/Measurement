namespace com.hafthor.Measurement;

public class Luminance {
    private readonly double candelasPerSquareMeter;

    private Luminance(double candelasPerSquareMeter) => this.candelasPerSquareMeter = candelasPerSquareMeter;

    // Arithmetic
    public static Luminance operator +(Luminance a, Luminance b) => new Luminance(a.candelasPerSquareMeter + b.candelasPerSquareMeter);
    public static Luminance operator -(Luminance a, Luminance b) => new Luminance(a.candelasPerSquareMeter - b.candelasPerSquareMeter);
    public static Luminance operator -(Luminance x) => new Luminance(-x.candelasPerSquareMeter);

    // Units
    public static Luminance FromCandelasPerSquareMeter(double candelasPerSquareMeter) => new Luminance(candelasPerSquareMeter);
    public double ToCandelasPerSquareMeter() => candelasPerSquareMeter;
    public static Luminance FromNits(double nits) => new Luminance(nits);
    public double ToNits() => candelasPerSquareMeter;
    public static Luminance FromStilbs(double stilbs) => new Luminance(stilbs * (1e4));
    public double ToStilbs() => candelasPerSquareMeter / (1e4);
    public static Luminance FromFootLamberts(double footLamberts) => new Luminance(footLamberts * (3.4262590996));
    public double ToFootLamberts() => candelasPerSquareMeter / (3.4262590996);

    // Composite relationships
    public static LuminousIntensity operator *(Luminance luminance, Area area) => LuminousIntensity.FromCandelas(luminance.ToCandelasPerSquareMeter() * area.ToSquareMeters());
    public static LuminousIntensity operator *(Area area, Luminance luminance) => LuminousIntensity.FromCandelas(area.ToSquareMeters() * luminance.ToCandelasPerSquareMeter());
}
