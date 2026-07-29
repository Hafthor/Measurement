using System.Globalization;

namespace com.hafthor.Measurement;

// Shared parsing for the generated Parse/TryParse (IParsable / ISpanParsable), the inverse of
// ToString. Accepts the value in its display SI unit — "5 m", "5m", or a bare "5" — validating the
// trailing unit symbol when present, then scales by DisplayFactor back to the stored canonical
// value. Only the SI unit symbol is recognised (use the FromXxx factories for other units).
internal static class MeasurementParsing {
    public static bool TryParseCanonical(ReadOnlySpan<char> s, string symbol, double displayFactor,
        IFormatProvider provider, out double canonical) {
        canonical = 0;
        s = s.Trim();
        if (s.IsEmpty) return false;
        if (symbol.Length > 0 && s.EndsWith(symbol)) s = s[..^symbol.Length].TrimEnd();
        if (!double.TryParse(s, NumberStyles.Float, provider, out double display)) return false;
        canonical = display * displayFactor;
        return true;
    }

    public static bool TryParseCanonical(string s, string symbol, double displayFactor,
        IFormatProvider provider, out double canonical) {
        canonical = 0;
        return s != null && TryParseCanonical(s.AsSpan(), symbol, displayFactor, provider, out canonical);
    }
}
