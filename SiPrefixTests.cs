using com.hafthor.Measurement.Fluent; // opt in to the `double` sugar

namespace com.hafthor.Measurement;

[TestClass]
public sealed class SiPrefixTests {
    [TestMethod]
    public void FluentPrefixes() {
        Assert.AreEqual(5000.0, 5.0.Kilo.Meters.ToMeters());
        Assert.AreEqual(0.25, 250.0.Milli.Meters.ToMeters());
        Assert.AreEqual(1e-6, 1.0.Micro.Seconds.ToSeconds());
        Assert.AreEqual(2.0, 2.0.Kilo.Newtons.ToKilonewtons());
    }

    [TestMethod]
    public void NoPrefixShortcut() {
        Assert.AreEqual(3.0, 3.0.Meters.ToMeters());
        Assert.AreEqual(20.0, 20.0.Newtons.ToNewtons());
    }

    [TestMethod]
    public void MassPrefixesAttachToGram() {
        // SI prefixes on mass attach to the gram, not the kilogram base.
        Assert.AreEqual(5.0, 5.0.Kilo.Grams.ToKilograms());   // 5 kg
        Assert.AreEqual(5.0, 5.0.Grams.ToGrams());            // 5 g
        Assert.AreEqual(1e-6, 1.0.Milli.Grams.ToKilograms()); // 1 mg = 1e-6 kg
    }

    [TestMethod]
    public void FluentReadOut() {
        var m = Mass.FromKilograms(2);            // 2 kg
        Assert.AreEqual(2000.0, m.To.Grams);      // 2000 g
        Assert.AreEqual(2_000_000.0, m.To.Milli.Grams);
        Assert.AreEqual(2.0, m.To.Kilo.Grams);
        Assert.AreEqual(5.0, Length.FromKilometers(5).To.Kilo.Meters);
        Assert.AreEqual(1500.0, Duration.FromSeconds(1.5).To.Milli.Seconds);
    }

    [TestMethod]
    public void RoundTripThroughFluentApi() {
        Length original = 3.0.Kilo.Meters;                 // 3 km
        Assert.AreEqual(3.0, original.To.Kilo.Meters);     // read back as km
    }

    [TestMethod]
    public void StackedPrefixes_Input() {
        // 1 Mega·Mega metre = 1e12 m
        Assert.AreEqual(1e12, (1.0.Mega.Mega.Meters).ToMeters(), 1e-3);
        // mixed stack: 2 Kilo·Milli = 2 (net factor 1)
        Assert.AreEqual(2.0, (2.0.Kilo.Milli.Meters).ToMeters(), 1e-12);
        // 3 milli·micro gram = 3e-9 g
        Assert.AreEqual(3e-9, (3.0.Milli.Micro.Grams).ToGrams(), 1e-21);
    }

    [TestMethod]
    public void StackedPrefixes_Output() {
        Assert.AreEqual(1.0, Length.FromMeters(1e12).To.Mega.Mega.Meters, 1e-12);
        Assert.AreEqual(2.0, Length.FromMeters(2).To.Kilo.Milli.Meters, 1e-12); // net factor 1
        // symmetric round trip with a stacked prefix
        Assert.AreEqual(7.0, (7.0.Mega.Mega.Meters).To.Mega.Mega.Meters, 1e-9);
    }
}
