using System.Numerics;

namespace com.hafthor.Measurement;

// Proves every measurement participates in System.Numerics generic math.
[TestClass]
public sealed class NumericsTests {
    // Fully generic — constrained only to the measurement interface.
    private static T Sum<T>(IEnumerable<T> items) where T : IMeasurement<T> {
        T total = T.AdditiveIdentity;               // true additive identity
        foreach (var x in items) total += x;         // interface addition operator
        return total;
    }

    private static T Average<T>(IReadOnlyCollection<T> items) where T : IMeasurement<T>
        => Sum(items) / items.Count;                 // scalar division operator

    [TestMethod]
    public void GenericSumAndAverage() {
        var lengths = new[] { Length.Of(1).Meters, Length.Of(2).Meters, Length.Of(3).Meters };
        Assert.AreEqual(6.0, Sum(lengths).To.Meters);
        Assert.AreEqual(2.0, Average(lengths).To.Meters);

        var masses = new[] { Mass.Of(10).Kilo.Grams, Mass.Of(20).Kilo.Grams };
        Assert.AreEqual(30.0, Sum(masses).To.Kilo.Grams);
    }

    [TestMethod]
    public void AdditiveIdentityIsZero() {
        Assert.AreEqual(0.0, Length.Zero.To.Meters);
        Assert.AreEqual(0.0, Energy.Zero.To.Joules);
        // via the interface, in a generic setting
        Assert.AreEqual(0.0, ZeroOf<Mass>().To.Kilo.Grams);
    }

    private static T ZeroOf<T>() where T : IMeasurement<T> => T.AdditiveIdentity;

    // Compiles only if T satisfies the full generic-math surface — a compile-time proof.
    private static void RequireNumeric<T>() where T :
        IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IUnaryNegationOperators<T, T>,
        IAdditiveIdentity<T, T>, IComparisonOperators<T, T, bool>,
        IMultiplyOperators<T, double, T>, IDivisionOperators<T, double, T> { }

    [TestMethod]
    public void SatisfiesNumericsInterfaces() {
        RequireNumeric<Length>();
        RequireNumeric<Mass>();
        RequireNumeric<Energy>();
        RequireNumeric<Temperature>();
    }
}
