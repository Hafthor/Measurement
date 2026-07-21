namespace com.hafthor.Measurement;

[Measurement("W/(m·K)")]
public readonly partial struct ThermalConductivity {

    // Units
    public static ThermalConductivity FromWattsPerMeterKelvin(double wattsPerMeterKelvin) => new(wattsPerMeterKelvin);
    public double ToWattsPerMeterKelvin() => value;
    public static ThermalConductivity FromMilliwattsPerMeterKelvin(double milliwattsPerMeterKelvin) => new(milliwattsPerMeterKelvin * (1e-3));
    public double ToMilliwattsPerMeterKelvin() => value / (1e-3);
    public static ThermalConductivity FromBtuPerHourFootFahrenheit(double btuPerHourFootFahrenheit) => new(btuPerHourFootFahrenheit * (1.730734666));
    public double ToBtuPerHourFootFahrenheit() => value / (1.730734666);

}
