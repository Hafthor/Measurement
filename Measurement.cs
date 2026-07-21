namespace com.hafthor.Measurement;

// Common base for every measurement. Holds the canonical value and centralises value
// equality, hashing, ToString, and the same-type +, - and unary-negation operators.
// The self type T (CRTP) lets those operators return the concrete measurement; each class
// supplies a Create factory and its canonical unit Symbol.
public abstract class Measurement<T> where T : Measurement<T> {
    protected readonly double value;

    protected Measurement(double value) => this.value = value;

    protected abstract T Create(double value);
    protected abstract string Symbol { get; }

    public override string ToString() => Symbol.Length == 0 ? $"{value}" : $"{value} {Symbol}";
    public override bool Equals(object obj) => obj != null && obj.GetType() == GetType() && ((Measurement<T>)obj).value == value;
    public override int GetHashCode() => value.GetHashCode();

    public static T operator +(Measurement<T> a, Measurement<T> b) => a.Create(a.value + b.value);
    public static T operator -(Measurement<T> a, Measurement<T> b) => a.Create(a.value - b.value);
    public static T operator -(Measurement<T> a) => a.Create(-a.value);
}
