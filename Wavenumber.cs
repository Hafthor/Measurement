namespace com.hafthor.Measurement;

public sealed class Wavenumber : Measurement<Wavenumber> {

    private Wavenumber(double value) : base(value) { }

    protected override Wavenumber Create(double value) => new(value);
    protected override string Symbol => "m⁻¹";

    // Units
    public static Wavenumber FromPerMeter(double value) => new(value);
    public double ToPerMeter() => value;
    public static Wavenumber FromPerCentimeter(double perCentimeter) => new(perCentimeter * (100));
    public double ToPerCentimeter() => value / (100);

    // Reciprocal of wavelength
    public static Wavenumber FromWavelength(Length wavelength) => new(1 / wavelength.ToMeters());
    public Length ToWavelength() => Length.FromMeters(1 / value);

}
