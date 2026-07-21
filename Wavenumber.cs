namespace com.hafthor.Measurement;

public class Wavenumber {
    private readonly double perMeter;

    private Wavenumber(double perMeter) => this.perMeter = perMeter;

    // Arithmetic
    public static Wavenumber operator +(Wavenumber a, Wavenumber b) => new(a.perMeter + b.perMeter);
    public static Wavenumber operator -(Wavenumber a, Wavenumber b) => new(a.perMeter - b.perMeter);
    public static Wavenumber operator -(Wavenumber x) => new(-x.perMeter);

    // Units
    public static Wavenumber FromPerMeter(double perMeter) => new(perMeter);
    public double ToPerMeter() => perMeter;
    public static Wavenumber FromPerCentimeter(double perCentimeter) => new(perCentimeter * (100));
    public double ToPerCentimeter() => perMeter / (100);

    // Reciprocal of wavelength
    public static Wavenumber FromWavelength(Length wavelength) => new(1 / wavelength.ToMeters());
    public Length ToWavelength() => Length.FromMeters(1 / perMeter);

    public override string ToString() => $"{perMeter} m⁻¹";

    public override bool Equals(object obj) => obj is Wavenumber other && other.perMeter == perMeter;
    public override int GetHashCode() => perMeter.GetHashCode();
}
