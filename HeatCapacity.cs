namespace com.hafthor.Measurement;

public class HeatCapacity {
    private readonly double joulesPerKelvin;

    private HeatCapacity(double joulesPerKelvin) => this.joulesPerKelvin = joulesPerKelvin;

    // Arithmetic
    public static HeatCapacity operator +(HeatCapacity a, HeatCapacity b) => new HeatCapacity(a.joulesPerKelvin + b.joulesPerKelvin);
    public static HeatCapacity operator -(HeatCapacity a, HeatCapacity b) => new HeatCapacity(a.joulesPerKelvin - b.joulesPerKelvin);
    public static HeatCapacity operator -(HeatCapacity x) => new HeatCapacity(-x.joulesPerKelvin);

    // Units
    public static HeatCapacity FromJoulesPerKelvin(double joulesPerKelvin) => new HeatCapacity(joulesPerKelvin);
    public double ToJoulesPerKelvin() => joulesPerKelvin;
    public static HeatCapacity FromKilojoulesPerKelvin(double kilojoulesPerKelvin) => new HeatCapacity(kilojoulesPerKelvin * (1e3));
    public double ToKilojoulesPerKelvin() => joulesPerKelvin / (1e3);
    public static HeatCapacity FromCaloriesPerKelvin(double caloriesPerKelvin) => new HeatCapacity(caloriesPerKelvin * (4.184));
    public double ToCaloriesPerKelvin() => joulesPerKelvin / (4.184);

    // Composite relationships
    public static SpecificHeatCapacity operator /(HeatCapacity heatCapacity, Mass mass) => SpecificHeatCapacity.FromJoulesPerKilogramKelvin(heatCapacity.ToJoulesPerKelvin() / mass.ToKilograms());
    public static MolarHeatCapacity operator /(HeatCapacity heatCapacity, Quantity quantity) => MolarHeatCapacity.FromJoulesPerMoleKelvin(heatCapacity.ToJoulesPerKelvin() / quantity.ToMoles());
    public static Energy operator *(HeatCapacity heatCapacity, Temperature temperatureChange) => Energy.FromJoules(heatCapacity.ToJoulesPerKelvin() * temperatureChange.ToKelvin());
}
