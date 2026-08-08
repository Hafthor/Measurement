using com.hafthor.Measurement.Fluent; // double sugar entry (2.0.Ampere.Hours)

namespace com.hafthor.Measurement;

// Product-unit token algebra: tokens compose through the [Product] relations, so any compatible
// unit works in each slot (e.g. .Joule.Minutes, .Light.Seconds), not just exactly-spelled units.
[TestClass]
public sealed class ProductFluentTests {
    [TestMethod]
    public void AmpereHours_ComposesToCharge() {
        ElectricCharge c = Measure.Of(2).Ampere.Hours;
        Assert.AreEqual(ElectricCharge.Of(2).Ampere.Hours.To.Coulombs, c.To.Coulombs, 1e-9);
    }

    [TestMethod]
    public void LeadingPrefixViaChain() {
        ElectricCharge mAh = Measure.Of(500).Milli.Ampere.Hours;
        Assert.AreEqual(ElectricCharge.Of(500).Milli.Ampere.Hours.To.Coulombs, mAh.To.Coulombs, 1e-9);
        Energy kWh = Measure.Of(3).Kilo.Watt.Hours;
        Assert.AreEqual(Energy.Of(3).Kilo.Watt.Hours.To.Joules, kWh.To.Joules, 1e-6);
    }

    [TestMethod]
    public void FlexibleTimeUnit_JouleMinutes_IsAction() {
        // 1 J·min == 60 J·s — a compatible Duration unit that has no dedicated JouleMinutes factory.
        Action a = Measure.Of(1).Joule.Minutes;
        Assert.AreEqual(Action.Of(60).Joule.Seconds.To.JouleSeconds, a.To.JouleSeconds, 1e-9);
        Action s = Measure.Of(1).Joule.Seconds;
        Assert.AreEqual(1.0, s.To.JouleSeconds, 1e-12);
    }

    [TestMethod]
    public void SpeedOfLightToken_LightSecondsAndLightYears() {
        // .Light is the speed of light; × a Duration gives a Length.
        Length ls = Measure.Of(1).Light.Seconds;
        Assert.AreEqual(299792458.0, ls.To.Meters, 1e-3);
        // an "Annum" is the Julian year, so .Light.Annums is a light-year.
        Length ly = Measure.Of(1).Light.Annums;
        Assert.AreEqual(Length.Of(1).Light.Years.To.Meters, ly.To.Meters, 1.0);
    }

    [TestMethod]
    public void AmbiguousProduct_NewtonMeters_SelectsTorqueOrEnergy() {
        // Force × Length is dimensionally both Torque and Energy → the product names each.
        Torque t = Measure.Of(5).Newton.Meters.Torque;
        Assert.AreEqual(5.0, t.To.NewtonMeters, 1e-12);
        Energy e = Measure.Of(5).Newton.Meters.Energy;
        Assert.AreEqual(5.0, e.To.Joules, 1e-12);
    }

    [TestMethod]
    public void RunningStateIsUsableAtEachStep() {
        // The first token alone is already the base measurement (implicit conversion).
        Force f = Measure.Of(10).Newton;
        Assert.AreEqual(10.0, f.To.Newtons, 1e-12);
        ElectricCurrent i = Measure.Of(3).Ampere;
        Assert.AreEqual(3.0, i.To.Amperes, 1e-12);
    }

    [TestMethod]
    public void DoubleSugar_ProductWalk() {
        ElectricCharge c = 2.0.Ampere.Hours;
        Assert.AreEqual(ElectricCharge.Of(2).Ampere.Hours.To.Coulombs, c.To.Coulombs, 1e-9);
    }

