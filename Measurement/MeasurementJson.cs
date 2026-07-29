using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.hafthor.Measurement;

// System.Text.Json support for every measurement type. Values serialize in their fundamental
// SI unit (via DisplayFactor) — e.g. Length → "5 m", Mass → "2000 g" — so the wire format is
// stable and human-readable, independent of the internal storage anchor. Dimensionless types
// (empty symbol, e.g. Ratio/Quantity) serialize as a bare JSON number. Register once:
//
//     var options = new JsonSerializerOptions();
//     options.Converters.Add(new MeasurementJsonConverterFactory());
public sealed class MeasurementJsonConverterFactory : JsonConverterFactory {
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsValueType && typeToConvert.GetCustomAttribute<MeasurementAttribute>() != null;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(typeof(MeasurementJsonConverter<>).MakeGenericType(typeToConvert));
}

public sealed class MeasurementJsonConverter<T> : JsonConverter<T> where T : IMeasurement<T> {
    private static readonly string Symbol;
    private static readonly double DisplayFactor;

    static MeasurementJsonConverter() {
        var attr = typeof(T).GetCustomAttribute<MeasurementAttribute>()
            ?? throw new InvalidOperationException($"{typeof(T)} is not a [Measurement] type.");
        Symbol = attr.Symbol;
        DisplayFactor = attr.DisplayFactor;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        double display;
        if (reader.TokenType == JsonTokenType.String) {
            var text = reader.GetString().AsSpan().Trim();
            int space = text.IndexOf(' ');
            var number = space < 0 ? text : text[..space];
            display = double.Parse(number, NumberStyles.Float, CultureInfo.InvariantCulture);
        } else {
            display = reader.GetDouble();
        }
        return T.FromCanonical(display * DisplayFactor);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
        double display = value.CanonicalValue / DisplayFactor;
        if (Symbol.Length == 0)
            writer.WriteNumberValue(display);
        else
            writer.WriteStringValue(display.ToString("R", CultureInfo.InvariantCulture) + " " + Symbol);
    }
}
