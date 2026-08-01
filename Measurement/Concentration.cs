namespace com.hafthor.Measurement;

[Measurement("mol/m³", VariableName = "micromolesPerLiter", DisplayFactor = 1e3)]
[SiUnit("MolesPerCubicMeter", 3)]
[SiUnit("MolesPerLiter", 6, "None Milli Micro")]
[Product<Duration, ReactionRate>]
public readonly partial struct Concentration {
    // Composite relationships
    public static Quantity operator *(Concentration concentration, Volume volume) => Quantity.FromMoles(concentration.ToMolesPerCubicMeter() * volume.ToCubicMeters());
    public static Quantity operator *(Volume volume, Concentration concentration) => Quantity.FromMoles(volume.ToCubicMeters() * concentration.ToMolesPerCubicMeter());
}
