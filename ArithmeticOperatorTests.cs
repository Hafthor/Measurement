namespace com.hafthor.Measurement;

[TestClass]
public sealed class ArithmeticOperatorTests {
    [TestMethod]
    public void Addition_Subtraction_Negation() {
        Assert.AreEqual(7.0, (Length.FromMeters(3) + Length.FromMeters(4)).ToMeters());
        Assert.AreEqual(1.0, (Length.FromMeters(4) - Length.FromMeters(3)).ToMeters());
        Assert.AreEqual(-5.0, (-Length.FromMeters(5)).ToMeters());
    }

    [TestMethod]
    public void Addition_UsesCanonicalRegardlessOfUnit() {
        var total = Mass.FromKilograms(1) + Mass.FromGrams(500);
        Assert.AreEqual(1.5, total.ToKilograms());
    }

    [TestMethod]
    public void Arithmetic_AcrossSeveralTypes() {
        Assert.AreEqual(9.0, (Energy.FromJoules(4) + Energy.FromJoules(5)).ToJoules());
        Assert.AreEqual(2.0, (Force.FromNewtons(5) - Force.FromNewtons(3)).ToNewtons());
        Assert.AreEqual(-3.0, (-Duration.FromSeconds(3)).ToSeconds());
        Assert.AreEqual(6.0, (Voltage.FromVolts(2) + Voltage.FromVolts(4)).ToVolts());
    }
}