    [TestMethod]
    public void SquareCubic_ComposeWithPrefixes() {
        // .Square.<length> composes to Area without needing a dedicated .SquareMillimeters hook.
        Area sm = Measure.Of(4).Square.Meters;
        Assert.AreEqual(4.0, sm.To.Square.Meters, 1e-12);
        Area smm = Measure.Of(1).Square.Milli.Meters;                 // 1 mm² = 1e-6 m²
        Assert.AreEqual(1e-6, smm.To.Square.Meters, 1e-18);
        Area scm = Measure.Of(1).Square.Centi.Meters;                 // 1 cm² = 1e-4 m²
        Assert.AreEqual(1e-4, scm.To.Square.Meters, 1e-16);
        Volume ccm = Measure.Of(1).Cubic.Centi.Meters;               // 1 cm³ = 1e-6 m³
        Assert.AreEqual(1e-6, ccm.To.Cubic.Meters, 1e-18);
        SolidAngle sd = Measure.Of(1).Square.Degrees;                 // .Square.Degrees → SolidAngle
        Assert.AreEqual(SolidAngle.Of(1).Square.Degrees.To.Steradians, sd.To.Steradians, 1e-12);
    }

    [TestMethod]
    public void SquareCubic_ExtendProductStates() {
        // A running product state can be scaled by an areal/cubic unit via the [Product] graph:
        // Mass × Area → MomentOfInertia, so .Kilo.Gram.Square.Meters is a moment of inertia.
        MomentOfInertia moi = Measure.Of(1).Kilo.Gram.Square.Meters;
        Assert.AreEqual(MomentOfInertia.Of(1).Kilo.Gram.Square.Meters.To.Kilo.GramSquareMeters,
            moi.To.Kilo.GramSquareMeters, 1e-9);
        // Prefixed length inside the modifier walk works too: 1 kg·cm² = 1e-4 kg·m².
        MomentOfInertia moiCm = Measure.Of(1).Kilo.Gram.Square.Centi.Meters;
        Assert.AreEqual(1e-4, moiCm.To.Kilo.GramSquareMeters, 1e-16);
        // .Gram alone (unprefixed) enters the Mass product state: 1 g·m².
        MomentOfInertia moiG = Measure.Of(1).Gram.Square.Meters;
        Assert.AreEqual(1e-3, moiG.To.Kilo.GramSquareMeters, 1e-9);
    }

    [TestMethod]
    public void TypedEntry_ConstrainsToTheType() {
        // T.Of(v) is a fluent entry limited to ways of constructing T (its own units only).
        Area a = Area.Of(4).Square.Milli.Meters;
        Assert.AreEqual(4.0, a.To.Square.Milli.Meters, 1e-9);
        Area am = Area.Of(2).Square.Meters;
        Assert.AreEqual(2.0, am.To.Square.Meters, 1e-12);
        Area ha = Area.Of(3).Hectares;                      // flat non-square area unit
        Assert.AreEqual(Area.Of(3).Hectares.To.Square.Meters, ha.To.Square.Meters, 1e-6);
        // prefix chain and non-SI units for other types
        Length km = Length.Of(5).Kilo.Meters;
        Assert.AreEqual(5000.0, km.To.Meters, 1e-9);
        Length mi = Length.Of(1).Miles;
        Assert.AreEqual(Length.Of(1).Miles.To.Meters, mi.To.Meters, 1e-6);
        Mass kg = Mass.Of(2).Kilo.Grams;
        Assert.AreEqual(2.0, kg.To.Kilo.Grams, 1e-12);
        // compound (Per) units compose too, still constrained to the type
        Speed s = Speed.Of(10).Meters.Per.Second;
        Assert.AreEqual(10.0, s.To.Meters.Per.Second, 1e-12);
        Volume cc = Volume.Of(1).Cubic.Centi.Meters;
        Assert.AreEqual(1e-6, cc.To.Cubic.Meters, 1e-18);
    }

