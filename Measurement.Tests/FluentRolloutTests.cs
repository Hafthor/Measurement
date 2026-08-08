using com.hafthor.Measurement.Fluent;

namespace com.hafthor.Measurement;

// Spot-checks that the fluent API is wired across a broad sample of classes.
[TestClass]
public sealed class FluentRolloutTests {
[TestMethod]
public void CompositionalWalk_DisambiguatesSharedUnitNames() {
    // JouleSeconds is a product spelling (Joule·Seconds), so the compositional walk disambiguates it:
    // `.Joule.Seconds` is implicitly the Primary (Action); `.Action`/`.AngularMomentum` name the reading.
    Action implicitlyAction = 5.0.Joule.Seconds;
    Assert.AreEqual(5.0, implicitlyAction.To.JouleSeconds);
    Action a = 5.0.Joule.Seconds.Action;
    Assert.AreEqual(5.0, a.To.JouleSeconds);
    AngularMomentum am = 5.0.Joule.Seconds.AngularMomentum;
    Assert.AreEqual(5.0, am.To.JouleSeconds);
    Action ka = Measure.Of(5).Kilo.Joule.Seconds.Action;
    Assert.AreEqual(5000.0, ka.To.JouleSeconds);
    // rpm is an angular rate — a revolution is one turn — so it composes directly, no selector.
    AngularVelocity w = Measure.Of(3).Revolutions.Per.Minute;
    Assert.AreEqual(AngularVelocity.Of(3).Revolutions.Per.Minute.To.Radians.Per.Second,
        w.To.Radians.Per.Second, 1e-12);
}

    [TestMethod]
    public void PrefixInput_AcrossClasses() {
        Assert.AreEqual(5000.0, Measure.Of(5).Kilo.Joules.To.Joules);
        Assert.AreEqual(2e6, Measure.Of(2).Mega.Watts.To.Watts);
        Assert.AreEqual(1e-3, Measure.Of(1).Milli.Volts.To.Volts);
        Assert.AreEqual(101325.0, Measure.Of(101.325).Kilo.Pascals.To.Pascals, 1e-6);
        Assert.AreEqual(1e9, Measure.Of(1).Giga.Hertz.To.Hertz);
    }

    [TestMethod]
    public void DoubleSugar_AcrossClasses() {
        Assert.AreEqual(5000.0, (5.0.Kilo.Joules).To.Joules);
        Assert.AreEqual(1e-6, (1.0.Micro.Farads).To.Farads);
        Assert.AreEqual(3.0, (3.0.Amperes).To.Amperes);
    }

    [TestMethod]
    public void ReadOut_AnyUnitAndPrefix() {
        Assert.AreEqual(1.0, Energy.Of(1).Kilo.Joules.To.Kilo.Joules, 1e-12);
        Assert.AreEqual(60.0, Duration.Of(1).Minutes.To.Seconds);
        Assert.AreEqual(1.0, Duration.Of(60).Seconds.To.Minutes);       // non-SI reader
        Assert.AreEqual(32.0, Temperature.Of(0).Celsius.To.Fahrenheit, 1e-9); // scale reader
        Assert.AreEqual(1000.0, Mass.Of(1).Kilo.Grams.To.Grams);
    }

    [TestMethod]
    public void NonSiUnits_InputAndOutput() {
        // non-SI units are fully fluent for input too
        Assert.AreEqual(1609.344, Measure.Of(1).Miles.To.Meters);
        Assert.AreEqual(0.45359237, Measure.Of(1).Pounds.To.Kilo.Grams);
        Assert.AreEqual(3.0, Measure.Of(3).Feet.To.Feet, 1e-12);
        // prefix stacks onto a non-SI unit too (5 kilo-miles = 5000 miles)
        Assert.AreEqual(5000.0, Measure.Of(5).Kilo.Miles.To.Miles, 1e-9);
        // opt-in double form
        Assert.AreEqual(1609.344, (1.0.Miles).To.Meters);
    }
}
