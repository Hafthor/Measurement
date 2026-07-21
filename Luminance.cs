namespace com.hafthor.Measurement;

public sealed class Luminance : Measurement<Luminance> {

    private Luminance(double value) : base(value) { }

    protected override Luminance Create(double value) => new(value);
    protected override string Symbol => "cd/m²";

    // Units
    public static Luminance FromCandelasPerSquareMeter(double value) => new(value);
    public double ToCandelasPerSquareMeter() => value;
    public static Luminance FromNits(double nits) => new(nits);
    public double ToNits() => value;
    public static Luminance FromStilbs(double stilbs) => new(stilbs * (1e4));
    public double ToStilbs() => value / (1e4);
    public static Luminance FromFootLamberts(double footLamberts) => new(footLamberts * (3.4262590996));
    public double ToFootLamberts() => value / (3.4262590996);

    // Composite relationships
    public static LuminousIntensity operator *(Luminance luminance, Area area) => LuminousIntensity.FromCandelas(luminance.ToCandelasPerSquareMeter() * area.ToSquareMeters());
    public static LuminousIntensity operator *(Area area, Luminance luminance) => LuminousIntensity.FromCandelas(area.ToSquareMeters() * luminance.ToCandelasPerSquareMeter());

}
