namespace com.hafthor.Measurement;

// Canonical unit is kelvin. Non-kelvin scales are affine: anchor = (value + PreOffset) * Factor +
// Offset. Subtracting the pre-offset before scaling keeps offset-cancelling conversions exact
// (e.g. 32 °F → 0 °C). Uses the inherited +/- operators: kelvin is an absolute (true-zero) scale,
// so they are well-defined; note a sum read on an offset scale looks shifted (0 °C + 0 °C = 546.30 K).
[Measurement("K", VariableName = "ninthsOfKelvin", DisplayFactor = 9)]
[Unit("Kelvin", 9.0)]
[Unit("Rankine", 5.0)]
[Unit("Celsius", 9.0, Offset = 273.15)]
[Unit("Fahrenheit", 5.0, Offset = 459.67)]
public readonly partial struct Temperature {
    // Composite relationships
    public static Energy operator *(Temperature temperatureChange, HeatCapacity heatCapacity) => Energy.FromJoules(temperatureChange.ToKelvin() * heatCapacity.ToJoulesPerKelvin());
    public static ThermalResistance operator /(Temperature temperatureChange, Power power) => ThermalResistance.FromKelvinsPerWatt(temperatureChange.ToKelvin() / power.ToWatts());
    public static Power operator /(Temperature temperatureChange, ThermalResistance thermalResistance) => Power.FromWatts(temperatureChange.ToKelvin() / thermalResistance.ToKelvinsPerWatt());
}
