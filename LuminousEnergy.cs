namespace com.hafthor.Measurement;

public class LuminousEnergy {
    private readonly double lumenSeconds;

    private LuminousEnergy(double lumenSeconds) => this.lumenSeconds = lumenSeconds;

    // Arithmetic
    public static LuminousEnergy operator +(LuminousEnergy a, LuminousEnergy b) => new LuminousEnergy(a.lumenSeconds + b.lumenSeconds);
    public static LuminousEnergy operator -(LuminousEnergy a, LuminousEnergy b) => new LuminousEnergy(a.lumenSeconds - b.lumenSeconds);
    public static LuminousEnergy operator -(LuminousEnergy x) => new LuminousEnergy(-x.lumenSeconds);

    // Units
    public static LuminousEnergy FromLumenSeconds(double lumenSeconds) => new LuminousEnergy(lumenSeconds);
    public double ToLumenSeconds() => lumenSeconds;
    public static LuminousEnergy FromLumenHours(double lumenHours) => new LuminousEnergy(lumenHours * (3600));
    public double ToLumenHours() => lumenSeconds / (3600);
    public static LuminousEnergy FromTalbots(double talbots) => new LuminousEnergy(talbots);
    public double ToTalbots() => lumenSeconds;

    // Composite relationships
    public static LuminousFlux operator /(LuminousEnergy luminousEnergy, Duration duration) => LuminousFlux.FromLumens(luminousEnergy.ToLumenSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousEnergy luminousEnergy, LuminousFlux luminousFlux) => Duration.FromSeconds(luminousEnergy.ToLumenSeconds() / luminousFlux.ToLumens());
}
