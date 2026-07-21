namespace com.hafthor.Measurement;

[Measurement("m³/s")]
public readonly partial struct VolumetricFlowRate {

    // Units
    public static VolumetricFlowRate FromCubicMetersPerSecond(double cubicMetersPerSecond) => new(cubicMetersPerSecond);
    public double ToCubicMetersPerSecond() => value;
    public static VolumetricFlowRate FromLitersPerSecond(double litersPerSecond) => new(litersPerSecond * (1e-3));
    public double ToLitersPerSecond() => value / (1e-3);
    public static VolumetricFlowRate FromLitersPerMinute(double litersPerMinute) => new(litersPerMinute * (1e-3 / 60));
    public double ToLitersPerMinute() => value / (1e-3 / 60);
    public static VolumetricFlowRate FromCubicFeetPerSecond(double cubicFeetPerSecond) => new(cubicFeetPerSecond * (0.028316846592));
    public double ToCubicFeetPerSecond() => value / (0.028316846592);
    public static VolumetricFlowRate FromGallonsPerMinute(double gallonsPerMinute) => new(gallonsPerMinute * (0.003785411784 / 60));
    public double ToGallonsPerMinute() => value / (0.003785411784 / 60);

    // Composite relationships
    public static Volume operator *(VolumetricFlowRate volumetricFlowRate, Duration duration) => Volume.FromCubicMeters(volumetricFlowRate.ToCubicMetersPerSecond() * duration.ToSeconds());
    public static Volume operator *(Duration duration, VolumetricFlowRate volumetricFlowRate) => Volume.FromCubicMeters(duration.ToSeconds() * volumetricFlowRate.ToCubicMetersPerSecond());

}
