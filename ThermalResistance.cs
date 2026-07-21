namespace com.hafthor.Measurement;

public class ThermalResistance {
    private readonly double kelvinsPerWatt;

    private ThermalResistance(double kelvinsPerWatt) => this.kelvinsPerWatt = kelvinsPerWatt;

    // Arithmetic
    public static ThermalResistance operator +(ThermalResistance a, ThermalResistance b) => new ThermalResistance(a.kelvinsPerWatt + b.kelvinsPerWatt);
    public static ThermalResistance operator -(ThermalResistance a, ThermalResistance b) => new ThermalResistance(a.kelvinsPerWatt - b.kelvinsPerWatt);
    public static ThermalResistance operator -(ThermalResistance x) => new ThermalResistance(-x.kelvinsPerWatt);

    // Units
    public static ThermalResistance FromKelvinsPerWatt(double kelvinsPerWatt) => new ThermalResistance(kelvinsPerWatt);
    public double ToKelvinsPerWatt() => kelvinsPerWatt;

    // Composite relationships
    public static Temperature operator *(ThermalResistance thermalResistance, Power power) => Temperature.FromKelvin(thermalResistance.ToKelvinsPerWatt() * power.ToWatts());
    public static Temperature operator *(Power power, ThermalResistance thermalResistance) => Temperature.FromKelvin(power.ToWatts() * thermalResistance.ToKelvinsPerWatt());
}
