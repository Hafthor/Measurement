using com.hafthor.Measurement.Fluent; // double sugar entry (2.0.Ampere.Hours)

namespace com.hafthor.Measurement;

// Product-unit token algebra: tokens compose through the [Product] relations, so any compatible
// unit works in each slot (e.g. .Joule.Minutes, .Light.Seconds), not just exactly-spelled units.
[TestClass]
public sealed class ProductFluentTests {
    [TestMethod]
    public void AmpereHours_ComposesToCharge() {
        ElectricCharge c = Measure.Of(2).Ampere.Hours;
        Assert.AreEqual(ElectricCharge.FromAmpereHours(2).ToCoulombs(), c.ToCoulombs(), 1e-9);
    }

    [TestMethod]
    public void LeadingPrefixViaChain() {
        ElectricCharge mAh = Measure.Of(500).Milli.Ampere.Hours;
        Assert.AreEqual(ElectricCharge.FromMilliampereHours(500).ToCoulombs(), mAh.ToCoulombs(), 1e-9);
        Energy kWh = Measure.Of(3).Kilo.Watt.Hours;
        Assert.AreEqual(Energy.FromKilowattHours(3).ToJoules(), kWh.ToJoules(), 1e-6);
    }

    [TestMethod]
    public void FlexibleTimeUnit_JouleMinutes_IsAction() {
        // 1 J·min == 60 J·s — a compatible Duration unit that has no dedicated JouleMinutes factory.
        Action a = Measure.Of(1).Joule.Minutes;
        Assert.AreEqual(Action.FromJouleSeconds(60).ToJouleSeconds(), a.ToJouleSeconds(), 1e-9);
        Action s = Measure.Of(1).Joule.Seconds;
        Assert.AreEqual(1.0, s.ToJouleSeconds(), 1e-12);
    }

    [TestMethod]
    public void SpeedOfLightToken_LightSecondsAndLightYears() {
        // .Light is the speed of light; × a Duration gives a Length.
        Length ls = Measure.Of(1).Light.Seconds;
        Assert.AreEqual(299792458.0, ls.ToMeters(), 1e-3);
        // an "Annum" is the Julian year, so .Light.Annums is a light-year.
        Length ly = Measure.Of(1).Light.Annums;
        Assert.AreEqual(Length.FromLightYears(1).ToMeters(), ly.ToMeters(), 1.0);
    }

    [TestMethod]
    public void AmbiguousProduct_NewtonMeters_SelectsTorqueOrEnergy() {
        // Force × Length is dimensionally both Torque and Energy → the product names each.
        Torque t = Measure.Of(5).Newton.Meters.Torque;
        Assert.AreEqual(5.0, t.ToNewtonMeters(), 1e-12);
        Energy e = Measure.Of(5).Newton.Meters.Energy;
        Assert.AreEqual(5.0, e.ToJoules(), 1e-12);
    }

    [TestMethod]
    public void RunningStateIsUsableAtEachStep() {
        // The first token alone is already the base measurement (implicit conversion).
        Force f = Measure.Of(10).Newton;
        Assert.AreEqual(10.0, f.ToNewtons(), 1e-12);
        ElectricCurrent i = Measure.Of(3).Ampere;
        Assert.AreEqual(3.0, i.ToAmperes(), 1e-12);
    }

    [TestMethod]
    public void DoubleSugar_ProductWalk() {
        ElectricCharge c = 2.0.Ampere.Hours;
        Assert.AreEqual(ElectricCharge.FromAmpereHours(2).ToCoulombs(), c.ToCoulombs(), 1e-9);
    }

    [TestMethod]
    public void SquareCubic_ComposeWithPrefixes() {
        // .Square.<length> composes to Area without needing a dedicated .SquareMillimeters hook.
        Area sm = Measure.Of(4).Square.Meters;
        Assert.AreEqual(4.0, sm.ToSquareMeters(), 1e-12);
        Area smm = Measure.Of(1).Square.Milli.Meters;                 // 1 mm² = 1e-6 m²
        Assert.AreEqual(1e-6, smm.ToSquareMeters(), 1e-18);
        Area scm = Measure.Of(1).Square.Centi.Meters;                 // 1 cm² = 1e-4 m²
        Assert.AreEqual(1e-4, scm.ToSquareMeters(), 1e-16);
        Volume ccm = Measure.Of(1).Cubic.Centi.Meters;               // 1 cm³ = 1e-6 m³
        Assert.AreEqual(1e-6, ccm.ToCubicMeters(), 1e-18);
        SolidAngle sd = Measure.Of(1).Square.Degrees;                 // .Square.Degrees → SolidAngle
        Assert.AreEqual(SolidAngle.FromSquareDegrees(1).ToSteradians(), sd.ToSteradians(), 1e-12);
    }

    [TestMethod]
    public void SquareCubic_ExtendProductStates() {
        // A running product state can be scaled by an areal/cubic unit via the [Product] graph:
        // Mass × Area → MomentOfInertia, so .Kilo.Gram.Square.Meters is a moment of inertia.
        MomentOfInertia moi = Measure.Of(1).Kilo.Gram.Square.Meters;
        Assert.AreEqual(MomentOfInertia.FromKilogramSquareMeters(1).ToKilogramSquareMeters(),
            moi.ToKilogramSquareMeters(), 1e-9);
        // Prefixed length inside the modifier walk works too: 1 kg·cm² = 1e-4 kg·m².
        MomentOfInertia moiCm = Measure.Of(1).Kilo.Gram.Square.Centi.Meters;
        Assert.AreEqual(1e-4, moiCm.ToKilogramSquareMeters(), 1e-16);
        // .Gram alone (unprefixed) enters the Mass product state: 1 g·m².
        MomentOfInertia moiG = Measure.Of(1).Gram.Square.Meters;
        Assert.AreEqual(1e-3, moiG.ToKilogramSquareMeters(), 1e-9);
    }

    [TestMethod]
    public void TypedEntry_ConstrainsToTheType() {
        // T.Of(v) is a fluent entry limited to ways of constructing T (its own units only).
        Area a = Area.Of(4).Square.Milli.Meters;
        Assert.AreEqual(4.0, a.ToSquareMillimeters(), 1e-9);
        Area am = Area.Of(2).Square.Meters;
        Assert.AreEqual(2.0, am.ToSquareMeters(), 1e-12);
        Area ha = Area.Of(3).Hectares;                      // flat non-square area unit
        Assert.AreEqual(Area.FromHectares(3).ToSquareMeters(), ha.ToSquareMeters(), 1e-6);
        // prefix chain and non-SI units for other types
        Length km = Length.Of(5).Kilo.Meters;
        Assert.AreEqual(5000.0, km.ToMeters(), 1e-9);
        Length mi = Length.Of(1).Miles;
        Assert.AreEqual(Length.FromMiles(1).ToMeters(), mi.ToMeters(), 1e-6);
        Mass kg = Mass.Of(2).Kilo.Grams;
        Assert.AreEqual(2.0, kg.ToKilograms(), 1e-12);
        // compound (Per) units compose too, still constrained to the type
        Speed s = Speed.Of(10).Meters.Per.Second;
        Assert.AreEqual(10.0, s.ToMetersPerSecond(), 1e-12);
        Volume cc = Volume.Of(1).Cubic.Centi.Meters;
        Assert.AreEqual(1e-6, cc.ToCubicMeters(), 1e-18);
    }
}
