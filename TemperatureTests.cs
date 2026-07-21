namespace com.hafthor.Measurement;

[TestClass]
public sealed class TemperatureTests {
    [TestMethod]
    public void Arithmetic_IsUniformInKelvin() {
        // Kelvin is a true-zero scale, so Temperature adds/subtracts like any other unit.
        Assert.AreEqual(200.0, (Temperature.FromKelvin(100) + Temperature.FromKelvin(100)).ToKelvin());
        Assert.AreEqual(20.0, (Temperature.FromKelvin(300) - Temperature.FromKelvin(280)).ToKelvin());
        Assert.AreEqual(-100.0, (-Temperature.FromKelvin(100)).ToKelvin());
    }

    [TestMethod]
    public void OffsetScaleReadBackOfSum() {
        // 0 °C + 0 °C = 546.30 K = 273.15 °C (expected consequence of the offset scale).
        Temperature sum = Temperature.FromCelsius(0) + Temperature.FromCelsius(0);
        Assert.AreEqual(546.30, sum.ToKelvin(), 1e-9);
        Assert.AreEqual(273.15, sum.ToCelsius(), 1e-9);
    }

    [TestMethod]
    public void ScaleConversions() {
        Assert.AreEqual(273.15, Temperature.FromCelsius(0).ToKelvin());
        Assert.AreEqual(-273.15, Temperature.FromKelvin(0).ToCelsius());
        Assert.AreEqual(212.0, Temperature.FromCelsius(100).ToFahrenheit(), 1e-9);
        Assert.AreEqual(0.0, Temperature.FromKelvin(0).ToRankine());
    }

    // Q = m c ΔT — the definition behind the calorie. The "ΔT" is a Temperature in kelvin.
    [TestMethod]
    public void CalorieHeatEquation() {
        var waterC = SpecificHeatCapacity.FromJoulesPerKilogramKelvin(4184); // water
        HeatCapacity capacity = Mass.FromKilograms(1) * waterC;              // 4184 J/K
        Energy q = capacity * Temperature.FromKelvin(1);                     // heat 1 kg by 1 K
        Assert.AreEqual(4184.0, q.ToJoules());
        Assert.AreEqual(Energy.FromKilocalories(1).ToJoules(), q.ToJoules()); // exactly 1 kcal
        // inverse: ΔT = Q / (m c), and C = Q / ΔT
        Assert.AreEqual(1.0, (q / capacity).ToKelvin());
        Assert.AreEqual(4184.0, (q / Temperature.FromKelvin(1)).ToJoulesPerKelvin());
    }
}
