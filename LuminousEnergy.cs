namespace com.hafthor.Measurement;

public sealed class LuminousEnergy : Measurement<LuminousEnergy> {

    private LuminousEnergy(double value) : base(value) { }

    protected override LuminousEnergy Create(double value) => new(value);
    protected override string Symbol => "lm·s";

    // Units
    public static LuminousEnergy FromLumenSeconds(double value) => new(value);
    public double ToLumenSeconds() => value;
    public static LuminousEnergy FromLumenHours(double lumenHours) => new(lumenHours * (3600));
    public double ToLumenHours() => value / (3600);
    public static LuminousEnergy FromTalbots(double talbots) => new(talbots);
    public double ToTalbots() => value;

    // Composite relationships
    public static LuminousFlux operator /(LuminousEnergy luminousEnergy, Duration duration) => LuminousFlux.FromLumens(luminousEnergy.ToLumenSeconds() / duration.ToSeconds());
    public static Duration operator /(LuminousEnergy luminousEnergy, LuminousFlux luminousFlux) => Duration.FromSeconds(luminousEnergy.ToLumenSeconds() / luminousFlux.ToLumens());

}
