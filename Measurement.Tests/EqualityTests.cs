namespace com.hafthor.Measurement;

[TestClass]
public sealed class EqualityTests {
    [TestMethod]
    public void EqualWhenCanonicalValuesMatch() {
        Assert.IsTrue(Length.Of(5).Meters.Equals(Length.Of(5).Meters));
        // equality is on the canonical value, regardless of the unit used to construct
        Assert.IsTrue(Length.Of(1).Kilo.Meters.Equals(Length.Of(1000).Meters));
        Assert.AreEqual(Length.Of(1000).Meters, Length.Of(1).Kilo.Meters);
    }

    [TestMethod]
    public void NotEqualWhenValuesDiffer() {
        Assert.IsFalse(Length.Of(5).Meters.Equals(Length.Of(6).Meters));
    }

    [TestMethod]
    public void NotEqualAcrossDifferentTypes() {
        object mass = Mass.Of(5).Kilo.Grams;
        Assert.IsFalse(Length.Of(5).Meters.Equals(mass));
    }

    [TestMethod]
    public void EqualValuesShareHashCode() {
        Assert.AreEqual(
            Length.Of(1).Kilo.Meters.GetHashCode(),
            Length.Of(1000).Meters.GetHashCode());
    }

    [TestMethod]
    public void UsableAsDictionaryKey() {
        var seen = new HashSet<Force> { Force.Of(10).Newtons, Force.Of(0.01).Kilo.Newtons };
        Assert.HasCount(1, seen); // 10 N == 0.01 kN
        Assert.Contains(Force.Of(10).Newtons, seen);
    }

    [TestMethod]
    public void NearlyEquals_AllowsUlpSlop() {
        // 0.1 + 0.2 != 0.3 exactly, but they are one ULP apart (sub-nanometre, so below the
        // exact-integer anchor scale where the rounding would otherwise vanish)
        Length a = Length.Of(0.1).Nano.Meters + Length.Of(0.2).Nano.Meters;
        Length b = Length.Of(0.3).Nano.Meters;
        Assert.IsFalse(a.Equals(b));         // exact equality: not equal
        Assert.IsTrue(a.NearlyEquals(b));     // within the default 4 ULPs
        Assert.IsFalse(a.NearlyEquals(b, 0)); // zero slop → strict
    }

    [TestMethod]
    public void NearlyEquals_HandlesExponentBoundary() {
        // 1.0 and the representable value just below it span an exponent boundary (the mantissa
        // rolls over) yet are exactly 1 ULP apart — the bit-trick must count that as 1.
        Length one = Length.FromCanonical(1.0);
        Length below1 = Length.FromCanonical(Math.BitDecrement(1.0));
        Length below2 = Length.FromCanonical(Math.BitDecrement(Math.BitDecrement(1.0)));
        Assert.IsTrue(below1.NearlyEquals(one, 1));
        Assert.IsFalse(below1.NearlyEquals(one, 0));
        Assert.IsFalse(below2.NearlyEquals(one, 1));
        Assert.IsTrue(below2.NearlyEquals(one, 2));
        // opposite signs straddling zero are never nearly-equal
        Assert.IsFalse(Length.FromCanonical(double.Epsilon).NearlyEquals(Length.FromCanonical(-double.Epsilon)));
    }

    [TestMethod]
    public void NearlyEquals_RejectsGenuinelyDifferentValues() {
        Assert.IsFalse(Length.Of(1).Meters.NearlyEquals(Length.Of(2).Meters));
        Assert.IsTrue(Length.Of(5).Meters.NearlyEquals(Length.Of(5).Meters));
    }

    [TestMethod]
    public void RelationalOperators() {
        Assert.IsTrue(Length.Of(1).Meters < Length.Of(2).Meters);
        Assert.IsTrue(Length.Of(1).Kilo.Meters > Length.Of(999).Meters);
        Assert.IsTrue(Length.Of(5).Meters <= Length.Of(5).Meters);
        Assert.IsTrue(Length.Of(5).Meters >= Length.Of(5).Meters);
        Assert.IsFalse(Length.Of(2).Meters < Length.Of(2).Meters);
    }

    [TestMethod]
    public void EqualityOperators() {
        Assert.IsTrue(Length.Of(1).Kilo.Meters == Length.Of(1000).Meters); // exact, unit-independent
        Assert.IsTrue(Length.Of(1).Meters != Length.Of(2).Meters);
        Assert.IsFalse(Length.Of(1).Meters == Length.Of(2).Meters);
    }

    [TestMethod]
    public void SortableAndMinMax() {
        var list = new List<Length> {
            Length.Of(3).Meters, Length.Of(1).Kilo.Meters, Length.Of(2).Meters,
        };
        list.Sort();
        Assert.AreEqual(2.0, list[0].To.Meters);
        Assert.AreEqual(3.0, list[1].To.Meters);
        Assert.AreEqual(1000.0, list[2].To.Meters);
        Assert.AreEqual(1000.0, list.Max().To.Meters);
        Assert.AreEqual(2.0, list.Min().To.Meters);
    }

    [TestMethod]
    public void CompareTo_IsTypeSafe() {
        Assert.AreEqual(-1, Math.Sign(Length.Of(1).Meters.CompareTo(Length.Of(2).Meters)));
        System.IComparable boxed = Length.Of(1).Meters;
        Assert.ThrowsExactly<ArgumentException>(() => boxed.CompareTo(Mass.Of(1).Kilo.Grams));
    }
}
