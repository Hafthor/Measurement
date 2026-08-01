namespace com.hafthor.Measurement;

[Measurement("lx·s", VariableName = "luxSeconds")]
[SiUnit("LuxSeconds", 0)]
[Unit("LuxHours", 3600)]
public readonly partial struct LuminousExposure {
    // Composite relationships
    public static Illuminance operator /(LuminousExposure luminousExposure, Duration duration) => Illuminance.FromLux(luminousExposure.ToLuxSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousExposure luminousExposure, Illuminance illuminance) => Duration.FromSeconds(luminousExposure.ToLuxSeconds() / illuminance.ToLux());
}
