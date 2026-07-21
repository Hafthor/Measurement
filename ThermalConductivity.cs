namespace com.hafthor.Measurement;

public sealed class ThermalConductivity : Measurement<ThermalConductivity> {

    private ThermalConductivity(double value) : base(value) { }

    protected override ThermalConductivity Create(double value) => new(value);
    protected override string Symbol => "W/(m·K)";

    // Units
    public static ThermalConductivity FromWattsPerMeterKelvin(double value) => new(value);
    public double ToWattsPerMeterKelvin() => value;
    public static ThermalConductivity FromMilliwattsPerMeterKelvin(double milliwattsPerMeterKelvin) => new(milliwattsPerMeterKelvin * (1e-3));
    public double ToMilliwattsPerMeterKelvin() => value / (1e-3);
    public static ThermalConductivity FromBtuPerHourFootFahrenheit(double btuPerHourFootFahrenheit) => new(btuPerHourFootFahrenheit * (1.730734666));
    public double ToBtuPerHourFootFahrenheit() => value / (1.730734666);

}
