namespace com.hafthor.Measurement;

public class MolarHeatCapacity {
    private readonly double joulesPerMoleKelvin;

    private MolarHeatCapacity(double joulesPerMoleKelvin) => this.joulesPerMoleKelvin = joulesPerMoleKelvin;

    // Arithmetic
    public static MolarHeatCapacity operator +(MolarHeatCapacity a, MolarHeatCapacity b) => new MolarHeatCapacity(a.joulesPerMoleKelvin + b.joulesPerMoleKelvin);
    public static MolarHeatCapacity operator -(MolarHeatCapacity a, MolarHeatCapacity b) => new MolarHeatCapacity(a.joulesPerMoleKelvin - b.joulesPerMoleKelvin);
    public static MolarHeatCapacity operator -(MolarHeatCapacity x) => new MolarHeatCapacity(-x.joulesPerMoleKelvin);

    // Units
    public static MolarHeatCapacity FromJoulesPerMoleKelvin(double joulesPerMoleKelvin) => new MolarHeatCapacity(joulesPerMoleKelvin);
    public double ToJoulesPerMoleKelvin() => joulesPerMoleKelvin;
    public static MolarHeatCapacity FromCaloriesPerMoleKelvin(double caloriesPerMoleKelvin) => new MolarHeatCapacity(caloriesPerMoleKelvin * (4.184));
    public double ToCaloriesPerMoleKelvin() => joulesPerMoleKelvin / (4.184);

    // Composite relationships
    public static HeatCapacity operator *(MolarHeatCapacity molarHeatCapacity, Quantity quantity) => HeatCapacity.FromJoulesPerKelvin(molarHeatCapacity.ToJoulesPerMoleKelvin() * quantity.ToMoles());
    public static HeatCapacity operator *(Quantity quantity, MolarHeatCapacity molarHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(quantity.ToMoles() * molarHeatCapacity.ToJoulesPerMoleKelvin());
}
