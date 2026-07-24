namespace com.hafthor.Measurement;

[TestClass]
public sealed class EqualityTests {
    [TestMethod]
    public void EqualWhenCanonicalValuesMatch() {
        Assert.IsTrue(Length.FromMeters(5).Equals(Length.FromMeters(5)));
        // equality is on the canonical value, regardless of the unit used to construct
        Assert.IsTrue(Length.FromKilometers(1).Equals(Length.FromMeters(1000)));
        Assert.AreEqual(Length.FromMeters(1000), Length.FromKilometers(1));
    }

    [TestMethod]
    public void NotEqualWhenValuesDiffer() {
        Assert.IsFalse(Length.FromMeters(5).Equals(Length.FromMeters(6)));
    }

    [TestMethod]
    public void NotEqualAcrossDifferentTypes() {
        object mass = Mass.FromKilograms(5);
        Assert.IsFalse(Length.FromMeters(5).Equals(mass));
    }

    [TestMethod]
    public void EqualValuesShareHashCode() {
        Assert.AreEqual(
            Length.FromKilometers(1).GetHashCode(),
            Length.FromMeters(1000).GetHashCode());
    }

    [TestMethod]
    public void UsableAsDictionaryKey() {
        var seen = new HashSet<Force> { Force.FromNewtons(10), Force.FromKilonewtons(0.01) };
        Assert.HasCount(1, seen); // 10 N == 0.01 kN
        Assert.Contains(Force.FromNewtons(10), seen);
    }

    [TestMethod]
    public void NearlyEquals_AllowsUlpSlop() {
        // 0.1 + 0.2 != 0.3 exactly, but they are one ULP apart (sub-nanometre, so below the
        // exact-integer anchor scale where the rounding would otherwise vanish)
        Length a = Length.FromNanometers(0.1) + Length.FromNanometers(0.2);
        Length b = Length.FromNanometers(0.3);
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
        Assert.IsFalse(Length.FromMeters(1).NearlyEquals(Length.FromMeters(2)));
        Assert.IsTrue(Length.FromMeters(5).NearlyEquals(Length.FromMeters(5)));
    }

    [TestMethod]
    public void RelationalOperators() {
        Assert.IsTrue(Length.FromMeters(1) < Length.FromMeters(2));
        Assert.IsTrue(Length.FromKilometers(1) > Length.FromMeters(999));
        Assert.IsTrue(Length.FromMeters(5) <= Length.FromMeters(5));
        Assert.IsTrue(Length.FromMeters(5) >= Length.FromMeters(5));
        Assert.IsFalse(Length.FromMeters(2) < Length.FromMeters(2));
    }

    [TestMethod]
    public void EqualityOperators() {
        Assert.IsTrue(Length.FromKilometers(1) == Length.FromMeters(1000)); // exact, unit-independent
        Assert.IsTrue(Length.FromMeters(1) != Length.FromMeters(2));
        Assert.IsFalse(Length.FromMeters(1) == Length.FromMeters(2));
    }

    [TestMethod]
    public void SortableAndMinMax() {
        var list = new List<Length> {
            Length.FromMeters(3), Length.FromKilometers(1), Length.FromMeters(2),
        };
        list.Sort();
        Assert.AreEqual(2.0, list[0].ToMeters());
        Assert.AreEqual(3.0, list[1].ToMeters());
        Assert.AreEqual(1000.0, list[2].ToMeters());
        Assert.AreEqual(1000.0, list.Max().ToMeters());
        Assert.AreEqual(2.0, list.Min().ToMeters());
    }

    [TestMethod]
    public void CompareTo_IsTypeSafe() {
        Assert.AreEqual(-1, Math.Sign(Length.FromMeters(1).CompareTo(Length.FromMeters(2))));
        System.IComparable boxed = Length.FromMeters(1);
        Assert.ThrowsExactly<ArgumentException>(() => boxed.CompareTo(Mass.FromKilograms(1)));
    }
}
