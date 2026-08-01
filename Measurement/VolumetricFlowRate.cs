namespace com.hafthor.Measurement;

[Measurement("m³/s", VariableName = "cubicMillimetersPerSecond", DisplayFactor = 1e9)]
[SiUnit("CubicMetersPerSecond", 9)]
[SiUnit("LitersPerSecond", 6)]
[Unit("LitersPerMinute", 1e6 / 60)]
[Unit("CubicFeetPerSecond", 0.028316846592e9)]
[Unit("GallonsPerMinute", 0.003785411784e9 / 60)]
public readonly partial struct VolumetricFlowRate {
    // Composite relationships
    public static Volume operator *(VolumetricFlowRate volumetricFlowRate, Duration duration) => Volume.FromCubicMeters(volumetricFlowRate.ToCubicMetersPerSecond() * duration.ToSeconds());
    public static Volume operator *(Duration duration, VolumetricFlowRate volumetricFlowRate) => Volume.FromCubicMeters(duration.ToSeconds() * volumetricFlowRate.ToCubicMetersPerSecond());
}
