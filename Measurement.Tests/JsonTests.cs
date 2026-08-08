using System.Text.Encodings.Web;
using System.Text.Json;

namespace com.hafthor.Measurement;

[TestClass]
public sealed class JsonTests {
    private static JsonSerializerOptions Options() {
        var o = new JsonSerializerOptions {
            // Unit symbols contain non-ASCII (², ³, Ω, ·, µ); keep them literal rather than \uXXXX.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        o.Converters.Add(new MeasurementJsonConverterFactory());
        return o;
    }

    [TestMethod]
    public void SerializesInFundamentalSiUnit() {
        var o = Options();
        Assert.AreEqual("\"5 m\"", JsonSerializer.Serialize(Length.Of(5).Meters, o));
        Assert.AreEqual("\"2000 g\"", JsonSerializer.Serialize(Mass.Of(2).Kilo.Grams, o));
        Assert.AreEqual("\"6 N\"", JsonSerializer.Serialize(Force.Of(6).Newtons, o));
        Assert.AreEqual("\"1 W/m²\"", JsonSerializer.Serialize(HeatFluxDensity.Of(1).Watts.Per.Square.Meter, o));
        // dimensionless → bare number
        Assert.AreEqual("1000", JsonSerializer.Serialize(Quantity.Of(1000).Count, o));
        Assert.AreEqual("0.5", JsonSerializer.Serialize(Ratio.Of(0.5).Ratio, o));
    }

    [TestMethod]
    public void RoundTripsThroughStringAndNumber() {
        var o = Options();
        Assert.AreEqual(Mass.Of(2.5).Kilo.Grams,
            JsonSerializer.Deserialize<Mass>(JsonSerializer.Serialize(Mass.Of(2.5).Kilo.Grams, o), o));
        Assert.AreEqual(Ratio.Of(37).Percent,
            JsonSerializer.Deserialize<Ratio>(JsonSerializer.Serialize(Ratio.Of(37).Percent, o), o));
    }

    [TestMethod]
    public void DeserializesLenientlyFromStringOrBareNumber() {
        var o = Options();
        // symbol optional on read
        Assert.AreEqual(Length.Of(5).Meters, JsonSerializer.Deserialize<Length>("\"5 m\"", o));
        Assert.AreEqual(Length.Of(5).Meters, JsonSerializer.Deserialize<Length>("\"5\"", o));
        Assert.AreEqual(Length.Of(5).Meters, JsonSerializer.Deserialize<Length>("5", o));
    }

    private sealed class Widget {
        public Length Width { get; set; }
        public Mass Weight { get; set; }
        public List<Speed> Speeds { get; set; }
    }

    [TestMethod]
    public void WorksInsidePocosAndCollections() {
        var o = Options();
        var w = new Widget {
            Width = Length.Of(30).Centi.Meters,
            Weight = Mass.Of(150).Grams,
            Speeds = [Speed.Of(1).Meters.Per.Second, Speed.Of(2).Meters.Per.Second],
        };
        var json = JsonSerializer.Serialize(w, o);
        var back = JsonSerializer.Deserialize<Widget>(json, o);
        Assert.AreEqual(w.Width, back.Width);
        Assert.AreEqual(w.Weight, back.Weight);
        CollectionAssert.AreEqual(w.Speeds, back.Speeds);
    }

    [TestMethod]
    public void EveryMeasurementTypeRoundTrips() {
        var o = Options();
        int count = 0;
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            object original = MeasurementReflection.FromCanonical(t, 12345.678);
            string json = JsonSerializer.Serialize(original, t, o);
            object back = JsonSerializer.Deserialize(json, t, o);
            double a = MeasurementReflection.Canonical(original);
            double b = MeasurementReflection.Canonical(back);
            double relErr = Math.Abs(b - a) / Math.Abs(a);
            if (relErr >= 1e-12) Assert.Fail($"{t.Name} JSON round-trip drifted: {a} → {json} → {b}");
            count++;
        }
        if (count < 80) Assert.Fail($"expected to check all measurement types, only saw {count}");
    }
}
