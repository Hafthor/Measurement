namespace com.hafthor.Measurement;

public class SurfaceTension {
    private readonly double newtonsPerMeter;

    private SurfaceTension(double newtonsPerMeter) => this.newtonsPerMeter = newtonsPerMeter;

    // Arithmetic
    public static SurfaceTension operator +(SurfaceTension a, SurfaceTension b) => new(a.newtonsPerMeter + b.newtonsPerMeter);
    public static SurfaceTension operator -(SurfaceTension a, SurfaceTension b) => new(a.newtonsPerMeter - b.newtonsPerMeter);
    public static SurfaceTension operator -(SurfaceTension x) => new(-x.newtonsPerMeter);

    // Units
    public static SurfaceTension FromNewtonsPerMeter(double newtonsPerMeter) => new(newtonsPerMeter);
    public double ToNewtonsPerMeter() => newtonsPerMeter;
    public static SurfaceTension FromMillinewtonsPerMeter(double millinewtonsPerMeter) => new(millinewtonsPerMeter * (1e-3));
    public double ToMillinewtonsPerMeter() => newtonsPerMeter / (1e-3);
    public static SurfaceTension FromDynesPerCentimeter(double dynesPerCentimeter) => new(dynesPerCentimeter * (1e-3));
    public double ToDynesPerCentimeter() => newtonsPerMeter / (1e-3);

    // Composite relationships
    public static Force operator *(SurfaceTension surfaceTension, Length length) => Force.FromNewtons(surfaceTension.ToNewtonsPerMeter() * length.ToMeters());
    public static Force operator *(Length length, SurfaceTension surfaceTension) => Force.FromNewtons(length.ToMeters() * surfaceTension.ToNewtonsPerMeter());

    public override string ToString() => $"{newtonsPerMeter} N/m";
}
