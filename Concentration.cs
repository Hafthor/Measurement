namespace com.hafthor.Measurement;

public sealed class Concentration : Measurement<Concentration> {

    private Concentration(double value) : base(value) { }

    protected override Concentration Create(double value) => new(value);
    protected override string Symbol => "mol/m³";

    // Units
    public static Concentration FromMolesPerCubicMeter(double value) => new(value);
    public double ToMolesPerCubicMeter() => value;
    public static Concentration FromMolesPerLiter(double molesPerLiter) => new(molesPerLiter * (1000));
    public double ToMolesPerLiter() => value / (1000);
    public static Concentration FromMillimolesPerLiter(double millimolesPerLiter) => new(millimolesPerLiter);
    public double ToMillimolesPerLiter() => value;
    public static Concentration FromMicromolesPerLiter(double micromolesPerLiter) => new(micromolesPerLiter * (1e-3));
    public double ToMicromolesPerLiter() => value / (1e-3);

    // Composite relationships
    public static Quantity operator *(Concentration concentration, Volume volume) => Quantity.FromMoles(concentration.ToMolesPerCubicMeter() * volume.ToCubicMeters());
    public static Quantity operator *(Volume volume, Concentration concentration) => Quantity.FromMoles(volume.ToCubicMeters() * concentration.ToMolesPerCubicMeter());
    public static ReactionRate operator /(Concentration concentration, Duration duration) => ReactionRate.FromMolesPerCubicMeterSecond(concentration.ToMolesPerCubicMeter() / duration.ToSeconds());

}
