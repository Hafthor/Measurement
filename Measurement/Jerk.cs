namespace com.hafthor.Measurement;

[Measurement("m/s³")]
public readonly partial struct Jerk {

    // Units
    public static Jerk FromMetersPerSecondCubed(double metersPerSecondCubed) => new(metersPerSecondCubed);
    public double ToMetersPerSecondCubed() => value;
    public static Jerk FromFeetPerSecondCubed(double feetPerSecondCubed) => new(feetPerSecondCubed * (0.3048));
    public double ToFeetPerSecondCubed() => value / (0.3048);
    public static Jerk FromGalsPerSecond(double galsPerSecond) => new(galsPerSecond * (1e-2));
    public double ToGalsPerSecond() => value / (1e-2);

    // Composite relationships
    public static Acceleration operator *(Jerk jerk, Duration duration) => Acceleration.FromMetersPerSecondSquared(jerk.ToMetersPerSecondCubed() * duration.ToSeconds());
    public static Acceleration operator *(Duration duration, Jerk jerk) => Acceleration.FromMetersPerSecondSquared(duration.ToSeconds() * jerk.ToMetersPerSecondCubed());

}
