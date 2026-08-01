namespace com.hafthor.Measurement;

[Measurement("m/s²", VariableName = "metersPerSecondSquared")]
[SiUnit("MetersPerSecondSquared", 0)]
[Unit("KilometersPerHourPerSecond", 1.0/(3.6))]
[Unit("FeetPerSecondSquared", 0.3048)]
[Unit("StandardGravity", 9.80665)]
[SiUnit("Gals", -2)]
public readonly partial struct Acceleration {
    // Composite relationships
    public static Speed operator *(Acceleration acceleration, Duration duration) => Speed.FromMetersPerSecond(acceleration.metersPerSecondSquared * duration.ToSeconds());
    public static Force operator *(Acceleration acceleration, Mass mass) => Force.FromNewtons(acceleration.metersPerSecondSquared * mass.ToKilograms());

    // Composite relationships (derived)
    public static Jerk operator /(Acceleration acceleration, Duration duration) => Jerk.FromMetersPerSecondCubed(acceleration.metersPerSecondSquared / duration.ToSeconds());
}
