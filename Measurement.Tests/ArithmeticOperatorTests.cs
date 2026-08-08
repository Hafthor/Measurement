namespace com.hafthor.Measurement;

[TestClass]
public sealed class ArithmeticOperatorTests {
    [TestMethod]
    public void Addition_Subtraction_Negation() {
        Assert.AreEqual(7.0, (Length.Of(3).Meters + Length.Of(4).Meters).To.Meters);
        Assert.AreEqual(1.0, (Length.Of(4).Meters - Length.Of(3).Meters).To.Meters);
        Assert.AreEqual(-5.0, (-Length.Of(5).Meters).To.Meters);
    }

    [TestMethod]
    public void Addition_UsesCanonicalRegardlessOfUnit() {
        var total = Mass.Of(1).Kilo.Grams + Mass.Of(500).Grams;
        Assert.AreEqual(1.5, total.To.Kilo.Grams);
    }

    [TestMethod]
    public void Arithmetic_AcrossSeveralTypes() {
        Assert.AreEqual(9.0, (Energy.Of(4).Joules + Energy.Of(5).Joules).To.Joules);
        Assert.AreEqual(2.0, (Force.Of(5).Newtons - Force.Of(3).Newtons).To.Newtons);
        Assert.AreEqual(-3.0, (-Duration.Of(3).Seconds).To.Seconds);
        Assert.AreEqual(6.0, (Voltage.Of(2).Volts + Voltage.Of(4).Volts).To.Volts);
    }
}
