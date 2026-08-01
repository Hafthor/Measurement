using com.hafthor.Measurement.Fluent;

namespace com.hafthor.Measurement;

// Spot-checks that the fluent API is wired across a broad sample of classes.
[TestClass]
public sealed class FluentRolloutTests {
[TestMethod]
public void CollisionSelector_DisambiguatesSharedUnitName() {
    // JouleSeconds is a product spelling (Joule·Seconds), so the compositional walk disambiguates it:
    // `.Joule.Seconds` is implicitly the Primary (Action); `.Action`/`.AngularMomentum` name the reading.
    Action implicitlyAction = 5.0.Joule.Seconds;
    Assert.AreEqual(5.0, implicitlyAction.ToJouleSeconds());
    Action a = 5.0.Joule.Seconds.Action;
    Assert.AreEqual(5.0, a.ToJouleSeconds());
    AngularMomentum am = 5.0.Joule.Seconds.AngularMomentum;
    Assert.AreEqual(5.0, am.ToJouleSeconds());
    Action ka = Measure.Of(5).Kilo.Joule.Seconds.Action;
    Assert.AreEqual(5000.0, ka.ToJouleSeconds());
    // RevolutionsPerMinute is a quotient spelling (has Per), not reachable by the product walk, so it
    // keeps its flat selector, shared by AngularVelocity and Frequency.
    Assert.AreEqual(3.0, (3.0.RevolutionsPerMinute.Frequency).ToRevolutionsPerMinute(), 1e-12);
    Assert.AreEqual(3.0, (3.0.RevolutionsPerMinute.AngularVelocity).ToRevolutionsPerMinute(), 1e-12);
    // read-out remains direct and unambiguous
    Assert.AreEqual(2.0, Frequency.FromRevolutionsPerMinute(2).To.RevolutionsPerMinute, 1e-12);
}

    [TestMethod]
    public void PrefixInput_AcrossClasses() {
        Assert.AreEqual(5000.0, Measure.Of(5).Kilo.Joules.ToJoules());
        Assert.AreEqual(2e6, Measure.Of(2).Mega.Watts.ToWatts());
        Assert.AreEqual(1e-3, Measure.Of(1).Milli.Volts.ToVolts());
        Assert.AreEqual(101325.0, Measure.Of(101.325).Kilo.Pascals.ToPascals(), 1e-6);
        Assert.AreEqual(1e9, Measure.Of(1).Giga.Hertz.ToHertz());
    }

    [TestMethod]
    public void DoubleSugar_AcrossClasses() {
        Assert.AreEqual(5000.0, (5.0.Kilo.Joules).ToJoules());
        Assert.AreEqual(1e-6, (1.0.Micro.Farads).ToFarads());
        Assert.AreEqual(3.0, (3.0.Amperes).ToAmperes());
    }

    [TestMethod]
    public void ReadOut_AnyUnitAndPrefix() {
        Assert.AreEqual(1.0, Energy.FromKilojoules(1).To.Kilo.Joules, 1e-12);
        Assert.AreEqual(60.0, Duration.FromMinutes(1).To.Seconds);
        Assert.AreEqual(1.0, Duration.FromSeconds(60).To.Minutes);       // non-SI reader
        Assert.AreEqual(32.0, Temperature.FromCelsius(0).To.Fahrenheit, 1e-9); // scale reader
        Assert.AreEqual(1000.0, Mass.FromKilograms(1).To.Grams);
    }

    [TestMethod]
    public void NonSiUnits_InputAndOutput() {
        // non-SI units are fully fluent for input too
        Assert.AreEqual(1609.344, Measure.Of(1).Miles.ToMeters());
        Assert.AreEqual(0.45359237, Measure.Of(1).Pounds.ToKilograms());
        Assert.AreEqual(3.0, Measure.Of(3).Feet.To.Feet, 1e-12);
        // prefix stacks onto a non-SI unit too (5 kilo-miles = 5000 miles)
        Assert.AreEqual(5000.0, Measure.Of(5).Kilo.Miles.ToMiles(), 1e-9);
        // opt-in double form
        Assert.AreEqual(1609.344, (1.0.Miles).ToMeters());
    }
}
