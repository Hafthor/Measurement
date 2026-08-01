namespace com.hafthor.Measurement.Fluent;

// OPT-IN double sugar: SI prefixes and base/non-SI unit shortcuts on `double` (5.0.Kilo.Meters, 5.0.Miles).
// Enable with `using com.hafthor.Measurement.Fluent;`. Without it, `double` is untouched.
public static partial class DoubleSugar {
    extension(double value) {
        public Prefixed Quetta => Measure.Of(value).Quetta;
        public Prefixed Ronna => Measure.Of(value).Ronna;
        public Prefixed Yotta => Measure.Of(value).Yotta;
        public Prefixed Zetta => Measure.Of(value).Zetta;
        public Prefixed Exa => Measure.Of(value).Exa;
        public Prefixed Peta => Measure.Of(value).Peta;
        public Prefixed Tera => Measure.Of(value).Tera;
        public Prefixed Giga => Measure.Of(value).Giga;
        public Prefixed Mega => Measure.Of(value).Mega;
        public Prefixed Kilo => Measure.Of(value).Kilo;
        public Prefixed Hecto => Measure.Of(value).Hecto;
        public Prefixed Deca => Measure.Of(value).Deca;
        public Prefixed Deci => Measure.Of(value).Deci;
        public Prefixed Centi => Measure.Of(value).Centi;
        public Prefixed Milli => Measure.Of(value).Milli;
        public Prefixed Micro => Measure.Of(value).Micro;
        public Prefixed Nano => Measure.Of(value).Nano;
        public Prefixed Pico => Measure.Of(value).Pico;
        public Prefixed Femto => Measure.Of(value).Femto;
        public Prefixed Atto => Measure.Of(value).Atto;
        public Prefixed Zepto => Measure.Of(value).Zepto;
        public Prefixed Yocto => Measure.Of(value).Yocto;
        public Prefixed Ronto => Measure.Of(value).Ronto;
        public Prefixed Quecto => Measure.Of(value).Quecto;
    }
}
