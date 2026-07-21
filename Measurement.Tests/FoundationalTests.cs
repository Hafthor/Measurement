namespace com.hafthor.Measurement;

// Conversions are a single multiply/divide against the base unit, so
// From<Unit>(1).To<Base>() equals the factor bit-for-bit: assert exact equality.
[TestClass]
public sealed class FoundationalTests {
    [TestMethod]
    public void Length_BaseConversions() {
        Assert.AreEqual(1609.344, Length.FromMiles(1).ToMeters());
        Assert.AreEqual(0.3048, Length.FromFeet(1).ToMeters());
        Assert.AreEqual(0.0254, Length.FromInches(1).ToMeters());
        Assert.AreEqual(1000.0, Length.FromKilometers(1).ToMeters());
        Assert.AreEqual(9_460_730_472_580_800.0, Length.FromLightYears(1).ToMeters());
        Assert.AreEqual(1.616255e-35, Length.FromPlanckLengths(1).ToMeters());
    }

    [TestMethod]
    public void Length_BaseRoundTrips() {
        foreach (var m in new[] { 0.0, 1.0, 1234.5, -42.0 })
            Assert.AreEqual(m, Length.FromMeters(m).ToMeters());
    }

    [TestMethod]
    public void Mass_BaseConversions() {
        Assert.AreEqual(0.45359237, Mass.FromPounds(1).ToKilograms());
        Assert.AreEqual(1000.0, Mass.FromTonnes(1).ToKilograms());
        Assert.AreEqual(0.028349523125, Mass.FromOunces(1).ToKilograms());
        Assert.AreEqual(6.35029318, Mass.FromStones(1).ToKilograms());
        Assert.AreEqual(1.66053906660e-27, Mass.FromDaltons(1).ToKilograms());
    }

    [TestMethod]
    public void Duration_BaseConversions() {
        Assert.AreEqual(60.0, Duration.FromMinutes(1).ToSeconds());
        Assert.AreEqual(3600.0, Duration.FromHours(1).ToSeconds());
        Assert.AreEqual(86400.0, Duration.FromDays(1).ToSeconds());
        Assert.AreEqual(31557600.0, Duration.FromJulianYears(1).ToSeconds());
        Assert.AreEqual(5.391247e-44, Duration.FromPlanckTimes(1).ToSeconds());
    }

    [TestMethod]
    public void ElectricCurrent_BaseConversions() {
        Assert.AreEqual(10.0, ElectricCurrent.FromAbamperes(1).ToAmperes());
        Assert.AreEqual(1000.0, ElectricCurrent.FromKiloamperes(1).ToAmperes());
        Assert.AreEqual(1e-3, ElectricCurrent.FromMilliamperes(1).ToAmperes());
    }

    [TestMethod]
    public void Quantity_BaseConversions() {
        Assert.AreEqual(1000.0, Quantity.FromKilomoles(1).ToMoles());
        Assert.AreEqual(1e-3, Quantity.FromMillimoles(1).ToMoles());
        Assert.AreEqual(6.02214076e23, Quantity.FromMoles(1).ToCount());
        Assert.AreEqual(1.0, Quantity.FromCount(6.02214076e23).ToMoles());
    }

    [TestMethod]
    public void LuminousIntensity_BaseConversions() {
        Assert.AreEqual(1000.0, LuminousIntensity.FromKilocandelas(1).ToCandelas());
        Assert.AreEqual(0.981, LuminousIntensity.FromCandlepower(1).ToCandelas());
        Assert.AreEqual(0.903, LuminousIntensity.FromHefnerkerze(1).ToCandelas());
    }

    // Temperature is an absolute (offset) scale: conversions across scales involve a
    // 273.15 shift, so they are only accurate to floating-point tolerance.
    [TestMethod]
    public void Temperature_ExactOffsets() {
        Assert.AreEqual(273.15, Temperature.FromCelsius(0).ToKelvin());
        Assert.AreEqual(-273.15, Temperature.FromKelvin(0).ToCelsius());
        Assert.AreEqual(32.0, Temperature.FromCelsius(0).ToFahrenheit());
        Assert.AreEqual(0.0, Temperature.FromFahrenheit(32).ToCelsius());
        Assert.AreEqual(0.0, Temperature.FromKelvin(0).ToRankine());
        foreach (var k in new[] { 0.0, 200.0, 273.15, 500.0 })
            Assert.AreEqual(k, Temperature.FromKelvin(k).ToKelvin());
    }

    [TestMethod]
    public void Temperature_ScaleConversions() {
        Assert.AreEqual(212.0, Temperature.FromCelsius(100).ToFahrenheit(), 1e-9);
        Assert.AreEqual(100.0, Temperature.FromFahrenheit(212).ToCelsius(), 1e-9);
        Assert.AreEqual(80.0, Temperature.FromCelsius(100).ToReaumur(), 1e-9);
        Assert.AreEqual(671.67, Temperature.FromFahrenheit(212).ToRankine(), 1e-9);
    }
}
