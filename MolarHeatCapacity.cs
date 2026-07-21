namespace com.hafthor.Measurement;

[Measurement("J/(mol·K)")]
public readonly partial struct MolarHeatCapacity {

    // Units
    public static MolarHeatCapacity FromJoulesPerMoleKelvin(double joulesPerMoleKelvin) => new(joulesPerMoleKelvin);
    public double ToJoulesPerMoleKelvin() => value;
    public static MolarHeatCapacity FromCaloriesPerMoleKelvin(double caloriesPerMoleKelvin) => new(caloriesPerMoleKelvin * (4.184));
    public double ToCaloriesPerMoleKelvin() => value / (4.184);

    // Composite relationships
    public static HeatCapacity operator *(MolarHeatCapacity molarHeatCapacity, Quantity quantity) => HeatCapacity.FromJoulesPerKelvin(molarHeatCapacity.ToJoulesPerMoleKelvin() * quantity.ToMoles());
    public static HeatCapacity operator *(Quantity quantity, MolarHeatCapacity molarHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(quantity.ToMoles() * molarHeatCapacity.ToJoulesPerMoleKelvin());

}
