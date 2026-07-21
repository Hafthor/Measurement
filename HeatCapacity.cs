namespace com.hafthor.Measurement;

[Measurement("J/K")]
public readonly partial struct HeatCapacity {

    // Units
    public static HeatCapacity FromJoulesPerKelvin(double joulesPerKelvin) => new(joulesPerKelvin);
    public double ToJoulesPerKelvin() => value;
    public static HeatCapacity FromKilojoulesPerKelvin(double kilojoulesPerKelvin) => new(kilojoulesPerKelvin * (1e3));
    public double ToKilojoulesPerKelvin() => value / (1e3);
    public static HeatCapacity FromCaloriesPerKelvin(double caloriesPerKelvin) => new(caloriesPerKelvin * (4.184));
    public double ToCaloriesPerKelvin() => value / (4.184);

    // Composite relationships
    public static SpecificHeatCapacity operator /(HeatCapacity heatCapacity, Mass mass) => SpecificHeatCapacity.FromJoulesPerKilogramKelvin(heatCapacity.ToJoulesPerKelvin() / mass.ToKilograms());
    public static MolarHeatCapacity operator /(HeatCapacity heatCapacity, Quantity quantity) => MolarHeatCapacity.FromJoulesPerMoleKelvin(heatCapacity.ToJoulesPerKelvin() / quantity.ToMoles());
    public static Energy operator *(HeatCapacity heatCapacity, Temperature temperatureChange) => Energy.FromJoules(heatCapacity.ToJoulesPerKelvin() * temperatureChange.ToKelvin());

}
