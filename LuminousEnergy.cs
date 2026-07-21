namespace com.hafthor.Measurement;

[Measurement("lm·s")]
public readonly partial struct LuminousEnergy {

    // Units
    public static LuminousEnergy FromLumenSeconds(double lumenSeconds) => new(lumenSeconds);
    public double ToLumenSeconds() => value;
    public static LuminousEnergy FromLumenHours(double lumenHours) => new(lumenHours * (3600));
    public double ToLumenHours() => value / (3600);
    public static LuminousEnergy FromTalbots(double talbots) => new(talbots);
    public double ToTalbots() => value;

    // Composite relationships
    public static LuminousFlux operator /(LuminousEnergy luminousEnergy, Duration duration) => LuminousFlux.FromLumens(luminousEnergy.ToLumenSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousEnergy luminousEnergy, LuminousFlux luminousFlux) => Duration.FromSeconds(luminousEnergy.ToLumenSeconds() / luminousFlux.ToLumens());

}
