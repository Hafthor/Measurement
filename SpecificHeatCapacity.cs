namespace com.hafthor.Measurement;

public class SpecificHeatCapacity {
    private readonly double joulesPerKilogramKelvin;

    private SpecificHeatCapacity(double joulesPerKilogramKelvin) => this.joulesPerKilogramKelvin = joulesPerKilogramKelvin;

    // Arithmetic
    public static SpecificHeatCapacity operator +(SpecificHeatCapacity a, SpecificHeatCapacity b) => new SpecificHeatCapacity(a.joulesPerKilogramKelvin + b.joulesPerKilogramKelvin);
    public static SpecificHeatCapacity operator -(SpecificHeatCapacity a, SpecificHeatCapacity b) => new SpecificHeatCapacity(a.joulesPerKilogramKelvin - b.joulesPerKilogramKelvin);
    public static SpecificHeatCapacity operator -(SpecificHeatCapacity x) => new SpecificHeatCapacity(-x.joulesPerKilogramKelvin);

    // Units
    public static SpecificHeatCapacity FromJoulesPerKilogramKelvin(double joulesPerKilogramKelvin) => new SpecificHeatCapacity(joulesPerKilogramKelvin);
    public double ToJoulesPerKilogramKelvin() => joulesPerKilogramKelvin;
    public static SpecificHeatCapacity FromKilojoulesPerKilogramKelvin(double kilojoulesPerKilogramKelvin) => new SpecificHeatCapacity(kilojoulesPerKilogramKelvin * (1e3));
    public double ToKilojoulesPerKilogramKelvin() => joulesPerKilogramKelvin / (1e3);
    public static SpecificHeatCapacity FromCaloriesPerGramKelvin(double caloriesPerGramKelvin) => new SpecificHeatCapacity(caloriesPerGramKelvin * (4184));
    public double ToCaloriesPerGramKelvin() => joulesPerKilogramKelvin / (4184);

    // Composite relationships
    public static HeatCapacity operator *(SpecificHeatCapacity specificHeatCapacity, Mass mass) => HeatCapacity.FromJoulesPerKelvin(specificHeatCapacity.ToJoulesPerKilogramKelvin() * mass.ToKilograms());
    public static HeatCapacity operator *(Mass mass, SpecificHeatCapacity specificHeatCapacity) => HeatCapacity.FromJoulesPerKelvin(mass.ToKilograms() * specificHeatCapacity.ToJoulesPerKilogramKelvin());
}
