namespace com.hafthor.Measurement;

public class VolumetricFlowRate {
    private readonly double cubicMetersPerSecond;

    private VolumetricFlowRate(double cubicMetersPerSecond) => this.cubicMetersPerSecond = cubicMetersPerSecond;

    // Arithmetic
    public static VolumetricFlowRate operator +(VolumetricFlowRate a, VolumetricFlowRate b) => new VolumetricFlowRate(a.cubicMetersPerSecond + b.cubicMetersPerSecond);
    public static VolumetricFlowRate operator -(VolumetricFlowRate a, VolumetricFlowRate b) => new VolumetricFlowRate(a.cubicMetersPerSecond - b.cubicMetersPerSecond);
    public static VolumetricFlowRate operator -(VolumetricFlowRate x) => new VolumetricFlowRate(-x.cubicMetersPerSecond);

    // Units
    public static VolumetricFlowRate FromCubicMetersPerSecond(double cubicMetersPerSecond) => new VolumetricFlowRate(cubicMetersPerSecond);
    public double ToCubicMetersPerSecond() => cubicMetersPerSecond;
    public static VolumetricFlowRate FromLitersPerSecond(double litersPerSecond) => new VolumetricFlowRate(litersPerSecond * (1e-3));
    public double ToLitersPerSecond() => cubicMetersPerSecond / (1e-3);
    public static VolumetricFlowRate FromLitersPerMinute(double litersPerMinute) => new VolumetricFlowRate(litersPerMinute * (1e-3 / 60));
    public double ToLitersPerMinute() => cubicMetersPerSecond / (1e-3 / 60);
    public static VolumetricFlowRate FromCubicFeetPerSecond(double cubicFeetPerSecond) => new VolumetricFlowRate(cubicFeetPerSecond * (0.028316846592));
    public double ToCubicFeetPerSecond() => cubicMetersPerSecond / (0.028316846592);
    public static VolumetricFlowRate FromGallonsPerMinute(double gallonsPerMinute) => new VolumetricFlowRate(gallonsPerMinute * (0.003785411784 / 60));
    public double ToGallonsPerMinute() => cubicMetersPerSecond / (0.003785411784 / 60);

    // Composite relationships
    public static Volume operator *(VolumetricFlowRate volumetricFlowRate, Duration duration) => Volume.FromCubicMeters(volumetricFlowRate.ToCubicMetersPerSecond() * duration.ToSeconds());
    public static Volume operator *(Duration duration, VolumetricFlowRate volumetricFlowRate) => Volume.FromCubicMeters(duration.ToSeconds() * volumetricFlowRate.ToCubicMetersPerSecond());
}
