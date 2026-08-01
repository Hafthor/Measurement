namespace com.hafthor.Measurement;

[Measurement("J/K", VariableName = "joulesPerKelvin")]
[SiUnit("JoulesPerKelvin", 0, "None Kilo")]
[Unit("CaloriesPerKelvin", 4.184)]
public readonly partial struct HeatCapacity {
    // Composite relationships
    public static SpecificHeatCapacity operator /(HeatCapacity heatCapacity, Mass mass) => SpecificHeatCapacity.FromJoulesPerKilogramKelvin(heatCapacity.ToJoulesPerKelvin() / mass.ToKilograms());
    public static MolarHeatCapacity operator /(HeatCapacity heatCapacity, Quantity quantity) => MolarHeatCapacity.FromJoulesPerMoleKelvin(heatCapacity.ToJoulesPerKelvin() / quantity.ToMoles());
    public static Energy operator *(HeatCapacity heatCapacity, Temperature temperatureChange) => Energy.FromJoules(heatCapacity.ToJoulesPerKelvin() * temperatureChange.ToKelvin());
}
