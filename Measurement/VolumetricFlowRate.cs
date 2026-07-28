namespace com.hafthor.Measurement;

[Measurement("m³/s", VariableName = "cubicMillimetersPerSecond", DisplayFactor = 1e9)]
public readonly partial struct VolumetricFlowRate {
    // Units
    public static VolumetricFlowRate FromCubicMetersPerSecond(double cubicMetersPerSecond) => new(cubicMetersPerSecond * 1e9);
    public double ToCubicMetersPerSecond() => cubicMillimetersPerSecond / 1e9;
    public static VolumetricFlowRate FromLitersPerSecond(double litersPerSecond) => new(litersPerSecond * (1e6));
    public double ToLitersPerSecond() => cubicMillimetersPerSecond / (1e6);
    public static VolumetricFlowRate FromLitersPerMinute(double litersPerMinute) => new(litersPerMinute * (1e6 / 60));
    public double ToLitersPerMinute() => cubicMillimetersPerSecond / (1e6 / 60);
    public static VolumetricFlowRate FromCubicFeetPerSecond(double cubicFeetPerSecond) => new(cubicFeetPerSecond * (0.028316846592e9));
    public double ToCubicFeetPerSecond() => cubicMillimetersPerSecond / (0.028316846592e9);
    public static VolumetricFlowRate FromGallonsPerMinute(double gallonsPerMinute) => new(gallonsPerMinute * (0.003785411784e9 / 60));
    public double ToGallonsPerMinute() => cubicMillimetersPerSecond / (0.003785411784e9 / 60);

    // Composite relationships
    public static Volume operator *(VolumetricFlowRate volumetricFlowRate, Duration duration) => Volume.FromCubicMeters(volumetricFlowRate.ToCubicMetersPerSecond() * duration.ToSeconds());
    public static Volume operator *(Duration duration, VolumetricFlowRate volumetricFlowRate) => Volume.FromCubicMeters(duration.ToSeconds() * volumetricFlowRate.ToCubicMetersPerSecond());
}
