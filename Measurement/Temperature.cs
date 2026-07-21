namespace com.hafthor.Measurement;

[Measurement("K")]
public readonly partial struct Temperature {

    // Absolute scales
    public static Temperature FromKelvin(double kelvin) => new(kelvin);
    public double ToKelvin() => value;
    public static Temperature FromRankine(double rankine) => new(rankine * 5.0 / 9.0);
    public double ToRankine() => value * 9.0 / 5.0;

    // Relative (offset) scales
    public static Temperature FromCelsius(double celsius) => new(celsius + 273.15);
    public double ToCelsius() => value - 273.15;
    public static Temperature FromFahrenheit(double fahrenheit) => new((fahrenheit - 32.0) * 5.0 / 9.0 + 273.15);
    public double ToFahrenheit() => (value - 273.15) * 9.0 / 5.0 + 32.0;

    // Historical scales
    public static Temperature FromReaumur(double reaumur) => new(reaumur * 5.0 / 4.0 + 273.15);
    public double ToReaumur() => (value - 273.15) * 4.0 / 5.0;
    public static Temperature FromDelisle(double delisle) => new(373.15 - delisle * 2.0 / 3.0);
    public double ToDelisle() => (373.15 - value) * 3.0 / 2.0;
    public static Temperature FromNewton(double newton) => new(newton * 100.0 / 33.0 + 273.15);
    public double ToNewton() => (value - 273.15) * 33.0 / 100.0;
    public static Temperature FromRomer(double romer) => new((romer - 7.5) * 40.0 / 21.0 + 273.15);
    public double ToRomer() => (value - 273.15) * 21.0 / 40.0 + 7.5;

    // Uses the inherited +/- operators: the canonical unit is kelvin, an absolute (true-zero)
    // scale, so they are well-defined; note a sum read back on an offset scale looks shifted
    // (0 °C + 0 °C = 273.15 °C, i.e. 546.30 K).

    // Composite relationships
    public static Energy operator *(Temperature temperatureChange, HeatCapacity heatCapacity) => Energy.FromJoules(temperatureChange.value * heatCapacity.ToJoulesPerKelvin());
    public static ThermalResistance operator /(Temperature temperatureChange, Power power) => ThermalResistance.FromKelvinsPerWatt(temperatureChange.value / power.ToWatts());
    public static Power operator /(Temperature temperatureChange, ThermalResistance thermalResistance) => Power.FromWatts(temperatureChange.value / thermalResistance.ToKelvinsPerWatt());

}
