namespace com.hafthor.Measurement;

public sealed class LuminousExposure : Measurement<LuminousExposure> {

    private LuminousExposure(double value) : base(value) { }

    protected override LuminousExposure Create(double value) => new(value);
    protected override string Symbol => "lx·s";

    // Units
    public static LuminousExposure FromLuxSeconds(double value) => new(value);
    public double ToLuxSeconds() => value;
    public static LuminousExposure FromLuxHours(double luxHours) => new(luxHours * (3600));
    public double ToLuxHours() => value / (3600);

    // Composite relationships
    public static Illuminance operator /(LuminousExposure luminousExposure, Duration duration) => Illuminance.FromLux(luminousExposure.ToLuxSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousExposure luminousExposure, Illuminance illuminance) => Duration.FromSeconds(luminousExposure.ToLuxSeconds() / illuminance.ToLux());

}