    [TestMethod]
    public void TypedEntry_OffersFullSiPrefixRange() {
        // The typed .Of entry exposes every SI prefix for a prefix-expandable unit, not just the
        // curated flat-hook subset (Length declares only None/Kilo/Centi/Milli/Micro/Nano).
        Length ym = Length.Of(2).Yotta.Meters;               // Yotta not in Length's declared list
        Assert.AreEqual(2e24, ym.To.Meters, 1);
        Length qm = Length.Of(3).Quecto.Meters;
        Assert.AreEqual(3e-30, qm.To.Meters, 1e-40);
        // stacks with the universal read chain, and round-trips
        Assert.AreEqual(5.0, Length.Of(5).Giga.Meters.To.Giga.Meters, 1e-9);
        // works for a prefixed base whose coherent unit is itself prefixed (Mass stores grams)
        Mass mg = Mass.Of(4).Mega.Grams;                     // Mega not in Mass's declared list
        Assert.AreEqual(4e6, mg.To.Grams, 1e-6);
        // collision type: .Kilo also carries a declared prefix-leading unit (Kilocalories) yet the
        // expandable base (Joules) is reachable, scaled, under the same .Kilo
        Energy kj = Energy.Of(2).Kilo.Joules;
        Assert.AreEqual(2000.0, kj.To.Joules, 1e-9);
        Energy kcal = Energy.Of(1).Kilo.Calories;            // declared Kilocalories path still works
        Assert.AreEqual(4184.0, kcal.To.Joules, 1e-6);
        // compound expandable: .Kilo.Grams.Per.Second and the declared .Kilo.Grams.Per.Hour coexist
        MassFlowRate kgs = MassFlowRate.Of(2).Kilo.Grams.Per.Second;
        Assert.AreEqual(2000.0, kgs.To.Grams.Per.Second, 1e-9);
        MassFlowRate kgh = MassFlowRate.Of(3600).Kilo.Grams.Per.Hour;
        Assert.AreEqual(1000.0, kgh.To.Grams.Per.Second, 1e-6);
    }

    [TestMethod]
    public void SquareCubic_OfferFullSiPrefixRange() {
        // Square/Cubic areal/cubic units now accept the whole SI prefix range on both construction
        // (Measure.Of / double entry) and read, folded into the value/factor as an exact power of ten
        // (n·prefixExp), not just the declared SquareKilometers/CubicCentimeters set.
        Area a = Measure.Of(3).Square.Kilo.Meters;          // km² = 1e6 m²
        Assert.AreEqual(3e6, a.To.Square.Meters, 1e-3);
        Assert.AreEqual(3.0, a.To.Square.Kilo.Meters, 1e-9);
        // a prefix that was never declared for area, on both construction and read
        Area ga = Measure.Of(2).Square.Giga.Meters;         // (Gm)² = 1e18 m²
        Assert.AreEqual(2e18, ga.To.Square.Meters, 1e6);
        Assert.AreEqual(2.0, ga.To.Square.Giga.Meters, 1e-9);
        // cubic, undeclared prefix, round-trip through the read side
        Volume v = Measure.Of(5).Cubic.Milli.Meters;        // mm³ = 1e-9 m³
        Assert.AreEqual(5e-9, v.To.Cubic.Meters, 1e-18);
        Volume vm = Measure.Of(5).Cubic.Mega.Meters;
        Assert.AreEqual(5.0, vm.To.Cubic.Mega.Meters, 1e-9);
        // exactness preserved for the declared cases (SquareCentimeters etc.)
        Area ac = Measure.Of(1).Square.Centi.Meters;
        Assert.AreEqual(1e-4, ac.To.Square.Meters, 1e-16);
        // typed entry is universal too now: Area.Of(x).Square.<anyPrefix>.Meters
        Area ta = Area.Of(2).Square.Giga.Meters;            // (Gm)² = 1e18 m²
        Assert.AreEqual(2e18, ta.To.Square.Meters, 1e6);
        Volume tv = Volume.Of(4).Cubic.Micro.Meters;        // µm³ = 1e-18 m³
        Assert.AreEqual(4e-18, tv.To.Cubic.Meters, 1e-28);
    }
}
