namespace com.hafthor.Measurement;

[Measurement("K/W", VariableName = "kelvinsPerWatt")]
[SiUnit("KelvinsPerWatt", 0)]
public readonly partial struct ThermalResistance {
    // Composite relationships
    public static Temperature operator *(ThermalResistance thermalResistance, Power power) => Temperature.FromKelvin(thermalResistance.ToKelvinsPerWatt() * power.ToWatts());
    public static Temperature operator *(Power power, ThermalResistance thermalResistance) => Temperature.FromKelvin(power.ToWatts() * thermalResistance.ToKelvinsPerWatt());
}
