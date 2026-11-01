namespace com.hafthor.Measurement;

[TestClass]
public sealed class IEnumerableExtensionsTests {
    [TestMethod]
    public void NonEmptySource_SumAndAverage() {
        Duration[] durations = [Duration.Of(1).Hours, Duration.Of(30).Minutes];

        Assert.AreEqual(Duration.Of(90).Minutes, durations.Sum());

        Assert.AreEqual(Duration.Of(45).Minutes, durations.Average());

        Temperature[] temperatures = [Temperature.Of(0).Celsius, Temperature.Of(10).Celsius];

        Assert.AreEqual(Temperature.Of(273.15 + 10).Celsius, temperatures.Sum());

        Assert.AreEqual(Temperature.Of(5).Celsius, temperatures.Average());
    }

    [TestMethod]
    public void NullableNonEmptySource_SumAndAverage() {
        Duration?[] durations = [Duration.Of(1).Hours, null, Duration.Of(30).Minutes];

        Assert.AreEqual(Duration.Of(90).Minutes, durations.Sum());

        Assert.AreEqual(Duration.Of(45).Minutes, durations.Average());

        Temperature?[] temperatures = [Temperature.Of(0).Celsius, null, Temperature.Of(10).Celsius];

        Assert.AreEqual(Temperature.Of(273.15 + 10).Celsius, temperatures.Sum());

        Assert.AreEqual(Temperature.Of(5).Celsius, temperatures.Average());
    }

    [TestMethod]
    public void EmptySource_SumAndAverage() {
        Assert.AreEqual(Duration.Zero, Array.Empty<Duration>().Sum());

        // Averaging over an empty source should produce the same exception type/message as it does for other types.
        Assert.AreEqual(
            Assert.Throws<InvalidOperationException>(() => Array.Empty<double>().Average()).Message,
            Assert.Throws<InvalidOperationException>(() => Array.Empty<Duration>().Average()).Message);
    }

    [TestMethod]
    public void NullableEmptySource_SumAndAverage() {
        Assert.IsNull(Array.Empty<Duration?>().Sum());

        // Behavior should match what .Average over double? does
        Assert.IsNull(Array.Empty<double?>().Average());
        Assert.IsNull(Array.Empty<Duration?>().Average());
    }
}
