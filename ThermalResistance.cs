namespace com.hafthor.Measurement;

public sealed class ThermalResistance : Measurement<ThermalResistance> {

    private ThermalResistance(double value) : base(value) { }

    protected override ThermalResistance Create(double value) => new(value);
    protected override string Symbol => "K/W";

    // Units
    public static ThermalResistance FromKelvinsPerWatt(double value) => new(value);
    public double ToKelvinsPerWatt() => value;

    // Composite relationships
    public static Temperature operator *(ThermalResistance thermalResistance, Power power) => Temperature.FromKelvin(thermalResistance.ToKelvinsPerWatt() * power.ToWatts());
    public static Temperature operator *(Power power, ThermalResistance thermalResistance) => Temperature.FromKelvin(power.ToWatts() * thermalResistance.ToKelvinsPerWatt());

}
