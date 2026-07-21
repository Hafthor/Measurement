namespace com.hafthor.Measurement;

[Measurement("J/(kg·K)")]
public readonly partial struct SpecificHeatCapacity {

    // Units
    public static SpecificHeatCapacity FromJoulesPerKilogramKelvin(double joulesPerKilogramKelvin) => new(joulesPerKilogramKelvin);
    public double ToJoulesPerKilogramKelvin() => value;
    public static SpecificHeatCapacity FromKilojoulesPerKilogramKelvin(double kilojoulesPerKilogramKelvin) => new(kilojoulesPerKilogramKelvin * (1e3));
    public double ToKilojoulesPerKilogramKelvin() => value / (1e3);
    public static SpecificHeatCapacity FromCaloriesPerGramKelvin(double caloriesPerGramKelvin) => new(caloriesPerGramKelvin * (4184));
    public double ToCaloriesPerGramKelvin() => value / (4184);

    // Composite relationships
    public static HeatCapacity operator *(SpecificHeatCapacity specificHeatCapacity, Mass mass) => HeatCapacity.FromJoulesPerKelvin(specificHeatCapacity.ToJoulesPerKilogramKelvin() * mass.ToKilograms());
    public static HeatCapacity operator *(Mass mass, SpecificHeatCapacity specificHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(mass.ToKilograms() * specificHeatCapacity.ToJoulesPerKilogramKelvin());

}
