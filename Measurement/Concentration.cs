namespace com.hafthor.Measurement;

[Measurement("mol/m³", VariableName = "micromolesPerLiter", DisplayFactor = 1e3)]
public readonly partial struct Concentration {
    // Units
    public static Concentration FromMolesPerCubicMeter(double molesPerCubicMeter) => new(molesPerCubicMeter * 1e3);
    public double ToMolesPerCubicMeter() => micromolesPerLiter / 1e3;
    public static Concentration FromMolesPerLiter(double molesPerLiter) => new(molesPerLiter * 1e6);
    public double ToMolesPerLiter() => micromolesPerLiter / 1e6;
    public static Concentration FromMillimolesPerLiter(double millimolesPerLiter) => new(millimolesPerLiter * 1e3);
    public double ToMillimolesPerLiter() => micromolesPerLiter / 1e3;
    public static Concentration FromMicromolesPerLiter(double micromolesPerLiter) => new(micromolesPerLiter);
    public double ToMicromolesPerLiter() => micromolesPerLiter;

    // Composite relationships
    public static Quantity operator *(Concentration concentration, Volume volume) => Quantity.FromMoles(concentration.ToMolesPerCubicMeter() * volume.ToCubicMeters());
    public static Quantity operator *(Volume volume, Concentration concentration) => Quantity.FromMoles(volume.ToCubicMeters() * concentration.ToMolesPerCubicMeter());
    public static ReactionRate operator /(Concentration concentration, Duration duration) => ReactionRate.FromMolesPerCubicMeterSecond(concentration.ToMolesPerCubicMeter() / duration.ToSeconds());
}
