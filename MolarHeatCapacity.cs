namespace com.hafthor.Measurement;

public sealed class MolarHeatCapacity : Measurement<MolarHeatCapacity> {

    private MolarHeatCapacity(double value) : base(value) { }

    protected override MolarHeatCapacity Create(double value) => new(value);
    protected override string Symbol => "J/(mol·K)";

    // Units
    public static MolarHeatCapacity FromJoulesPerMoleKelvin(double value) => new(value);
    public double ToJoulesPerMoleKelvin() => value;
    public static MolarHeatCapacity FromCaloriesPerMoleKelvin(double caloriesPerMoleKelvin) => new(caloriesPerMoleKelvin * (4.184));
    public double ToCaloriesPerMoleKelvin() => value / (4.184);

    // Composite relationships
    public static HeatCapacity operator *(MolarHeatCapacity molarHeatCapacity, Quantity quantity) => HeatCapacity.FromJoulesPerKelvin(molarHeatCapacity.ToJoulesPerMoleKelvin() * quantity.ToMoles());
    public static HeatCapacity operator *(Quantity quantity, MolarHeatCapacity molarHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(quantity.ToMoles() * molarHeatCapacity.ToJoulesPerMoleKelvin());

}
