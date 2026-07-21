namespace com.hafthor.Measurement;

[Measurement("m⁻¹")]
public readonly partial struct Wavenumber {

    // Units
    public static Wavenumber FromPerMeter(double perMeter) => new(perMeter);
    public double ToPerMeter() => value;
    public static Wavenumber FromPerCentimeter(double perCentimeter) => new(perCentimeter * (100));
    public double ToPerCentimeter() => value / (100);

    // Reciprocal of wavelength
    public static Wavenumber FromWavelength(Length wavelength) => new(1 / wavelength.ToMeters());
    public Length ToWavelength() => Length.FromMeters(1 / value);

}
