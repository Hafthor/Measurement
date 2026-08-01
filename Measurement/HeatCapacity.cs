namespace com.hafthor.Measurement;

[Measurement("J/K", VariableName = "joulesPerKelvin")]
[SiUnit("JoulesPerKelvin", 0, "None Kilo")]
[Unit("CaloriesPerKelvin", 4.184)]
[Product<Mass, SpecificHeatCapacity>]
public readonly partial struct HeatCapacity {
    // Composite relationships
    public static MolarHeatCapacity operator /(HeatCapacity heatCapacity, Quantity quantity) => MolarHeatCapacity.FromJoulesPerMoleKelvin(heatCapacity.ToJoulesPerKelvin() / quantity.ToMoles());
}
