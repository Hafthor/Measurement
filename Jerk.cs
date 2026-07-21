namespace com.hafthor.Measurement;

public class Jerk {
    private readonly double metersPerSecondCubed;

    private Jerk(double metersPerSecondCubed) => this.metersPerSecondCubed = metersPerSecondCubed;

    // Arithmetic
    public static Jerk operator +(Jerk a, Jerk b) => new Jerk(a.metersPerSecondCubed + b.metersPerSecondCubed);
    public static Jerk operator -(Jerk a, Jerk b) => new Jerk(a.metersPerSecondCubed - b.metersPerSecondCubed);
    public static Jerk operator -(Jerk x) => new Jerk(-x.metersPerSecondCubed);

    // Units
    public static Jerk FromMetersPerSecondCubed(double metersPerSecondCubed) => new Jerk(metersPerSecondCubed);
    public double ToMetersPerSecondCubed() => metersPerSecondCubed;
    public static Jerk FromFeetPerSecondCubed(double feetPerSecondCubed) => new Jerk(feetPerSecondCubed * (0.3048));
    public double ToFeetPerSecondCubed() => metersPerSecondCubed / (0.3048);
    public static Jerk FromGalsPerSecond(double galsPerSecond) => new Jerk(galsPerSecond * (1e-2));
    public double ToGalsPerSecond() => metersPerSecondCubed / (1e-2);

    // Composite relationships
    public static Acceleration operator *(Jerk jerk, Duration duration) => Acceleration.FromMetersPerSecondSquared(jerk.ToMetersPerSecondCubed() * duration.ToSeconds());
    public static Acceleration operator *(Duration duration, Jerk jerk) => Acceleration.FromMetersPerSecondSquared(duration.ToSeconds() * jerk.ToMetersPerSecondCubed());
}
