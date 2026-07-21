namespace com.hafthor.Measurement;

// These tests deliberately do NOT import com.hafthor.Measurement.Fluent, proving the
// fluent API works without any `double` sugar opt-in.
[TestClass]
public sealed class MeasureEntryTests {
    [TestMethod]
    public void NonDoubleEntry_Build() {
        Mass m = Measure.Of(5).Kilo.Grams;    // 5 kg without touching double
        Assert.AreEqual(5.0, m.ToKilograms());
        Length d = Measure.Of(3).Kilo.Meters; // 3 km
        Assert.AreEqual(3000.0, d.ToMeters());
        Mass mg = Measure.Of(250).Milli.Grams; // 250 mg
        Assert.AreEqual(0.25, mg.ToGrams());
    }

    [TestMethod]
    public void NonDoubleEntry_ReadInAnyUnit() {
        // Measure.Of(5).Kilo.Grams.To.Pounds  → 5 kg expressed in pounds
        Assert.AreEqual(5.0 / 0.45359237, Measure.Of(5).Kilo.Grams.To.Pounds, 1e-9);
        Assert.AreEqual(3.0, Measure.Of(3).Kilo.Meters.To.Kilo.Meters, 1e-12);
    }
}
