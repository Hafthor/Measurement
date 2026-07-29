using System.Globalization;
using System.Reflection;

namespace com.hafthor.Measurement;

[TestClass]
public sealed class ParseTests {
    [TestMethod]
    public void ParsesSiUnitBareAndNoSpace() {
        Assert.AreEqual(Length.FromMeters(5), Length.Parse("5 m"));
        Assert.AreEqual(Length.FromMeters(5), Length.Parse("5m"));
        Assert.AreEqual(Length.FromMeters(5), Length.Parse("5"));
        Assert.AreEqual(Length.FromKilometers(1), Length.Parse("1000 m"));
        Assert.AreEqual(Mass.FromKilograms(2), Mass.Parse("2000 g"));
        Assert.AreEqual(HeatFluxDensity.FromWattsPerSquareMeter(1), HeatFluxDensity.Parse("1 W/m²"));
        Assert.AreEqual(Quantity.FromCount(1000), Quantity.Parse("1000"));
        Assert.AreEqual(Ratio.FromRatio(3), Ratio.Parse("3"));
    }

    [TestMethod]
    public void ParseHandlesSignAndExponent() {
        Assert.AreEqual(Temperature.FromKelvin(-40), Temperature.Parse("-40 K"));
        Assert.AreEqual(Capacitance.FromMicrofarads(5), Capacitance.Parse("5E-06 F"));
    }

    [TestMethod]
    public void TryParseRejectsGarbageAndWrongUnit() {
        Assert.IsFalse(Length.TryParse("abc", out _));
        Assert.IsFalse(Length.TryParse("", out _));
        Assert.IsFalse(Length.TryParse("5 kg", out _));   // not the metre symbol
        Assert.IsFalse(Quantity.TryParse("5 m", out _));  // dimensionless carries no unit
        Assert.IsTrue(Length.TryParse("5 m", out var ok));
        Assert.AreEqual(Length.FromMeters(5), ok);
    }

    [TestMethod]
    public void ParseThrowsFormatExceptionOnFailure() =>
        Assert.ThrowsExactly<FormatException>(() => Length.Parse("nope"));

    [TestMethod]
    public void SpanParseWorks() {
        ReadOnlySpan<char> s = "42 m".AsSpan();
        Assert.AreEqual(Length.FromMeters(42), Length.Parse(s, null));
        Assert.IsTrue(Length.TryParse("42 m".AsSpan(), null, out var r));
        Assert.AreEqual(Length.FromMeters(42), r);
    }

    [TestMethod]
    public void ParseIsCultureAware() {
        // German uses ',' as the decimal separator
        var de = CultureInfo.GetCultureInfo("de-DE");
        Assert.AreEqual(Length.FromMeters(1.5), Length.Parse("1,5 m", de));
        Assert.AreEqual(Length.FromMeters(1.5), Length.Parse("1.5 m", CultureInfo.InvariantCulture));
    }

    // Generic usage through the System IParsable<T> constraint.
    private static T ParseGeneric<T>(string s) where T : IParsable<T> => T.Parse(s, CultureInfo.InvariantCulture);

    [TestMethod]
    public void WorksThroughIParsableConstraint() =>
        Assert.AreEqual(Speed.FromMetersPerSecond(9.8), ParseGeneric<Speed>("9.8 m/s"));

    [TestMethod]
    public void ParseRoundTripsToStringForEveryType() {
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            var parse = t.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
            object original = MeasurementReflection.FromCanonical(t, 1234.5);
            string s = original.ToString();
            object parsed = parse.Invoke(null, [s]);
            double a = MeasurementReflection.Canonical(original);
            double b = MeasurementReflection.Canonical(parsed);
            double relErr = Math.Abs(b - a) / Math.Abs(a);
            if (relErr > 1e-9) Assert.Fail($"{t.Name}: '{s}' round-trip {a} → {b} (relErr {relErr:E2})");
        }
    }
}
