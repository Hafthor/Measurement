namespace com.hafthor.Measurement;

// Conversions are a single multiply/divide against the base unit, so
// From<Unit>(1).To<Base>() equals the factor bit-for-bit: assert exact equality.
[TestClass]
public sealed class FoundationalTests {
    [TestMethod]
    public void Length_BaseConversions() {
        Assert.AreEqual(1609.344, Length.Of(1).Miles.To.Meters);
        Assert.AreEqual(0.3048, Length.Of(1).Feet.To.Meters);
        Assert.AreEqual(0.0254, Length.Of(1).Inches.To.Meters);
        Assert.AreEqual(1000.0, Length.Of(1).Kilo.Meters.To.Meters);
        Assert.AreEqual(9_460_730_472_580_800.0, Length.Of(1).Light.Years.To.Meters);
        // white-box: bit-exact stored value (a prefixed fluent read would add last-ULP FP noise).
        // Accessed via reflection so the tests need no InternalsVisibleTo to the unit methods.
        Assert.AreEqual(1.616255e-26, MeasurementReflection.Convert(typeof(Length), "PlanckLengths", 1, "Nanometers"));
    }

    [TestMethod]
    public void Length_BaseRoundTrips() {
        foreach (var m in new[] { 0.0, 1.0, 1234.5, -42.0 })
            Assert.AreEqual(m, Length.Of(m).Meters.To.Meters);
    }

    [TestMethod]
    public void Mass_BaseConversions() {
        Assert.AreEqual(453.59237, Mass.Of(1).Pounds.To.Grams);
        Assert.AreEqual(1_000_000.0, Mass.Of(1).Tonnes.To.Grams);
        Assert.AreEqual(28.349523125, Mass.Of(1).Ounces.To.Grams);
        Assert.AreEqual(6350.29318, Mass.Of(1).Stones.To.Grams);
        Assert.AreEqual(1.66053906660e-24, Mass.Of(1).Daltons.To.Grams);
    }

    [TestMethod]
    public void MicroAnchor_MakesSubScaleValuesExact() {
        // white-box: asserts bit-exact internal storage (micro/nano anchors). The exactness is a
        // storage property, so it reads the exact stored value via reflection (a prefixed fluent read
        // like .To.Micro.Grams divides twice and would introduce last-ULP noise). Reflection reaches
        // the internal unit methods without InternalsVisibleTo.
        var C = MeasurementReflection.Convert;
        Assert.AreEqual(100.0, C(typeof(Mass), "Milligrams", 0.1, "Micrograms"));
        Assert.AreEqual(1.0, C(typeof(Mass), "Micrograms", 1, "Micrograms"));
        Assert.AreEqual(500.0, C(typeof(Mass), "Grams", 0.0005, "Micrograms"));
        Assert.AreEqual(100.0, C(typeof(Volume), "Milliliters", 0.1, "Microliters"));
        Assert.AreEqual(1.0, C(typeof(Volume), "Microliters", 1, "Microliters"));
        Assert.AreEqual(250.0, C(typeof(Volume), "Liters", 0.00025, "Microliters"));
        Assert.AreEqual(100.0, C(typeof(Length), "Micrometers", 0.1, "Nanometers"));
        Assert.AreEqual(100.0, C(typeof(Capacitance), "Nanofarads", 0.1, "Picofarads"));
        Assert.AreEqual(100.0, C(typeof(ElectricCurrent), "Milliamperes", 0.1, "Microamperes"));
        Assert.AreEqual(100.0, C(typeof(MagneticFlux), "Microwebers", 0.1, "Nanowebers"));
        Assert.AreEqual(1.0, C(typeof(Ratio), "PartsPerMillion", 1, "PartsPerMillion"));
        Assert.AreEqual(250.0, C(typeof(Ratio), "Percent", 0.025, "PartsPerMillion"));
    }

    [TestMethod]
    public void Duration_BaseConversions() {
        Assert.AreEqual(60.0, Duration.Of(1).Minutes.To.Seconds);
        Assert.AreEqual(3600.0, Duration.Of(1).Hours.To.Seconds);
        Assert.AreEqual(86400.0, Duration.Of(1).Days.To.Seconds);
        Assert.AreEqual(31557600.0, Duration.Of(1).Julian.Years.To.Seconds);
        Assert.AreEqual(5.391247e-44, Duration.Of(1).Planck.Times.To.Seconds);
    }

    [TestMethod]
    public void ElectricCurrent_BaseConversions() {
        Assert.AreEqual(10.0, ElectricCurrent.Of(1).Abamperes.To.Amperes);
        Assert.AreEqual(1000.0, ElectricCurrent.Of(1).Kilo.Amperes.To.Amperes);
        Assert.AreEqual(1e-3, ElectricCurrent.Of(1).Milli.Amperes.To.Amperes);
    }

    [TestMethod]
    public void Quantity_BaseConversions() {
        // Count is the canonical unit: integer counts, pairs, dozens, gross are exact
        Assert.AreEqual(1000.0, Quantity.Of(1000).Count.To.Count);
        Assert.AreEqual(6.0, Quantity.Of(3).Pairs.To.Count);
        Assert.AreEqual(12.0, Quantity.Of(1).Dozens.To.Count);
        Assert.AreEqual(144.0, Quantity.Of(1).Gross.To.Count);
        Assert.AreEqual(6.02214076e23, Quantity.Of(1).Moles.To.Count);
        Assert.AreEqual(1.0, Quantity.Of(6.02214076e23).Count.To.Moles);
        // Moles are carried as count / Avogadro, so mole round-trips are approximate
        Assert.AreEqual(1000.0, Quantity.Of(1).Kilo.Moles.To.Moles, 1e-9);
        Assert.AreEqual(1e-3, Quantity.Of(1).Milli.Moles.To.Moles, 1e-15);
    }

    [TestMethod]
    public void LuminousIntensity_BaseConversions() {
        Assert.AreEqual(1000.0, LuminousIntensity.Of(1).Kilo.Candelas.To.Candelas);
        Assert.AreEqual(0.981, LuminousIntensity.Of(1).Candlepower.To.Candelas);
        Assert.AreEqual(0.903, LuminousIntensity.Of(1).Hefnerkerze.To.Candelas);
    }

    // Temperature is an absolute (offset) scale: conversions across scales involve a
    // 273.15 shift, so they are only accurate to floating-point tolerance.
    [TestMethod]
    public void Temperature_ExactOffsets() {
        Assert.AreEqual(273.15, Temperature.Of(0).Celsius.To.Kelvin, 1e-12);
        Assert.AreEqual(-273.15, Temperature.Of(0).Kelvin.To.Celsius);
        Assert.AreEqual(32.0, Temperature.Of(0).Celsius.To.Fahrenheit, 1e-12);
        Assert.AreEqual(0.0, Temperature.Of(32).Fahrenheit.To.Celsius);
        Assert.AreEqual(0.0, Temperature.Of(0).Kelvin.To.Rankine);
        foreach (var k in new[] { 0.0, 200.0, 273.15, 500.0 })
            Assert.AreEqual(k, Temperature.Of(k).Kelvin.To.Kelvin);
    }

    [TestMethod]
    public void Temperature_ScaleConversions() {
        Assert.AreEqual(212.0, Temperature.Of(100).Celsius.To.Fahrenheit, 1e-9);
        Assert.AreEqual(100.0, Temperature.Of(212).Fahrenheit.To.Celsius, 1e-9);
        Assert.AreEqual(671.67, Temperature.Of(212).Fahrenheit.To.Rankine, 1e-9);
    }
}
