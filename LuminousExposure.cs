namespace com.hafthor.Measurement;

[Measurement("lx·s")]
public readonly partial struct LuminousExposure {

    // Units
    public static LuminousExposure FromLuxSeconds(double luxSeconds) => new(luxSeconds);
    public double ToLuxSeconds() => value;
    public static LuminousExposure FromLuxHours(double luxHours) => new(luxHours * (3600));
    public double ToLuxHours() => value / (3600);

    // Composite relationships
    public static Illuminance operator /(LuminousExposure luminousExposure, Duration duration) => Illuminance.FromLux(luminousExposure.ToLuxSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousExposure luminousExposure, Illuminance illuminance) => Duration.FromSeconds(luminousExposure.ToLuxSeconds() / illuminance.ToLux());

}
