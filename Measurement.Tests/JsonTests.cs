using System.Text.Encodings.Web;
using System.Text.Json;

namespace com.hafthor.Measurement;

[TestClass]
public sealed class JsonTests {
    private static JsonSerializerOptions Options() {
        var options = new JsonSerializerOptions {
            // Unit symbols contain non-ASCII (², ³, Ω, ·, µ); keep them literal rather than \uXXXX.
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        options.Converters.Add(new MeasurementJsonConverterFactory());
        return options;
    }

    [TestMethod]
    public void SerializesInFundamentalSiUnit() {
        var options = Options();
        Assert.AreEqual("\"5 m\"", JsonSerializer.Serialize(Length.Of(5).Meters, options));
        Assert.AreEqual("\"2000 g\"", JsonSerializer.Serialize(Mass.Of(2).Kilo.Grams, options));
        Assert.AreEqual("\"6 N\"", JsonSerializer.Serialize(Force.Of(6).Newtons, options));
        Assert.AreEqual("\"1 W/m²\"", JsonSerializer.Serialize(HeatFluxDensity.Of(1).Watts.Per.Square.Meter, options));
        // dimensionless → bare number
        Assert.AreEqual("1000", JsonSerializer.Serialize(Quantity.Of(1000).Count, options));
        Assert.AreEqual("0.5", JsonSerializer.Serialize(Ratio.Of(0.5).Ratio, options));
    }

    [TestMethod]
    public void RoundTripsThroughStringAndNumber() {
        var options = Options();
        Assert.AreEqual(Mass.Of(2.5).Kilo.Grams,
            JsonSerializer.Deserialize<Mass>(JsonSerializer.Serialize(Mass.Of(2.5).Kilo.Grams, options), options));
        Assert.AreEqual(Ratio.Of(37).Percent,
            JsonSerializer.Deserialize<Ratio>(JsonSerializer.Serialize(Ratio.Of(37).Percent, options), options));
    }

    [TestMethod]
    public void DeserializesLenientlyFromStringOrBareNumber() {
        var options = Options();
        // symbol optional on read
        Assert.AreEqual(Length.Of(5).Meters, JsonSerializer.Deserialize<Length>("\"5 m\"", options));
        Assert.AreEqual(Length.Of(5).Meters, JsonSerializer.Deserialize<Length>("\"5\"", options));
        Assert.AreEqual(Length.Of(5).Meters, JsonSerializer.Deserialize<Length>("5", options));
    }

    private sealed class Widget {
        public Length Width { get; set; }
        public Mass Weight { get; set; }
        public List<Speed> Speeds { get; set; }
    }

    [TestMethod]
    public void WorksInsidePocosAndCollections() {
        var options = Options();
        var widget = new Widget {
            Width = Length.Of(30).Centi.Meters,
            Weight = Mass.Of(150).Grams,
            Speeds = [Speed.Of(1).Meters.Per.Second, Speed.Of(2).Meters.Per.Second],
        };
        var json = JsonSerializer.Serialize(widget, options);
        var back = JsonSerializer.Deserialize<Widget>(json, options);
        Assert.AreEqual(widget.Width, back.Width);
        Assert.AreEqual(widget.Weight, back.Weight);
        CollectionAssert.AreEqual(widget.Speeds, back.Speeds);
    }

    [TestMethod]
    public void EveryMeasurementTypeRoundTrips() {
        var options = Options();
        int count = 0;
        foreach (var t in MeasurementReflection.AllMeasurementTypes()) {
            object original = MeasurementReflection.FromCanonical(t, 12345.678);
            string json = JsonSerializer.Serialize(original, t, options);
            object back = JsonSerializer.Deserialize(json, t, options);
            double a = MeasurementReflection.Canonical(original);
            double b = MeasurementReflection.Canonical(back);
            double relErr = Math.Abs(b - a) / Math.Abs(a);
            if (relErr >= 1e-12) Assert.Fail($"{t.Name} JSON round-trip drifted: {a} → {json} → {b}");
            count++;
        }
        Assert.IsGreaterThanOrEqualTo(80, count, $"expected to check all measurement types, only saw {count}");
    }
}
