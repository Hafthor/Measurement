namespace com.hafthor.Measurement;

public sealed class Radiance : Measurement<Radiance> {

    private Radiance(double value) : base(value) { }

    protected override Radiance Create(double value) => new(value);
    protected override string Symbol => "W/(m²·sr)";

    // Units
    public static Radiance FromWattsPerSquareMeterSteradian(double value) => new(value);
    public double ToWattsPerSquareMeterSteradian() => value;

}
