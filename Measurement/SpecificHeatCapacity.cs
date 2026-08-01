namespace com.hafthor.Measurement;

[Measurement("J/(g·K)", VariableName = "joulesPerGramKelvin")]
[SiUnit("JoulesPerKilogramKelvin", -3, "None Kilo")]
[SiUnit("JoulesPerGramKelvin", 0)]
[Unit("CaloriesPerGramKelvin", 4.184)]
public readonly partial struct SpecificHeatCapacity {
    // Composite relationships
    public static HeatCapacity operator *(SpecificHeatCapacity specificHeatCapacity, Mass mass) => HeatCapacity.FromJoulesPerKelvin(specificHeatCapacity.ToJoulesPerKilogramKelvin() * mass.ToKilograms());
    public static HeatCapacity operator *(Mass mass, SpecificHeatCapacity specificHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(mass.ToKilograms() * specificHeatCapacity.ToJoulesPerKilogramKelvin());
}
