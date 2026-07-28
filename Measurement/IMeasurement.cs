using System.Numerics;

namespace com.hafthor.Measurement;

// Opts every measurement into System.Numerics generic math: addition, subtraction, negation,
// additive identity, comparison/equality, and scaling by a scalar. Default implementations are
// supplied here (explicit form); each measurement struct provides the static FromCanonical
// factory and CanonicalValue (both emitted by the source generator).
public interface IMeasurement<T> :
    IAdditionOperators<T, T, T>,
    ISubtractionOperators<T, T, T>,
    IUnaryNegationOperators<T, T>,
    IAdditiveIdentity<T, T>,
    IMultiplicativeIdentity<T, T>,
    IComparisonOperators<T, T, bool>,
    IMultiplyOperators<T, double, T>,
    IDivisionOperators<T, double, T>
    where T : IMeasurement<T> {
    static abstract T FromCanonical(double canonicalValue);
    double CanonicalValue { get; }

    static T IAdditionOperators<T, T, T>.operator +(T a, T b) => T.FromCanonical(a.CanonicalValue + b.CanonicalValue);
    static T ISubtractionOperators<T, T, T>.operator -(T a, T b) => T.FromCanonical(a.CanonicalValue - b.CanonicalValue);
    static T IUnaryNegationOperators<T, T>.operator -(T a) => T.FromCanonical(-a.CanonicalValue);
    static T IAdditiveIdentity<T, T>.AdditiveIdentity => T.FromCanonical(0);
    static T IMultiplyOperators<T, double, T>.operator *(T a, double factor) => T.FromCanonical(a.CanonicalValue * factor);
    static T IMultiplicativeIdentity<T, T>.MultiplicativeIdentity => T.FromCanonical(1);
    static T IDivisionOperators<T, double, T>.operator /(T a, double divisor) => T.FromCanonical(a.CanonicalValue / divisor);
    static bool IComparisonOperators<T, T, bool>.operator <(T a, T b) => a.CanonicalValue < b.CanonicalValue;
    static bool IComparisonOperators<T, T, bool>.operator >(T a, T b) => a.CanonicalValue > b.CanonicalValue;
    static bool IComparisonOperators<T, T, bool>.operator <=(T a, T b) => a.CanonicalValue <= b.CanonicalValue;
    static bool IComparisonOperators<T, T, bool>.operator >=(T a, T b) => a.CanonicalValue >= b.CanonicalValue;
    static bool IEqualityOperators<T, T, bool>.operator ==(T a, T b) => a is null ? b is null : b is not null && a.CanonicalValue == b.CanonicalValue;
    static bool IEqualityOperators<T, T, bool>.operator !=(T a, T b) => !(a == b);
}
