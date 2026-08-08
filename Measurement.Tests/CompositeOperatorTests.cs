namespace com.hafthor.Measurement;

// Operator results are computed in base units; with round-number inputs the
// results are exactly representable, so exact equality is asserted.
[TestClass]
public sealed class CompositeOperatorTests {
    [TestMethod]
    public void Kinematics() {
        Speed s = Length.Of(100).Meters / Duration.Of(10).Seconds;
        Assert.AreEqual(10.0, s.To.Meters.Per.Second);
        Length l = s * Duration.Of(3).Seconds;
        Assert.AreEqual(30.0, l.To.Meters);
        Acceleration a = s / Duration.Of(2).Seconds;
        Assert.AreEqual(5.0, a.To.Meters.Per.Second.Squared);
        Speed s2 = a * Duration.Of(4).Seconds;
        Assert.AreEqual(20.0, s2.To.Meters.Per.Second);
    }

    [TestMethod]
    public void Geometry() {
        Area a = Length.Of(4).Meters * Length.Of(5).Meters;
        Assert.AreEqual(20.0, a.To.Square.Meters);
        Volume v = a * Length.Of(2).Meters;
        Assert.AreEqual(40.0, v.To.Cubic.Meters);
        Assert.AreEqual(5.0, (a / Length.Of(4).Meters).To.Meters);
        Assert.AreEqual(20.0, (v / Length.Of(2).Meters).To.Square.Meters);
    }

    [TestMethod]
    public void Mechanics() {
        Force f = Mass.Of(2).Kilo.Grams * Acceleration.Of(3).Meters.Per.Second.Squared;
        Assert.AreEqual(6.0, f.To.Newtons);
        Assert.AreEqual(3.0, (f / Mass.Of(2).Kilo.Grams).To.Meters.Per.Second.Squared);
        Assert.AreEqual(2.0, (f / Acceleration.Of(3).Meters.Per.Second.Squared).To.Kilo.Grams);
        Energy e = f * Length.Of(5).Meters;   // implicit conversion to the primary (Energy)
        Assert.AreEqual(30.0, e.To.Joules);
        Power p = e / Duration.Of(2).Seconds;
        Assert.AreEqual(15.0, p.To.Watts);
        Pressure pr = Force.Of(100).Newtons / Area.Of(4).Square.Meters;
        Assert.AreEqual(25.0, pr.To.Pascals);
        Power p2 = Force.Of(2).Newtons * Speed.Of(3).Meters.Per.Second;
        Assert.AreEqual(6.0, p2.To.Watts);
    }

    [TestMethod]
    public void ForceTimesLength_SelectorPicksEnergyOrTorque() {
        // Force × Length is dimensionally ambiguous (J ≡ N·m), so the product is a selector that
        // names the intended result rather than silently choosing one.
        var product = Force.Of(2).Newtons * Length.Of(3).Meters;
        Energy implicitlyEnergy = product;                 // implicit conversion to the primary result
        Assert.AreEqual(6.0, implicitlyEnergy.To.Joules);
        Assert.AreEqual(6.0, product.Energy.To.Joules);
        Assert.AreEqual(6.0, product.Torque.To.NewtonMeters);
        // commutative order yields the same selector
        Assert.AreEqual(6.0, (Length.Of(3).Meters * Force.Of(2).Newtons).Torque.To.NewtonMeters);
    }

    // E = m c^2 : Mass × Speed = Momentum, then Momentum × Speed = Energy,
    // so `mass * c * c` chains into an Energy directly.
    [TestMethod]
    public void MassEnergyEquivalence() {
        Energy avgHouseYear = Measure.Of(12.5).Mega.WattHours;
        Speed c = Speed.Of(1).Speed.Of.Light;
        Energy e = Mass.Of(1).Grams * c * c; // a heavy paper clip converted to pure energy
        // 1 g ≈ 8.9875517873681764e13 J (c = 299792458 m/s exactly)
        Assert.AreEqual(299792458.0 * 299792458.0 * 1e-3, e.To.Joules); // wattseconds
        Assert.AreEqual(1997.2337305262615, e / avgHouseYear); // that's enough for 2000 average houses for a year
        // and the inverse relations recover mass and speed
        Assert.AreEqual(1.0, (e / c / c).To.Grams);
        Assert.AreEqual(299792458.0, (e / (Mass.Of(1).Grams * c)).To.Meters.Per.Second);

        // nuclear fission (total energy of 1g of U-235, initial fissle products only)
        Mass neutron = Mass.Of(1.008664891).Daltons, u235 = Mass.Of(235.043928).Daltons;
        Mass kr92 = Mass.Of(91.9261731).Daltons, ba141 = Mass.Of(140.914403).Daltons;
        e = ((neutron + u235) - (kr92 + ba141 + neutron * 3)) * c * c * (Mass.Of(1).Grams / u235);
        Assert.AreEqual(1.580681755342055, e / avgHouseYear); // that's roughly 1.6 average houses for a year

        // nuclear fusion (total energy from 1g of hydrogen)
        Mass h2 = Mass.Of(2.01410177812).Daltons, he4 = Mass.Of(4.00260325415).Daltons;
        e = (h2 * 2 - he4) * c * c * (Mass.Of(1).Grams / h2);
        Assert.AreEqual(25.38590025650838, e / avgHouseYear); // that's roughly 25.4 average houses for a year
    }

