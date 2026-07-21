namespace com.hafthor.Measurement;

public sealed class SpecificHeatCapacity : Measurement<SpecificHeatCapacity> {

    private SpecificHeatCapacity(double value) : base(value) { }

    protected override SpecificHeatCapacity Create(double value) => new(value);
    protected override string Symbol => "J/(kg·K)";

    // Units
    public static SpecificHeatCapacity FromJoulesPerKilogramKelvin(double value) => new(value);
    public double ToJoulesPerKilogramKelvin() => value;
    public static SpecificHeatCapacity FromKilojoulesPerKilogramKelvin(double kilojoulesPerKilogramKelvin) => new(kilojoulesPerKilogramKelvin * (1e3));
    public double ToKilojoulesPerKilogramKelvin() => value / (1e3);
    public static SpecificHeatCapacity FromCaloriesPerGramKelvin(double caloriesPerGramKelvin) => new(caloriesPerGramKelvin * (4184));
    public double ToCaloriesPerGramKelvin() => value / (4184);

    // Composite relationships
    public static HeatCapacity operator *(SpecificHeatCapacity specificHeatCapacity, Mass mass) => HeatCapacity.FromJoulesPerKelvin(specificHeatCapacity.ToJoulesPerKilogramKelvin() * mass.ToKilograms());
    public static HeatCapacity operator *(Mass mass, SpecificHeatCapacity specificHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(mass.ToKilograms() * specificHeatCapacity.ToJoulesPerKilogramKelvin());

}
