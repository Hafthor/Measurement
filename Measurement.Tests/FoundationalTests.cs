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
        Assert.AreEqual(1.616255e-26, Length.FromPlanckLengths(1).ToNanometers());
    }

    [TestMethod]
    public void Length_BaseRoundTrips() {
        foreach (var m in new[] { 0.0, 1.0, 1234.5, -42.0 })
            Assert.AreEqual(m, Length.FromMeters(m).ToMeters());
    }

    [TestMethod]
    public void Mass_BaseConversions() {
        Assert.AreEqual(453.59237, Mass.FromPounds(1).ToGrams());
        Assert.AreEqual(1_000_000.0, Mass.FromTonnes(1).ToGrams());
        Assert.AreEqual(28.349523125, Mass.FromOunces(1).ToGrams());
        Assert.AreEqual(6350.29318, Mass.FromStones(1).ToGrams());
        Assert.AreEqual(1.66053906660e-24, Mass.FromDaltons(1).ToGrams());
    }

    [TestMethod]
    public void MicroAnchor_MakesSubScaleValuesExact() {
        // Stored canonically as micrograms / microlitres, so decimals that would be inexact
        // against a gram / cubic-metre anchor become exact integer counts of the anchor unit.
        Assert.AreEqual(100.0, Mass.FromMilligrams(0.1).ToMicrograms());
        Assert.AreEqual(1.0, Mass.FromMicrograms(1).ToMicrograms());
        Assert.AreEqual(500.0, Mass.FromGrams(0.0005).ToMicrograms());
        Assert.AreEqual(100.0, Volume.FromMilliliters(0.1).ToMicroliters());
        Assert.AreEqual(1.0, Volume.FromMicroliters(1).ToMicroliters());
        Assert.AreEqual(250.0, Volume.FromLiters(0.00025).ToMicroliters());
        Assert.AreEqual(100.0, Length.FromMicrometers(0.1).ToNanometers());
        Assert.AreEqual(100.0, Capacitance.FromNanofarads(0.1).ToPicofarads());
        Assert.AreEqual(100.0, ElectricCurrent.FromMilliamperes(0.1).ToMicroamperes());
        Assert.AreEqual(100.0, MagneticFlux.FromMicrowebers(0.1).ToNanowebers());
        Assert.AreEqual(1.0, Ratio.FromPartsPerMillion(1).ToPartsPerMillion());
        Assert.AreEqual(250.0, Ratio.FromPercent(0.025).ToPartsPerMillion());
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
        // Count is the canonical unit: integer counts, pairs, dozens, gross are exact
        Assert.AreEqual(1000.0, Quantity.FromCount(1000).ToCount());
        Assert.AreEqual(6.0, Quantity.FromPairs(3).ToCount());
        Assert.AreEqual(12.0, Quantity.FromDozens(1).ToCount());
        Assert.AreEqual(144.0, Quantity.FromGross(1).ToCount());
        Assert.AreEqual(6.02214076e23, Quantity.FromMoles(1).ToCount());
        Assert.AreEqual(1.0, Quantity.FromCount(6.02214076e23).ToMoles());
        // Moles are carried as count / Avogadro, so mole round-trips are approximate
        Assert.AreEqual(1000.0, Quantity.FromKilomoles(1).ToMoles(), 1e-9);
        Assert.AreEqual(1e-3, Quantity.FromMillimoles(1).ToMoles(), 1e-15);
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
