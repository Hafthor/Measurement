namespace com.hafthor.Measurement;

// Marks a readonly partial struct as a measurement type; the source generator fills in the
// value field, ctor, equality, comparison, formatting, operators, math utilities, and the
// IMeasurement<T> / System.Numerics surface.
[AttributeUsage(AttributeTargets.Struct)]
internal sealed class MeasurementAttribute(string symbol) : Attribute {
    public string Symbol => symbol;

    // Canonical (stored) units per one display unit. ToString divides the stored value by
    // this so a type can be stored at a finer scale than it is presented (e.g. store
    // micrograms, display grams with DisplayFactor = 1e6).
    public double DisplayFactor { get; set; } = 1;

    public string VariableName { get; set; } = "value";
}

// Declares a family of metric units to be generated for a measurement struct. The factor of the
// base (un-prefixed) unit relative to the stored anchor is 10^TenExponent, and Prefixes lists the
// SI prefixes to expand (space-separated; "None" = the base unit itself). The prefix attaches to
// the leading token, e.g. [SiUnit("WattsPerSquareMeter", 3, "None Milli")] generates
// From/ToWattsPerSquareMeter (×1e3) and From/ToMilliwattsPerSquareMeter (×1e0).
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class SiUnitAttribute(string baseName, int tenExponent, string prefixes = "None") : Attribute {
    public string BaseName => baseName;
    public int TenExponent => tenExponent;
    public string Prefixes => prefixes;

    // SI prefixes to apply to the denominator unit (the token after "Per"). The generated factor
    // accounts for its power (Square ⇒ ², Cubic ⇒ ³) and the division, so on "…PerSquareMeter",
    // PerPrefixes = "None Centi Milli" yields …PerSquareMeter, …PerSquareCentimeter (×1e4 relative),
    // …PerSquareMillimeter (×1e6 relative). Numerator × denominator prefixes form a cross product.
    public string PerPrefixes { get; set; } = "None";
}

// Declares a single non-metric unit whose factor to the stored anchor isn't a power of ten (e.g.
// Pounds, Daltons, SolarMasses). Factor = anchor units per one of this unit, so with a microgram
// anchor, [Unit("Pounds", 453.59237e6)] gives From/ToPounds. For affine scales (e.g. temperature),
// Offset and PreOffset give anchor = (value + PreOffset) * factor + Offset, so
// [Unit("Fahrenheit", 5.0/9.0, PreOffset = -32, Offset = 273.15)] is (°F − 32)·5⁄9 + 273.15 K.
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class UnitAttribute(string name, double factor) : Attribute {
    public string Name => name;
    public double Factor => factor;
    public double Offset { get; set; }
}

// Declares an extra fluent hook (Measure.Of(x).Name, x.Name, value.To.Name) for a unit whose
// From/To methods are hand-written rather than generated (e.g. the logarithmic Ratio.Decibels).
// The generator emits only the fluent wiring and assumes From{Name}/To{Name} already exist.
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class UnitHookAttribute(string name) : Attribute {
    public string Name => name;
}

// Declares the dimensional relation C = Left × Right on the result struct C, from which the
// generator emits the full operator set — Left*Right and Right*Left → C, plus C/Left → Right and
// C/Right → Left — with the unit-conversion factor derived from each type's measurement symbol
// (grams→kilograms) and DisplayFactor, so the grams↔kilograms scaling is always correct. When two
// result types declare the same product (e.g. Energy and Torque both = Force × Length) the product
// resolves to a selector naming each result; set Primary = true on the one the selector should
// implicitly convert to.
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class ProductAttribute<TLeft, TRight> : Attribute
    where TLeft : struct, IMeasurement
    where TRight : struct, IMeasurement {
    public bool Primary { get; set; }
}

// Declares that this measurement is the coherent-SI reciprocal of TPartner (this = 1 / TPartner) —
// e.g. Frequency = 1 / Duration, Wavenumber = 1 / Length. The generator emits cross-type read
// properties on both sides' `.To` builders: `reader.To.<Name>` returns the partner (Name defaults to
// the partner's type name) and the reverse `reader.To.<thisTypeName>` returns this type. So a single
// [Reciprocal<Duration>(Name = "Period")] on Frequency yields both `frequency.To.Period` (→ Duration)
// and `period.To.Frequency` (→ Frequency). Declare it on one side of each pair.
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class ReciprocalAttribute<TPartner> : Attribute
    where TPartner : struct, IMeasurement {
    // How this type names its reciprocal in the fluent read (`reader.To.<Name>`); defaults to the
    // partner's type name.
    public string Name { get; set; }
}
