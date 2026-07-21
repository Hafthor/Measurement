namespace com.hafthor.Measurement;

[Measurement("K/W")]
public readonly partial struct ThermalResistance {

    // Units
    public static ThermalResistance FromKelvinsPerWatt(double kelvinsPerWatt) => new(kelvinsPerWatt);
    public double ToKelvinsPerWatt() => value;

    // Composite relationships
    public static Temperature operator *(ThermalResistance thermalResistance, Power power) => Temperature.FromKelvin(thermalResistance.ToKelvinsPerWatt() * power.ToWatts());
    public static Temperature operator *(Power power, ThermalResistance thermalResistance) => Temperature.FromKelvin(power.ToWatts() * thermalResistance.ToKelvinsPerWatt());

}
