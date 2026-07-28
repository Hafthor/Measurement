namespace com.hafthor.Measurement;

[Measurement("W/(m·K)", VariableName = "milliwattsPerMeterKelvin", DisplayFactor = 1e3)]
public readonly partial struct ThermalConductivity {
    // Units
    public static ThermalConductivity FromWattsPerMeterKelvin(double wattsPerMeterKelvin) => new(wattsPerMeterKelvin * 1e3);
    public double ToWattsPerMeterKelvin() => milliwattsPerMeterKelvin / 1e3;
    public static ThermalConductivity FromMilliwattsPerMeterKelvin(double milliwattsPerMeterKelvin) => new(milliwattsPerMeterKelvin);
    public double ToMilliwattsPerMeterKelvin() => milliwattsPerMeterKelvin;
    public static ThermalConductivity FromBtuPerHourFootFahrenheit(double btuPerHourFootFahrenheit) => new(btuPerHourFootFahrenheit * (1.730734666e3));
    public double ToBtuPerHourFootFahrenheit() => milliwattsPerMeterKelvin / (1.730734666e3);
}