    // E = h f : Planck's constant (an Action, J·s) × Frequency (1/s) = Energy.
    [TestMethod]
    public void PlanckEinsteinRelation() {
        Action h = Action.Of(1).Planck.Constants;
        Frequency f = Frequency.Of(5e14).Hertz;
        Energy e = h * f;
        Assert.AreEqual(6.62607015e-34 * 5e14, e.To.Joules);
        // inverse relations recover the constant and the frequency
        Assert.AreEqual(5e14, (e / h).To.Hertz);
        Assert.AreEqual(1.0, (e / f).To.PlanckConstants);
    }

    [TestMethod]
    public void Electromagnetism() {
        Voltage v = ElectricCurrent.Of(2).Amperes * ElectricResistance.Of(3).Ohms;
        Assert.AreEqual(6.0, v.To.Volts);
        Assert.AreEqual(2.0, (v / ElectricResistance.Of(3).Ohms).To.Amperes);
        Assert.AreEqual(3.0, (v / ElectricCurrent.Of(2).Amperes).To.Ohms);
        Power p = ElectricCurrent.Of(2).Amperes * Voltage.Of(6).Volts;
        Assert.AreEqual(12.0, p.To.Watts);
        ElectricCharge q = ElectricCurrent.Of(2).Amperes * Duration.Of(5).Seconds;
        Assert.AreEqual(10.0, q.To.Coulombs);
        Capacitance c = ElectricCharge.Of(6).Coulombs / Voltage.Of(3).Volts;
        Assert.AreEqual(2.0, c.To.Farads);
        MagneticFlux flux = Voltage.Of(4).Volts * Duration.Of(2).Seconds;
        Assert.AreEqual(8.0, flux.To.Webers);
        MagneticFluxDensity b = flux / Area.Of(4).Square.Meters;
        Assert.AreEqual(2.0, b.To.Teslas);
        LinearMagneticFluxDensity lmb = flux / Length.Of(32e6).Meters;
        Assert.AreEqual(250.0, lmb.To.Nano.Webers.Per.Meter, 1e-13); // 1000 Hz reference fluxivity
    }

    [TestMethod]
    public void Photometry() {
        LuminousFlux lf = LuminousIntensity.Of(2).Candelas * SolidAngle.Of(3).Steradians;
        Assert.AreEqual(6.0, lf.To.Lumens);
        Illuminance ill = lf / Area.Of(2).Square.Meters;
        Assert.AreEqual(3.0, ill.To.Lux);
        LuminousIntensity i = lf / SolidAngle.Of(3).Steradians;
        Assert.AreEqual(2.0, i.To.Candelas);
    }

    [TestMethod]
    public void MassDerivedAndChemistry() {
        Density d = Mass.Of(20).Kilo.Grams / Volume.Of(4).Cubic.Meters;
        Assert.AreEqual(5.0, d.To.Kilo.Grams.Per.Cubic.Meter);
        Assert.AreEqual(10.0, (d * Volume.Of(2).Cubic.Meters).To.Kilo.Grams);
        Momentum mom = Mass.Of(3).Kilo.Grams * Speed.Of(4).Meters.Per.Second;
        Assert.AreEqual(12.0, mom.To.Kilo.GramMetersPerSecond);
        Concentration conc = Quantity.Of(6).Moles / Volume.Of(3).Cubic.Meters;
        Assert.AreEqual(2.0, conc.To.Moles.Per.Cubic.Meter);
        ReactionRate r = conc / Duration.Of(4).Seconds;
        Assert.AreEqual(0.5, r.To.Moles.Per.Cubic.Meter.Second);
        DoseRate dr = AbsorbedDose.Of(9).Grays / Duration.Of(3).Seconds;
        Assert.AreEqual(3.0, dr.To.Grays.Per.Second);
        MomentOfInertia moi = Mass.Of(2).Kilo.Grams * Area.Of(3).Square.Meters;
        AngularMomentum am = moi * AngularVelocity.Of(5).Radians.Per.Second;
        Assert.AreEqual(30.0, am.To.Kilo.GramSquareMetersPerSecond);
    }
}
