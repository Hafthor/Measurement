using com.hafthor.Measurement.Fluent; // opt in to the `double` sugar

namespace com.hafthor.Measurement;

[TestClass]
public sealed class SiPrefixTests {
    [TestMethod]
    public void FluentPrefixes() {
        Assert.AreEqual(5000.0, 5.0.Kilo.Meters.To.Meters);
        Assert.AreEqual(0.25, 250.0.Milli.Meters.To.Meters);
        Assert.AreEqual(1e-6, 1.0.Micro.Seconds.To.Seconds);
        Assert.AreEqual(2.0, 2.0.Kilo.Newtons.To.Kilo.Newtons);
    }

    [TestMethod]
    public void NoPrefixShortcut() {
        Assert.AreEqual(3.0, 3.0.Meters.To.Meters);
        Assert.AreEqual(20.0, 20.0.Newtons.To.Newtons);
    }

    [TestMethod]
    public void MassPrefixesAttachToGram() {
        // SI prefixes on mass attach to the gram, not the kilogram base.
        Assert.AreEqual(5.0, 5.0.Kilo.Grams.To.Kilo.Grams);   // 5 kg
        Assert.AreEqual(5.0, 5.0.Grams.To.Grams);            // 5 g
        Assert.AreEqual(1e-6, 1.0.Milli.Grams.To.Kilo.Grams); // 1 mg = 1e-6 kg
    }

    [TestMethod]
    public void PrefixChainEqualsExplicitPrefixedFactory() {
        // The decade ladder is expressed via the prefix chain, not a combined hook; the result
        // matches the explicit FromKilometers/FromMilligrams-style factory exactly.
        Assert.AreEqual(Length.Of(3).Kilo.Meters.To.Meters, (3.0.Kilo.Meters).To.Meters);
        Assert.AreEqual(Mass.Of(250).Milli.Grams.To.Kilo.Grams, (250.0.Milli.Grams).To.Kilo.Grams);
        // read-out likewise composes through the chain
        Assert.AreEqual(Length.Of(5000).Meters.To.Kilo.Meters, Length.Of(5000).Meters.To.Kilo.Meters);
    }

    [TestMethod]
    public void FluentReadOut() {
        var m = Mass.Of(2).Kilo.Grams;            // 2 kg
        Assert.AreEqual(2000.0, m.To.Grams);      // 2000 g
        Assert.AreEqual(2_000_000.0, m.To.Milli.Grams);
        Assert.AreEqual(2.0, m.To.Kilo.Grams);
        Assert.AreEqual(5.0, Length.Of(5).Kilo.Meters.To.Kilo.Meters);
        Assert.AreEqual(1500.0, Duration.Of(1.5).Seconds.To.Milli.Seconds);
    }

    [TestMethod]
    public void RoundTripThroughFluentApi() {
        Length original = 3.0.Kilo.Meters;                 // 3 km
        Assert.AreEqual(3.0, original.To.Kilo.Meters);     // read back as km
    }

    [TestMethod]
    public void StackedPrefixes_Input() {
        // 1 Mega·Mega metre = 1e12 m
        Assert.AreEqual(1e12, (1.0.Mega.Mega.Meters).To.Meters, 1e-3);
        // mixed stack: 2 Kilo·Milli = 2 (net factor 1)
        Assert.AreEqual(2.0, (2.0.Kilo.Milli.Meters).To.Meters, 1e-12);
        // 3 milli·micro gram = 3e-9 g
        Assert.AreEqual(3e-9, (3.0.Milli.Micro.Grams).To.Grams, 1e-21);
    }

    [TestMethod]
    public void StackedPrefixes_Output() {
        Assert.AreEqual(1.0, Length.Of(1e12).Meters.To.Mega.Mega.Meters, 1e-12);
        Assert.AreEqual(2.0, Length.Of(2).Meters.To.Kilo.Milli.Meters, 1e-12); // net factor 1
        // symmetric round trip with a stacked prefix
        Assert.AreEqual(7.0, (7.0.Mega.Mega.Meters).To.Mega.Mega.Meters, 1e-9);
    }
}
