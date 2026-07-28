namespace com.hafthor.Measurement;

[Measurement("N/m", VariableName = "millinewtonsPerMeter", DisplayFactor = 1e3)]
public readonly partial struct SurfaceTension {

    // Units
    public static SurfaceTension FromNewtonsPerMeter(double newtonsPerMeter) => new(newtonsPerMeter * 1e3);
    public double ToNewtonsPerMeter() => millinewtonsPerMeter / 1e3;
    public static SurfaceTension FromMillinewtonsPerMeter(double millinewtonsPerMeter) => new(millinewtonsPerMeter);
    public double ToMillinewtonsPerMeter() => millinewtonsPerMeter;
    public static SurfaceTension FromDynesPerCentimeter(double dynesPerCentimeter) => new(dynesPerCentimeter);
    public double ToDynesPerCentimeter() => millinewtonsPerMeter;

    // Composite relationships
    public static Force operator *(SurfaceTension surfaceTension, Length length) => Force.FromNewtons(surfaceTension.ToNewtonsPerMeter() * length.ToMeters());
    public static Force operator *(Length length, SurfaceTension surfaceTension) => Force.FromNewtons(length.ToMeters() * surfaceTension.ToNewtonsPerMeter());

}
