using System.Globalization;

namespace com.hafthor.Measurement;

[TestClass]
public sealed class BaseMathTests {
    [TestMethod]
    public void ScalarMultiplyAndDivide() {
        Assert.AreEqual(15.0, (Length.FromMeters(5) * 3).ToMeters());
        Assert.AreEqual(15.0, (3 * Length.FromMeters(5)).ToMeters());
        Assert.AreEqual(2.5, (Length.FromMeters(5) / 2).ToMeters());
    }

    [TestMethod]
    public void SameTypeDivisionIsDimensionlessRatio() {
        double ratio = Length.FromKilometers(1) / Length.FromMeters(250);
        Assert.AreEqual(4.0, ratio);
    }

    [TestMethod]
    public void AbsMinMaxClampLerp() {
        Assert.AreEqual(5.0, (-Length.FromMeters(5)).Abs().ToMeters());
        Assert.AreEqual(2.0, Length.FromMeters(2).Min(Length.FromMeters(7)).ToMeters());
        Assert.AreEqual(7.0, Length.FromMeters(2).Max(Length.FromMeters(7)).ToMeters());
        Assert.AreEqual(0.0, (-Length.FromMeters(5)).Clamp(Length.FromMeters(0), Length.FromMeters(10)).ToMeters());
        Assert.AreEqual(3.0, Length.FromMeters(3).Clamp(Length.FromMeters(0), Length.FromMeters(10)).ToMeters());
        // static Lerp is inherited, callable on the concrete type
        Assert.AreEqual(5.0, Length.Lerp(Length.FromMeters(0), Length.FromMeters(10), 0.5).ToMeters());
    }

    [TestMethod]
    public void Equatable_NoBoxing() {
        Assert.IsTrue(Length.FromMeters(1000).Equals(Length.FromKilometers(1)));
        Assert.IsFalse(Length.FromMeters(1).Equals(Length.FromMeters(2)));
    }

    [TestMethod]
    public void Formattable() {
        Assert.AreEqual("5.00 m", Length.FromMeters(5).ToString("F2", CultureInfo.InvariantCulture));
        Assert.AreEqual("5.00", Ratio.FromRatio(5).ToString("F2", CultureInfo.InvariantCulture));
        Assert.AreEqual("1,000 m", Length.FromMeters(1000).ToString("N0", CultureInfo.InvariantCulture));
    }
}
