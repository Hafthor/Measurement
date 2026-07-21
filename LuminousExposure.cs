namespace com.hafthor.Measurement;

public class LuminousExposure {
    private readonly double luxSeconds;

    private LuminousExposure(double luxSeconds) => this.luxSeconds = luxSeconds;

    // Arithmetic
    public static LuminousExposure operator +(LuminousExposure a, LuminousExposure b) => new(a.luxSeconds + b.luxSeconds);
    public static LuminousExposure operator -(LuminousExposure a, LuminousExposure b) => new(a.luxSeconds - b.luxSeconds);
    public static LuminousExposure operator -(LuminousExposure x) => new(-x.luxSeconds);

    // Units
    public static LuminousExposure FromLuxSeconds(double luxSeconds) => new(luxSeconds);
    public double ToLuxSeconds() => luxSeconds;
    public static LuminousExposure FromLuxHours(double luxHours) => new(luxHours * (3600));
    public double ToLuxHours() => luxSeconds / (3600);

    // Composite relationships
    public static Illuminance operator /(LuminousExposure luminousExposure, Duration duration) => Illuminance.FromLux(luminousExposure.ToLuxSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousExposure luminousExposure, Illuminance illuminance) => Duration.FromSeconds(luminousExposure.ToLuxSeconds() / illuminance.ToLux());

    public override string ToString() => $"{luxSeconds} lx·s";

    public override bool Equals(object obj) => obj is LuminousExposure other && other.luxSeconds == luxSeconds;
    public override int GetHashCode() => luxSeconds.GetHashCode();
}
