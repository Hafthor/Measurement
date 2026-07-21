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
        Assert.AreEqual(0.0, default(Length).ToMeters());
        Assert.AreEqual(0.0, Length.Zero.ToMeters());
    }

    [TestMethod]
    public void GeneratedOperatorsWork() {
        Assert.AreEqual(7.0, (Length.FromMeters(3) + Length.FromMeters(4)).ToMeters());
        Assert.AreEqual(15.0, (Length.FromMeters(5) * 3).ToMeters());
        Assert.AreEqual(4.0, Length.FromKilometers(1) / Length.FromMeters(250)); // ratio
        Assert.IsTrue(Length.FromMeters(1) < Length.FromMeters(2));
        Assert.IsTrue(Length.FromKilometers(1) == Length.FromMeters(1000));
        Assert.AreEqual("5 m", Length.FromMeters(5).ToString());
    }

    [TestMethod]
    public void CrossTypeOperatorsStillWork() {
        // Length (struct) / Duration (struct) => Speed (class)
        Speed s = Length.FromMeters(100) / Duration.FromSeconds(10);
        Assert.AreEqual(10.0, s.ToMetersPerSecond());
        // Mass (struct) * Acceleration (class) => Force (class)
        Force f = Mass.FromKilograms(2) * Acceleration.FromMetersPerSecondSquared(3);
        Assert.AreEqual(6.0, f.ToNewtons());
    }

    private static T Sum<T>(IEnumerable<T> xs) where T : IMeasurement<T> {
        T total = T.AdditiveIdentity;
        foreach (var x in xs) total += x;
        return total;
    }

    [TestMethod]
    public void GenericMathWorksForStructs() {
        var lengths = new[] { Length.FromMeters(1), Length.FromMeters(2), Length.FromMeters(3) };
        Assert.AreEqual(6.0, Sum(lengths).ToMeters());
        Assert.AreEqual(0.0, Length.Zero.ToMeters());
    }
}
