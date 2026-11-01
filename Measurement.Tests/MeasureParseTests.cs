using System.Globalization;

namespace com.hafthor.Measurement;

[TestClass]
public sealed class MeasureParseTests {
    [TestMethod]
    public void PicksTypeFromUnitSymbol() {
        (string s, IMeasurement m)[] equivalents = [
            ("5 m/s", Speed.Of(5).Meters.Per.Second),
            ("2000 g", Mass.Of(2000).Grams),
            ("300 K", Temperature.Of(300).Kelvin),
            ("1 W/m²", HeatFluxDensity.Of(1).Watts.Per.Square.Meter),
            ("5 J/(mol·K)", MolarHeatCapacity.Of(5).Joules.Per.Mole.Kelvin),
            ("2 Ω·m", Resistivity.Of(2).Ohm.Meters)
        ];
        foreach (var equivalent in equivalents) {
            var message = "Testing " + equivalent.s;
            var parsed = Measure.Parse(equivalent.s);
            Assert.AreEqual(equivalent.m.GetType(), parsed.GetType(), message);
            Assert.AreEqual(equivalent.m, parsed, message);
            Assert.AreEqual(equivalent.s, parsed.ToString(), message);
            Assert.AreEqual(equivalent.s.Split(' ', 2)[1], parsed.UnitSymbol, message);
            Assert.AreEqual(equivalent.m.CanonicalValue, parsed.CanonicalValue, message);
        }
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
        Assert.AreEqual(Length.Of(1.5).Meters, (Length)Measure.Parse("1,5 m", de));
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
        if (count < 78) Assert.Fail($"expected to round-trip most types, only did {count}");
    }
}
