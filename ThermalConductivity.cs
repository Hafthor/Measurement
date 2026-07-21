namespace com.hafthor.Measurement;

public class ThermalConductivity {
    private readonly double wattsPerMeterKelvin;

    private ThermalConductivity(double wattsPerMeterKelvin) => this.wattsPerMeterKelvin = wattsPerMeterKelvin;

    // Arithmetic
    public static ThermalConductivity operator +(ThermalConductivity a, ThermalConductivity b) => new ThermalConductivity(a.wattsPerMeterKelvin + b.wattsPerMeterKelvin);
    public static ThermalConductivity operator -(ThermalConductivity a, ThermalConductivity b) => new ThermalConductivity(a.wattsPerMeterKelvin - b.wattsPerMeterKelvin);
    public static ThermalConductivity operator -(ThermalConductivity x) => new ThermalConductivity(-x.wattsPerMeterKelvin);

    // Units
    public static ThermalConductivity FromWattsPerMeterKelvin(double wattsPerMeterKelvin) => new ThermalConductivity(wattsPerMeterKelvin);
    public double ToWattsPerMeterKelvin() => wattsPerMeterKelvin;
    public static ThermalConductivity FromMilliwattsPerMeterKelvin(double milliwattsPerMeterKelvin) => new ThermalConductivity(milliwattsPerMeterKelvin * (1e-3));
    public double ToMilliwattsPerMeterKelvin() => wattsPerMeterKelvin / (1e-3);
    public static ThermalConductivity FromBtuPerHourFootFahrenheit(double btuPerHourFootFahrenheit) => new ThermalConductivity(btuPerHourFootFahrenheit * (1.730734666));
    public double ToBtuPerHourFootFahrenheit() => wattsPerMeterKelvin / (1.730734666);
}
