namespace com.hafthor.Measurement;

[TestClass]
public sealed class TemperatureTests {
    [TestMethod]
    public void Arithmetic_IsUniformInKelvin() {
        // Kelvin is a true-zero scale, so Temperature adds/subtracts like any other unit.
        Assert.AreEqual(200.0, (Temperature.Of(100).Kelvin + Temperature.Of(100).Kelvin).To.Kelvin);
        Assert.AreEqual(20.0, (Temperature.Of(300).Kelvin - Temperature.Of(280).Kelvin).To.Kelvin);
        Assert.AreEqual(-100.0, (-Temperature.Of(100).Kelvin).To.Kelvin);
    }

    [TestMethod]
    public void OffsetScaleReadBackOfSum() {
        // 0 °C + 0 °C = 546.30 K = 273.15 °C (expected consequence of the offset scale).
        Temperature sum = Temperature.Of(0).Celsius + Temperature.Of(0).Celsius;
        Assert.AreEqual(546.30, sum.To.Kelvin, 1e-9);
        Assert.AreEqual(273.15, sum.To.Celsius, 1e-9);
    }

    [TestMethod]
    public void ScaleConversions() {
        Assert.AreEqual(273.15, Temperature.Of(0).Celsius.To.Kelvin);
        Assert.AreEqual(-273.15, Temperature.Of(0).Kelvin.To.Celsius);
        Assert.AreEqual(212.0, Temperature.Of(100).Celsius.To.Fahrenheit, 1e-9);
        Assert.AreEqual(0.0, Temperature.Of(0).Kelvin.To.Rankine);
    }

    // Q = m c ΔT — the definition behind the calorie. The "ΔT" is a Temperature in kelvin.
    [TestMethod]
    public void CalorieHeatEquation() {
        var waterC = SpecificHeatCapacity.Of(4184).Joules.Per.Kilo.Gram.Kelvin; // water
        HeatCapacity capacity = Mass.Of(1).Kilo.Grams * waterC;              // 4184 J/K
        Energy q = capacity * Temperature.Of(1).Kelvin;                     // heat 1 kg by 1 K
        Assert.AreEqual(4184.0, q.To.Joules);
        Assert.AreEqual(Energy.Of(1).Kilo.Calories.To.Joules, q.To.Joules); // exactly 1 kcal
        // inverse: ΔT = Q / (m c), and C = Q / ΔT
        Assert.AreEqual(1.0, (q / capacity).To.Kelvin);
        Assert.AreEqual(4184.0, (q / Temperature.Of(1).Kelvin).To.Joules.Per.Kelvin);
    }
}
