using System.Globalization;

namespace com.hafthor.Measurement;

[TestClass]
public sealed class BaseMathTests {
    [TestMethod]
    public void ScalarMultiplyAndDivide() {
        Assert.AreEqual(15.0, (Length.Of(5).Meters * 3).To.Meters);
        Assert.AreEqual(15.0, (3 * Length.Of(5).Meters).To.Meters);
        Assert.AreEqual(2.5, (Length.Of(5).Meters / 2).To.Meters);
    }

    [TestMethod]
    public void SameTypeDivisionIsDimensionlessRatio() {
        double ratio = Length.Of(1).Kilo.Meters / Length.Of(250).Meters;
        Assert.AreEqual(4.0, ratio);
    }

    [TestMethod]
    public void AbsMinMaxClampLerp() {
        Assert.AreEqual(5.0, (-Length.Of(5).Meters).Abs().To.Meters);
        Assert.AreEqual(2.0, Length.Of(2).Meters.Min(Length.Of(7).Meters).To.Meters);
        Assert.AreEqual(7.0, Length.Of(2).Meters.Max(Length.Of(7).Meters).To.Meters);
        Assert.AreEqual(0.0, (-Length.Of(5).Meters).Clamp(Length.Of(0).Meters, Length.Of(10).Meters).To.Meters);
        Assert.AreEqual(3.0, Length.Of(3).Meters.Clamp(Length.Of(0).Meters, Length.Of(10).Meters).To.Meters);
        // static Lerp is inherited, callable on the concrete type
        Assert.AreEqual(5.0, Length.Lerp(Length.Of(0).Meters, Length.Of(10).Meters, 0.5).To.Meters);
    }

    [TestMethod]
    public void Equatable_NoBoxing() {
        Assert.IsTrue(Length.Of(1000).Meters.Equals(Length.Of(1).Kilo.Meters));
        Assert.IsFalse(Length.Of(1).Meters.Equals(Length.Of(2).Meters));
    }

    [TestMethod]
    public void Formattable() {
        Assert.AreEqual("5.00 m", Length.Of(5).Meters.ToString("F2", CultureInfo.InvariantCulture));
        Assert.AreEqual("5.00", Ratio.Of(5).Ratio.ToString("F2", CultureInfo.InvariantCulture));
        Assert.AreEqual("1,000 m", Length.Of(1000).Meters.ToString("N0", CultureInfo.InvariantCulture));
    }
}
