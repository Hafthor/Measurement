namespace com.hafthor.Measurement;

// Carries a scalar already scaled into a unit; prefixes stack, e.g. Measure.Of(1).Mega.Mega.Meters.
public readonly struct Prefixed {
    public readonly double Value;
    internal Prefixed(double value) => Value = value;

    public Prefixed Quetta => new(Value * 1e30);
    public Prefixed Ronna => new(Value * 1e27);
    public Prefixed Yotta => new(Value * 1e24);
    public Prefixed Zetta => new(Value * 1e21);
    public Prefixed Exa => new(Value * 1e18);
    public Prefixed Peta => new(Value * 1e15);
    public Prefixed Tera => new(Value * 1e12);
    public Prefixed Giga => new(Value * 1e9);
    public Prefixed Mega => new(Value * 1e6);
    public Prefixed Kilo => new(Value * 1e3);
    public Prefixed Hecto => new(Value * 1e2);
    public Prefixed Deca => new(Value * 1e1);
    public Prefixed Deci => new(Value * 1e-1);
    public Prefixed Centi => new(Value * 1e-2);
    public Prefixed Milli => new(Value * 1e-3);
    public Prefixed Micro => new(Value * 1e-6);
    public Prefixed Nano => new(Value * 1e-9);
    public Prefixed Pico => new(Value * 1e-12);
    public Prefixed Femto => new(Value * 1e-15);
    public Prefixed Atto => new(Value * 1e-18);
    public Prefixed Zepto => new(Value * 1e-21);
    public Prefixed Yocto => new(Value * 1e-24);
    public Prefixed Ronto => new(Value * 1e-27);
    public Prefixed Quecto => new(Value * 1e-30);
}

// Read-out builder: a measurement plus a running prefix factor, e.g. mass.To.Milli.Grams.
public readonly struct Reader<T> {
    public readonly T Value;
    public readonly double Factor;
    internal Reader(T value, double factor) { Value = value; Factor = factor; }

    public Reader<T> Quetta => new(Value, Factor * 1e30);
    public Reader<T> Ronna => new(Value, Factor * 1e27);
    public Reader<T> Yotta => new(Value, Factor * 1e24);
    public Reader<T> Zetta => new(Value, Factor * 1e21);
    public Reader<T> Exa => new(Value, Factor * 1e18);
    public Reader<T> Peta => new(Value, Factor * 1e15);
    public Reader<T> Tera => new(Value, Factor * 1e12);
    public Reader<T> Giga => new(Value, Factor * 1e9);
    public Reader<T> Mega => new(Value, Factor * 1e6);
    public Reader<T> Kilo => new(Value, Factor * 1e3);
    public Reader<T> Hecto => new(Value, Factor * 1e2);
    public Reader<T> Deca => new(Value, Factor * 1e1);
    public Reader<T> Deci => new(Value, Factor * 1e-1);
    public Reader<T> Centi => new(Value, Factor * 1e-2);
    public Reader<T> Milli => new(Value, Factor * 1e-3);
    public Reader<T> Micro => new(Value, Factor * 1e-6);
    public Reader<T> Nano => new(Value, Factor * 1e-9);
    public Reader<T> Pico => new(Value, Factor * 1e-12);
    public Reader<T> Femto => new(Value, Factor * 1e-15);
    public Reader<T> Atto => new(Value, Factor * 1e-18);
    public Reader<T> Zepto => new(Value, Factor * 1e-21);
    public Reader<T> Yocto => new(Value, Factor * 1e-24);
    public Reader<T> Ronto => new(Value, Factor * 1e-27);
    public Reader<T> Quecto => new(Value, Factor * 1e-30);
}

// Non-double entry point — always available (never extends double): Measure.Of(5).Kilo.Grams.
public static partial class Measure {
    public static Prefixed Of(double value) => new(value);
}

// The measurement-side fluent members (input hooks on Prefixed, the `To` read-out builder, and
// output hooks on Reader<T>) are generated per struct into the partial class Units — see
// MeasurementGenerator.EmitFluent. Only base and non-metric unit names get a direct hook; SI
// prefixes are expressed through the prefix chain (e.g. Measure.Of(1).Kilo.Grams).
public static partial class Units {
}
