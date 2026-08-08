namespace com.hafthor.Measurement;

[TestClass]
public sealed class ToStringTests {
    [TestMethod]
    public void RendersValueWithCanonicalSymbol() {
        Assert.AreEqual("5 m", Length.Of(5).Meters.ToString());
        Assert.AreEqual("2000 g", Mass.Of(2).Kilo.Grams.ToString());
        Assert.AreEqual("5 g", Mass.Of(5).Grams.ToString());
        Assert.AreEqual("1 m³", Volume.Of(1).Cubic.Meters.ToString());
        Assert.AreEqual("0.005 m³", Volume.Of(5).Liters.ToString());
        Assert.AreEqual("5 V", Voltage.Of(5).Volts.ToString());
        Assert.AreEqual("1000", Quantity.Of(1000).Count.ToString());
        Assert.AreEqual("5E-06 F", Capacitance.Of(5).Micro.Farads.ToString());
        Assert.AreEqual("300 K", Temperature.Of(300).Kelvin.ToString());
        Assert.AreEqual("6 N", Force.Of(6).Newtons.ToString());
        Assert.AreEqual("100 J", Energy.Of(100).Joules.ToString());
        Assert.AreEqual("10 m/s", Speed.Of(10).Meters.Per.Second.ToString());
        Assert.AreEqual("9 m/s²", Acceleration.Of(9).Meters.Per.Second.Squared.ToString());
        Assert.AreEqual("50 Ω", ElectricResistance.Of(50).Ohms.ToString());
        Assert.AreEqual("4.184 J/(g·K)", SpecificHeatCapacity.Of(4184).Joules.Per.Kilo.Gram.Kelvin.ToString());
        Assert.AreEqual("1 W/m²", HeatFluxDensity.Of(1).Watts.Per.Square.Meter.ToString());
        Assert.AreEqual("2 Ω·m", Resistivity.Of(2).Ohm.Meters.ToString());
    }

    [TestMethod]
    public void ConvertsToCanonicalUnitFirst() {
        // 1 km prints as its canonical metres value
        Assert.AreEqual("1000 m", Length.Of(1).Kilo.Meters.ToString());
    }

    [TestMethod]
    public void DimensionlessRatioHasNoSymbol() {
        Assert.AreEqual("3", Ratio.Of(3).Ratio.ToString());
    }
}
