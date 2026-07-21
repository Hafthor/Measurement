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
