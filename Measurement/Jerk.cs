namespace com.hafthor.Measurement;

[Measurement("m/s³", VariableName = "metersPerSecondCubed")]
[SiUnit("MetersPerSecondCubed", 0)]
[Unit("FeetPerSecondCubed", 0.3048)]
[SiUnit("GalsPerSecond", -2)]
public readonly partial struct Jerk {
    // Composite relationships
    public static Acceleration operator *(Jerk jerk, Duration duration) => Acceleration.FromMetersPerSecondSquared(jerk.ToMetersPerSecondCubed() * duration.ToSeconds());
    public static Acceleration operator *(Duration duration, Jerk jerk) => Acceleration.FromMetersPerSecondSquared(duration.ToSeconds() * jerk.ToMetersPerSecondCubed());
}
