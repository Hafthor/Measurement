namespace com.hafthor.Measurement;

[Measurement("J/(g·K)", VariableName = "joulesPerGramKelvin")]
public readonly partial struct SpecificHeatCapacity {
    // Units
    public static SpecificHeatCapacity FromJoulesPerKilogramKelvin(double joulesPerKilogramKelvin) => new(joulesPerKilogramKelvin * 1e-3);
    public double ToJoulesPerKilogramKelvin() => joulesPerGramKelvin / 1e-3;
    public static SpecificHeatCapacity FromJoulesPerGramKelvin(double joulesPerGramKelvin) => new(joulesPerGramKelvin);
    public double ToJoulesPerGramKelvin() => joulesPerGramKelvin;
    public static SpecificHeatCapacity FromKilojoulesPerKilogramKelvin(double kilojoulesPerKilogramKelvin) => new(kilojoulesPerKilogramKelvin);
    public double ToKilojoulesPerKilogramKelvin() => joulesPerGramKelvin;
    public static SpecificHeatCapacity FromCaloriesPerGramKelvin(double caloriesPerGramKelvin) => new(caloriesPerGramKelvin * (4.184));
    public double ToCaloriesPerGramKelvin() => joulesPerGramKelvin / (4.184);

    // Composite relationships
    public static HeatCapacity operator *(SpecificHeatCapacity specificHeatCapacity, Mass mass) => HeatCapacity.FromJoulesPerKelvin(specificHeatCapacity.ToJoulesPerKilogramKelvin() * mass.ToKilograms());
    public static HeatCapacity operator *(Mass mass, SpecificHeatCapacity specificHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(mass.ToKilograms() * specificHeatCapacity.ToJoulesPerKilogramKelvin());
}
