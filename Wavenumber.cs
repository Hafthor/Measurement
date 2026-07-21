namespace com.hafthor.Measurement;

public class Wavenumber {
    private readonly double perMeter;

    private Wavenumber(double perMeter) => this.perMeter = perMeter;

    // Arithmetic
    public static Wavenumber operator +(Wavenumber a, Wavenumber b) => new Wavenumber(a.perMeter + b.perMeter);
    public static Wavenumber operator -(Wavenumber a, Wavenumber b) => new Wavenumber(a.perMeter - b.perMeter);
    public static Wavenumber operator -(Wavenumber x) => new Wavenumber(-x.perMeter);

    // Units
    public static Wavenumber FromPerMeter(double perMeter) => new Wavenumber(perMeter);
    public double ToPerMeter() => perMeter;
    public static Wavenumber FromPerCentimeter(double perCentimeter) => new Wavenumber(perCentimeter * (100));
    public double ToPerCentimeter() => perMeter / (100);

    // Reciprocal of wavelength
    public static Wavenumber FromWavelength(Length wavelength) => new Wavenumber(1 / wavelength.ToMeters());
    public Length ToWavelength() => Length.FromMeters(1 / perMeter);
}
