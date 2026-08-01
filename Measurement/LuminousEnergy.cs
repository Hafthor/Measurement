namespace com.hafthor.Measurement;

[Measurement("lm·s", VariableName = "lumenSeconds")]
[SiUnit("LumenSeconds", 0)]
[Unit("LumenHours", 3600)]
[SiUnit("Talbots", 0)]
public readonly partial struct LuminousEnergy {
    // Composite relationships
    public static LuminousFlux operator /(LuminousEnergy luminousEnergy, Duration duration) => LuminousFlux.FromLumens(luminousEnergy.ToLumenSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousEnergy luminousEnergy, LuminousFlux luminousFlux) => Duration.FromSeconds(luminousEnergy.ToLumenSeconds() / luminousFlux.ToLumens());
}
