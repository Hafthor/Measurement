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
}
