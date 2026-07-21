namespace com.hafthor.Measurement;

public class Concentration {
    private readonly double molesPerCubicMeter;

    private Concentration(double molesPerCubicMeter) => this.molesPerCubicMeter = molesPerCubicMeter;

    // Arithmetic
    public static Concentration operator +(Concentration a, Concentration b) => new Concentration(a.molesPerCubicMeter + b.molesPerCubicMeter);
    public static Concentration operator -(Concentration a, Concentration b) => new Concentration(a.molesPerCubicMeter - b.molesPerCubicMeter);
    public static Concentration operator -(Concentration x) => new Concentration(-x.molesPerCubicMeter);

    // Units
    public static Concentration FromMolesPerCubicMeter(double molesPerCubicMeter) => new Concentration(molesPerCubicMeter);
    public double ToMolesPerCubicMeter() => molesPerCubicMeter;
    public static Concentration FromMolesPerLiter(double molesPerLiter) => new Concentration(molesPerLiter * (1000));
    public double ToMolesPerLiter() => molesPerCubicMeter / (1000);
    public static Concentration FromMillimolesPerLiter(double millimolesPerLiter) => new Concentration(millimolesPerLiter);
    public double ToMillimolesPerLiter() => molesPerCubicMeter;
    public static Concentration FromMicromolesPerLiter(double micromolesPerLiter) => new Concentration(micromolesPerLiter * (1e-3));
    public double ToMicromolesPerLiter() => molesPerCubicMeter / (1e-3);

    // Composite relationships
    public static Quantity operator *(Concentration concentration, Volume volume) => Quantity.FromMoles(concentration.ToMolesPerCubicMeter() * volume.ToCubicMeters());
    public static Quantity operator *(Volume volume, Concentration concentration) => Quantity.FromMoles(volume.ToCubicMeters() * concentration.ToMolesPerCubicMeter());
    public static ReactionRate operator /(Concentration concentration, Duration duration) => ReactionRate.FromMolesPerCubicMeterSecond(concentration.ToMolesPerCubicMeter() / duration.ToSeconds());
}
