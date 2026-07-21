namespace com.hafthor.Measurement;

public class ThermalResistance {
    private readonly double kelvinsPerWatt;

    private ThermalResistance(double kelvinsPerWatt) => this.kelvinsPerWatt = kelvinsPerWatt;

    // Arithmetic
    public static ThermalResistance operator +(ThermalResistance a, ThermalResistance b) => new(a.kelvinsPerWatt + b.kelvinsPerWatt);
    public static ThermalResistance operator -(ThermalResistance a, ThermalResistance b) => new(a.kelvinsPerWatt - b.kelvinsPerWatt);
    public static ThermalResistance operator -(ThermalResistance x) => new(-x.kelvinsPerWatt);

    // Units
    public static ThermalResistance FromKelvinsPerWatt(double kelvinsPerWatt) => new(kelvinsPerWatt);
    public double ToKelvinsPerWatt() => kelvinsPerWatt;

    // Composite relationships
    public static Temperature operator *(ThermalResistance thermalResistance, Power power) => Temperature.FromKelvin(thermalResistance.ToKelvinsPerWatt() * power.ToWatts());
    public static Temperature operator *(Power power, ThermalResistance thermalResistance) => Temperature.FromKelvin(power.ToWatts() * thermalResistance.ToKelvinsPerWatt());

    public override string ToString() => $"{kelvinsPerWatt} K/W";

    public override bool Equals(object obj) => obj is ThermalResistance other && other.kelvinsPerWatt == kelvinsPerWatt;
    public override int GetHashCode() => kelvinsPerWatt.GetHashCode();
}
