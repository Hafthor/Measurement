namespace com.hafthor.Measurement;

[Measurement("J/(mol·K)", VariableName = "joulesPerMoleKelvin")]
[SiUnit("JoulesPerMoleKelvin", 0)]
[Unit("CaloriesPerMoleKelvin", 4.184)]
public readonly partial struct MolarHeatCapacity {
    public static HeatCapacity operator *(MolarHeatCapacity molarHeatCapacity, Quantity quantity) => HeatCapacity.FromJoulesPerKelvin(molarHeatCapacity.ToJoulesPerMoleKelvin() * quantity.ToMoles());
    public static HeatCapacity operator *(Quantity quantity, MolarHeatCapacity molarHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(quantity.ToMoles() * molarHeatCapacity.ToJoulesPerMoleKelvin());
}
