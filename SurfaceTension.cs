namespace com.hafthor.Measurement;

public sealed class SurfaceTension : Measurement<SurfaceTension> {

    private SurfaceTension(double value) : base(value) { }

    protected override SurfaceTension Create(double value) => new(value);
    protected override string Symbol => "N/m";

    // Units
    public static SurfaceTension FromNewtonsPerMeter(double value) => new(value);
    public double ToNewtonsPerMeter() => value;
    public static SurfaceTension FromMillinewtonsPerMeter(double millinewtonsPerMeter) => new(millinewtonsPerMeter * (1e-3));
    public double ToMillinewtonsPerMeter() => value / (1e-3);
    public static SurfaceTension FromDynesPerCentimeter(double dynesPerCentimeter) => new(dynesPerCentimeter * (1e-3));
    public double ToDynesPerCentimeter() => value / (1e-3);

    // Composite relationships
    public static Force operator *(SurfaceTension surfaceTension, Length length) => Force.FromNewtons(surfaceTension.ToNewtonsPerMeter() * length.ToMeters());
    public static Force operator *(Length length, SurfaceTension surfaceTension) => Force.FromNewtons(length.ToMeters() * surfaceTension.ToNewtonsPerMeter());

}
