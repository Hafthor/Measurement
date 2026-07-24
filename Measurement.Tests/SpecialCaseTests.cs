namespace com.hafthor.Measurement;

[TestClass]
public sealed class SpecialCaseTests {
    [TestMethod]
    public void Ratio_Fractions() {
        Assert.AreEqual(1e-2, Ratio.FromPercent(1).ToRatio());
        Assert.AreEqual(1e-6, Ratio.FromPartsPerMillion(1).ToRatio());
        Assert.AreEqual(2.5, Ratio.FromRatio(2.5).ToRatio());
    }

    // Decibels are logarithmic, so a floating tolerance is inherent.
    [TestMethod]
    public void Ratio_Decibels() {
        Assert.AreEqual(20.0, Ratio.FromDecibels(20).ToDecibels(), 1e-9);
        Assert.AreEqual(100.0, Ratio.FromDecibels(20).ToRatio(), 1e-9);
        Assert.AreEqual(0.0, Ratio.FromRatio(1).ToDecibels(), 1e-9);
    }

    [TestMethod]
    public void ResistanceConductance_AreReciprocal() {
        Assert.AreEqual(0.25, ElectricResistance.FromOhms(4).ToElectricConductance().ToSiemens());
        Assert.AreEqual(4.0, ElectricConductance.FromSiemens(0.25).ToElectricResistance().ToOhms());
        // round-trips back to itself
        Assert.AreEqual(50.0, ElectricResistance.FromOhms(50).ToElectricConductance().ToElectricResistance().ToOhms(), 1e-9);
    }

    [TestMethod]
    public void ResistivityConductivity_AreReciprocal() {
        Assert.AreEqual(0.25, Resistivity.FromOhmMeters(4).ToConductivity().ToSiemensPerMeter());
        Assert.AreEqual(4.0, Conductivity.FromSiemensPerMeter(0.25).ToResistivity().ToOhmMeters());
        Assert.AreEqual(50.0, Resistivity.FromOhmMeters(50).ToConductivity().ToResistivity().ToOhmMeters(), 1e-9);
    }

    [TestMethod]
    public void Concentration_MolarUnit() {
        Assert.AreEqual(1000.0, Concentration.FromMolesPerLiter(1).ToMolesPerCubicMeter());
        Assert.AreEqual(1.0, Concentration.FromMolesPerCubicMeter(1000).ToMolesPerLiter());
    }

    [TestMethod]
    public void Density_AlternateUnits() {
        Assert.AreEqual(1000.0, Density.FromGramsPerCubicCentimeter(1).ToKilogramsPerCubicMeter());
        Assert.AreEqual(1.0, Density.FromKilogramsPerCubicMeter(1000).ToKilogramsPerLiter());
    }

    [TestMethod]
    public void Duration_ExtremeScales() {
        Assert.AreEqual(5.391247e-44, Duration.FromPlanckTimes(1).ToSeconds());
        Assert.AreEqual(31557600000000000.0, Duration.FromGigaannums(1).ToSeconds());
        Assert.AreEqual(4.803349612e17, Duration.FromHubbleTimes(1).ToSeconds());
    }
}
