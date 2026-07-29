using System.Globalization;

namespace com.hafthor.Measurement;

[TestClass]
public sealed class MeasureParseTests {
    [TestMethod]
    public void PicksTypeFromUnitSymbol() {
        Assert.IsInstanceOfType<Speed>(Measure.Parse("5 m/s"));
        Assert.IsInstanceOfType<Mass>(Measure.Parse("2000 g"));
        Assert.IsInstanceOfType<Temperature>(Measure.Parse("300 K"));
        Assert.IsInstanceOfType<HeatFluxDensity>(Measure.Parse("1 W/m²"));
        Assert.IsInstanceOfType<MolarHeatCapacity>(Measure.Parse("5 J/(mol·K)"));
        Assert.IsInstanceOfType<Resistivity>(Measure.Parse("2 Ω·m"));
    }

    [TestMethod]
    public void ReturnsEquivalentValue() {
        var m = Measure.Parse("2000 g");
        Assert.AreEqual(Mass.FromKilograms(2), (Mass)m);
        // usable through the non-generic surface
        Assert.AreEqual("m/s", Measure.Parse("5 m/s").UnitSymbol);
        Assert.AreEqual(Speed.FromMetersPerSecond(5).CanonicalValue, Measure.Parse("5 m/s").CanonicalValue);
    }

    [TestMethod]
    public void RejectsBareNumbersAndUnknownUnits() {
        Assert.IsFalse(Measure.TryParse("5", out _));        // no unit → ambiguous
        Assert.IsFalse(Measure.TryParse("5 xyz", out _));    // unknown unit
        Assert.IsFalse(Measure.TryParse("", out _));
        Assert.IsFalse(Measure.TryParse("m/s", out _));      // no number
        Assert.ThrowsExactly<FormatException>(() => Measure.Parse("5 xyz"));
    }

    [TestMethod]
    public void IsCultureAware() {
        var de = CultureInfo.GetCultureInfo("de-DE");
        Assert.AreEqual(Length.FromMeters(1.5), (Length)Measure.Parse("1,5 m", de));
    }

    [TestMethod]
    public void RoundTripsToStringForEveryDimensionedType() {
        int count = 0;
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            if (MeasurementReflection.Symbol(t).Length == 0) continue; // dimensionless → no unit to key on
            object original = MeasurementReflection.FromCanonical(t, 1234.5);
            string s = original.ToString();
            IMeasurement parsed = Measure.Parse(s);
            Assert.AreEqual(t, parsed.GetType(), $"'{s}' resolved to the wrong type");
            double a = MeasurementReflection.Canonical(original);
            double b = parsed.CanonicalValue;
            double relErr = Math.Abs(b - a) / Math.Abs(a);
            if (relErr > 1e-9) Assert.Fail($"{t.Name}: '{s}' round-trip {a} → {b} (relErr {relErr:E2})");
            count++;
        }
        if (count < 70) Assert.Fail($"expected to round-trip most types, only did {count}");
    }
}
