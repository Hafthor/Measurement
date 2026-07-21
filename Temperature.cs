namespace com.hafthor.Measurement;

public class Temperature {
    private readonly double kelvin;

    private Temperature(double kelvin) => this.kelvin = kelvin;

    // Absolute scales
    public static Temperature FromKelvin(double kelvin) => new Temperature(kelvin);
    public double ToKelvin() => kelvin;
    public static Temperature FromRankine(double rankine) => new Temperature(rankine * 5.0 / 9.0);
    public double ToRankine() => kelvin * 9.0 / 5.0;

    // Relative (offset) scales
    public static Temperature FromCelsius(double celsius) => new Temperature(celsius + 273.15);
    public double ToCelsius() => kelvin - 273.15;
    public static Temperature FromFahrenheit(double fahrenheit) => new Temperature((fahrenheit - 32.0) * 5.0 / 9.0 + 273.15);
    public double ToFahrenheit() => (kelvin - 273.15) * 9.0 / 5.0 + 32.0;

    // Historical scales
    public static Temperature FromReaumur(double reaumur) => new Temperature(reaumur * 5.0 / 4.0 + 273.15);
    public double ToReaumur() => (kelvin - 273.15) * 4.0 / 5.0;
    public static Temperature FromDelisle(double delisle) => new Temperature(373.15 - delisle * 2.0 / 3.0);
    public double ToDelisle() => (373.15 - kelvin) * 3.0 / 2.0;
    public static Temperature FromNewton(double newton) => new Temperature(newton * 100.0 / 33.0 + 273.15);
    public double ToNewton() => (kelvin - 273.15) * 33.0 / 100.0;
    public static Temperature FromRomer(double romer) => new Temperature((romer - 7.5) * 40.0 / 21.0 + 273.15);
    public double ToRomer() => (kelvin - 273.15) * 21.0 / 40.0 + 7.5;

    // Arithmetic. The canonical unit is kelvin, an absolute (true-zero) scale, so these
    // are well-defined; note a sum read back on an offset scale looks shifted
    // (0 °C + 0 °C = 273.15 °C, i.e. 546.30 K).
    public static Temperature operator +(Temperature a, Temperature b) => new Temperature(a.kelvin + b.kelvin);
    public static Temperature operator -(Temperature a, Temperature b) => new Temperature(a.kelvin - b.kelvin);
    public static Temperature operator -(Temperature x) => new Temperature(-x.kelvin);

    // Composite relationships
    public static Energy operator *(Temperature temperatureChange, HeatCapacity heatCapacity) => Energy.FromJoules(temperatureChange.kelvin * heatCapacity.ToJoulesPerKelvin());
    public static ThermalResistance operator /(Temperature temperatureChange, Power power) => ThermalResistance.FromKelvinsPerWatt(temperatureChange.kelvin / power.ToWatts());
    public static Power operator /(Temperature temperatureChange, ThermalResistance thermalResistance) => Power.FromWatts(temperatureChange.kelvin / thermalResistance.ToKelvinsPerWatt());
}
