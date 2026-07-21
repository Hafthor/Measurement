namespace com.hafthor.Measurement;

[TestClass]
public sealed class ToStringTests {
    [TestMethod]
    public void RendersValueWithCanonicalSymbol() {
        Assert.AreEqual("5 m", Length.FromMeters(5).ToString());
        Assert.AreEqual("2 kg", Mass.FromKilograms(2).ToString());
        Assert.AreEqual("300 K", Temperature.FromKelvin(300).ToString());
        Assert.AreEqual("6 N", Force.FromNewtons(6).ToString());
        Assert.AreEqual("100 J", Energy.FromJoules(100).ToString());
        Assert.AreEqual("10 m/s", Speed.FromMetersPerSecond(10).ToString());
        Assert.AreEqual("9 m/s²", Acceleration.FromMetersPerSecondSquared(9).ToString());
        Assert.AreEqual("50 Ω", ElectricResistance.FromOhms(50).ToString());
        Assert.AreEqual("4184 J/(kg·K)", SpecificHeatCapacity.FromJoulesPerKilogramKelvin(4184).ToString());
    }

    [TestMethod]
    public void ConvertsToCanonicalUnitFirst() {
        // 1 km prints as its canonical metres value
        Assert.AreEqual("1000 m", Length.FromKilometers(1).ToString());
    }

    [TestMethod]
    public void DimensionlessRatioHasNoSymbol() {
        Assert.AreEqual("3", Ratio.FromRatio(3).ToString());
    }
}
