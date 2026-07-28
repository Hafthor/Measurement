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
