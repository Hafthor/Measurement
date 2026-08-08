namespace com.hafthor.Measurement;

// Every measurement is a readonly struct whose operator/equality/comparison/System.Numerics
// surface is supplied by the source generator.
[TestClass]
public sealed class ValueTypeTests {
    [TestMethod]
    public void AreValueTypes() {
        Assert.IsTrue(typeof(Length).IsValueType);
        Assert.IsTrue(typeof(Mass).IsValueType);
        Assert.IsTrue(typeof(Duration).IsValueType);
    }

    [TestMethod]
    public void DefaultIsZero() {
        Assert.AreEqual(0.0, default(Length).To.Meters);
        Assert.AreEqual(0.0, Length.Zero.To.Meters);
    }

    [TestMethod]
    public void GeneratedOperatorsWork() {
        Assert.AreEqual(7.0, (Length.Of(3).Meters + Length.Of(4).Meters).To.Meters);
        Assert.AreEqual(15.0, (Length.Of(5).Meters * 3).To.Meters);
        Assert.AreEqual(4.0, Length.Of(1).Kilo.Meters / Length.Of(250).Meters); // ratio
        Assert.IsTrue(Length.Of(1).Meters < Length.Of(2).Meters);
        Assert.IsTrue(Length.Of(1).Kilo.Meters == Length.Of(1000).Meters);
        Assert.AreEqual("5 m", Length.Of(5).Meters.ToString());
    }

    [TestMethod]
    public void CrossTypeOperatorsStillWork() {
        // Length (struct) / Duration (struct) => Speed (class)
        Speed s = Length.Of(100).Meters / Duration.Of(10).Seconds;
        Assert.AreEqual(10.0, s.To.Meters.Per.Second);
        // Mass (struct) * Acceleration (class) => Force (class)
        Force f = Mass.Of(2).Kilo.Grams * Acceleration.Of(3).Meters.Per.Second.Squared;
        Assert.AreEqual(6.0, f.To.Newtons);
    }

    private static T Sum<T>(IEnumerable<T> xs) where T : IMeasurement<T> {
        T total = T.AdditiveIdentity;
        foreach (var x in xs) total += x;
        return total;
    }

    [TestMethod]
    public void GenericMathWorksForStructs() {
        var lengths = new[] { Length.Of(1).Meters, Length.Of(2).Meters, Length.Of(3).Meters };
        Assert.AreEqual(6.0, Sum(lengths).To.Meters);
        Assert.AreEqual(0.0, Length.Zero.To.Meters);
    }
}
