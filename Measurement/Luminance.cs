namespace com.hafthor.Measurement;

[Measurement("cd/m²", VariableName = "nits")]
public readonly partial struct Luminance {
    // Units
    public static Luminance FromCandelasPerSquareMeter(double candelasPerSquareMeter) => new(candelasPerSquareMeter);
    public double ToCandelasPerSquareMeter() => nits;
    public static Luminance FromNits(double nits) => new(nits);
    public double ToNits() => nits;
    public static Luminance FromStilbs(double stilbs) => new(stilbs * (1e4));
    public double ToStilbs() => nits / (1e4);
    public static Luminance FromFootLamberts(double footLamberts) => new(footLamberts * (3.4262590996));
    public double ToFootLamberts() => nits / (3.4262590996);

    // Composite relationships
    public static LuminousIntensity operator *(Luminance luminance, Area area) => LuminousIntensity.FromCandelas(luminance.ToCandelasPerSquareMeter() * area.ToSquareMeters());
    public static LuminousIntensity operator *(Area area, Luminance luminance) => LuminousIntensity.FromCandelas(area.ToSquareMeters() * luminance.ToCandelasPerSquareMeter());
}
