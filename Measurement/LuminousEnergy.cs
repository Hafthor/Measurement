namespace com.hafthor.Measurement;

[Measurement("lm·s", VariableName = "lumenSeconds")]
public readonly partial struct LuminousEnergy {
    // Units
    public static LuminousEnergy FromLumenSeconds(double lumenSeconds) => new(lumenSeconds);
    public double ToLumenSeconds() => lumenSeconds;
    public static LuminousEnergy FromLumenHours(double lumenHours) => new(lumenHours * (3600));
    public double ToLumenHours() => lumenSeconds / (3600);
    public static LuminousEnergy FromTalbots(double talbots) => new(talbots);
    public double ToTalbots() => lumenSeconds;

    // Composite relationships
    public static LuminousFlux operator /(LuminousEnergy luminousEnergy, Duration duration) => LuminousFlux.FromLumens(luminousEnergy.ToLumenSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousEnergy luminousEnergy, LuminousFlux luminousFlux) => Duration.FromSeconds(luminousEnergy.ToLumenSeconds() / luminousFlux.ToLumens());
}
