namespace com.hafthor.Measurement;

[TestClass]
public sealed class SpecialCaseTests {
    [TestMethod]
    public void Ratio_Fractions() {
        Assert.AreEqual(1e-2, Ratio.Of(1).Percent.To.Ratio);
        Assert.AreEqual(1e-6, Ratio.Of(1).Parts.Per.Million.To.Ratio);
        Assert.AreEqual(2.5, Ratio.Of(2.5).Ratio.To.Ratio);
    }

    // Decibels are logarithmic, so a floating tolerance is inherent.
    [TestMethod]
    public void Ratio_Decibels() {
        Assert.AreEqual(20.0, Ratio.Of(20).Decibels.To.Decibels, 1e-9);
        Assert.AreEqual(100.0, Ratio.Of(20).Decibels.To.Ratio, 1e-9);
        Assert.AreEqual(0.0, Ratio.Of(1).Ratio.To.Decibels, 1e-9);
    }

    [TestMethod]
    public void Exposure_GramCanonicalAndRoentgen() {
        // canonical is C/g: 1 C/kg = 1e-3 C/g
        Assert.AreEqual(1e-3, Exposure.Of(1).Coulombs.Per.Kilo.Gram.To.Coulombs.Per.Gram);
        Assert.AreEqual(1000.0, Exposure.Of(1).Coulombs.Per.Gram.To.Coulombs.Per.Kilo.Gram);
        Assert.AreEqual("0.001 C/g", Exposure.Of(1).Coulombs.Per.Kilo.Gram.ToString());
        // 1 roentgen = 2.58e-4 C/kg
        Assert.AreEqual(2.58e-4, Exposure.Of(1).Roentgens.To.Coulombs.Per.Kilo.Gram, 1e-16);
        // Exposure × Mass = Charge:  2 C/kg over 3 kg = 6 C
        Assert.AreEqual(6.0, (Exposure.Of(2).Coulombs.Per.Kilo.Gram * Mass.Of(3).Kilo.Grams).To.Coulombs, 1e-9);
    }

    [TestMethod]
    public void ResistanceConductance_AreReciprocal() {
        Assert.AreEqual(0.25, ElectricResistance.Of(4).Ohms.To.ElectricConductance.To.Siemens);
        Assert.AreEqual(4.0, ElectricConductance.Of(0.25).Siemens.To.ElectricResistance.To.Ohms);
        // round-trips back to itself
        Assert.AreEqual(50.0, ElectricResistance.Of(50).Ohms.To.ElectricConductance.To.ElectricResistance.To.Ohms, 1e-9);
    }

    [TestMethod]
    public void ResistivityConductivity_AreReciprocal() {
        Assert.AreEqual(0.25, Resistivity.Of(4).Ohm.Meters.To.Conductivity.To.Siemens.Per.Meter);
        Assert.AreEqual(4.0, Conductivity.Of(0.25).Siemens.Per.Meter.To.Resistivity.To.OhmMeters);
        Assert.AreEqual(50.0, Resistivity.Of(50).Ohm.Meters.To.Conductivity.To.Resistivity.To.OhmMeters, 1e-9);
    }

    [TestMethod]
    public void Concentration_MolarUnit() {
        Assert.AreEqual(1000.0, Concentration.Of(1).Moles.Per.Liter.To.Moles.Per.Cubic.Meter);
        Assert.AreEqual(1.0, Concentration.Of(1000).Moles.Per.Cubic.Meter.To.Moles.Per.Liter);
    }

    [TestMethod]
    public void Density_AlternateUnits() {
        Assert.AreEqual(1000.0, Density.Of(1).Grams.Per.Cubic.Centi.Meter.To.Kilo.Grams.Per.Cubic.Meter);
        Assert.AreEqual(1.0, Density.Of(1000).Kilo.Grams.Per.Cubic.Meter.To.Kilo.Grams.Per.Liter);
    }

    [TestMethod]
    public void Duration_ExtremeScales() {
        Assert.AreEqual(5.391247e-44, Duration.Of(1).Planck.Times.To.Seconds);
        Assert.AreEqual(31557600000000000.0, Measure.Of(1).Giga.Annums.To.Seconds);
        Assert.AreEqual(4.803349612e17, Duration.Of(1).Hubble.Times.To.Seconds);
    }
}
