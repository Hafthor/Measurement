namespace com.hafthor.Measurement;

// Operator results are computed in base units; with round-number inputs the
// results are exactly representable, so exact equality is asserted.
[TestClass]
public sealed class CompositeOperatorTests {
    [TestMethod]
    public void Kinematics() {
        Speed s = Length.FromMeters(100) / Duration.FromSeconds(10);
        Assert.AreEqual(10.0, s.ToMetersPerSecond());
        Length l = s * Duration.FromSeconds(3);
        Assert.AreEqual(30.0, l.ToMeters());
        Acceleration a = s / Duration.FromSeconds(2);
        Assert.AreEqual(5.0, a.ToMetersPerSecondSquared());
        Speed s2 = a * Duration.FromSeconds(4);
        Assert.AreEqual(20.0, s2.ToMetersPerSecond());
    }

    [TestMethod]
    public void Geometry() {
        Area a = Length.FromMeters(4) * Length.FromMeters(5);
        Assert.AreEqual(20.0, a.ToSquareMeters());
        Volume v = a * Length.FromMeters(2);
        Assert.AreEqual(40.0, v.ToCubicMeters());
        Assert.AreEqual(5.0, (a / Length.FromMeters(4)).ToMeters());
        Assert.AreEqual(20.0, (v / Length.FromMeters(2)).ToSquareMeters());
    }

    [TestMethod]
    public void Mechanics() {
        Force f = Mass.FromKilograms(2) * Acceleration.FromMetersPerSecondSquared(3);
        Assert.AreEqual(6.0, f.ToNewtons());
        Assert.AreEqual(3.0, (f / Mass.FromKilograms(2)).ToMetersPerSecondSquared());
        Assert.AreEqual(2.0, (f / Acceleration.FromMetersPerSecondSquared(3)).ToKilograms());
        Energy e = f * Length.FromMeters(5);   // implicit conversion to the primary (Energy)
        Assert.AreEqual(30.0, e.ToJoules());
        Power p = e / Duration.FromSeconds(2);
        Assert.AreEqual(15.0, p.ToWatts());
        Pressure pr = Force.FromNewtons(100) / Area.FromSquareMeters(4);
        Assert.AreEqual(25.0, pr.ToPascals());
        Power p2 = Force.FromNewtons(2) * Speed.FromMetersPerSecond(3);
        Assert.AreEqual(6.0, p2.ToWatts());
    }

    [TestMethod]
    public void ForceTimesLength_SelectorPicksEnergyOrTorque() {
        // Force × Length is dimensionally ambiguous (J ≡ N·m), so the product is a selector that
        // names the intended result rather than silently choosing one.
        var product = Force.FromNewtons(2) * Length.FromMeters(3);
        Energy implicitlyEnergy = product;                 // implicit conversion to the primary result
        Assert.AreEqual(6.0, implicitlyEnergy.ToJoules());
        Assert.AreEqual(6.0, product.Energy.ToJoules());
        Assert.AreEqual(6.0, product.Torque.ToNewtonMeters());
        // commutative order yields the same selector
        Assert.AreEqual(6.0, (Length.FromMeters(3) * Force.FromNewtons(2)).Torque.ToNewtonMeters());
    }

    // E = m c^2 : Mass × Speed = Momentum, then Momentum × Speed = Energy,
    // so `mass * c * c` chains into an Energy directly.
    [TestMethod]
    public void MassEnergyEquivalence() {
        Energy avgHouseYear = Measure.Of(12.5).Mega.WattHours;
        Speed c = Speed.FromSpeedOfLight(1);
        Energy e = Mass.FromGrams(1) * c * c; // a heavy paper clip converted to pure energy
        // 1 g ≈ 8.9875517873681764e13 J (c = 299792458 m/s exactly)
        Assert.AreEqual(299792458.0 * 299792458.0 * 1e-3, e.ToJoules()); // wattseconds
        Assert.AreEqual(1997.2337305262615, e / avgHouseYear); // that's enough for 2000 average houses for a year
        // and the inverse relations recover mass and speed
        Assert.AreEqual(1.0, (e / c / c).ToGrams());
        Assert.AreEqual(299792458.0, (e / (Mass.FromGrams(1) * c)).ToMetersPerSecond());

        // nuclear fission (total energy of 1g of U-235, initial fissle products only)
        Mass neutron = Mass.FromDaltons(1.008664891), u235 = Mass.FromDaltons(235.043928);
        Mass kr92 = Mass.FromDaltons(91.9261731), ba141 = Mass.FromDaltons(140.914403);
        e = ((neutron + u235) - (kr92 + ba141 + neutron * 3)) * c * c * (Mass.FromGrams(1) / u235);
        Assert.AreEqual(1.580681755342055, e / avgHouseYear); // that's roughly 1.6 average houses for a year

        // nuclear fusion (total energy from 1g of hydrogen)
        Mass h2 = Mass.FromDaltons(2.01410177812), he4 = Mass.FromDaltons(4.00260325415);
        e = (h2 * 2 - he4) * c * c * (Mass.FromGrams(1) / h2);
        Assert.AreEqual(25.38590025650838, e / avgHouseYear); // that's roughly 25.4 average houses for a year
    }

    // E = h f : Planck's constant (an Action, J·s) × Frequency (1/s) = Energy.
    [TestMethod]
    public void PlanckEinsteinRelation() {
        Action h = Action.FromPlanckConstants(1);
        Frequency f = Frequency.FromHertz(5e14);
        Energy e = h * f;
        Assert.AreEqual(6.62607015e-34 * 5e14, e.ToJoules());
        // inverse relations recover the constant and the frequency
        Assert.AreEqual(5e14, (e / h).ToHertz());
        Assert.AreEqual(1.0, (e / f).ToPlanckConstants());
    }

    [TestMethod]
    public void Electromagnetism() {
        Voltage v = ElectricCurrent.FromAmperes(2) * ElectricResistance.FromOhms(3);
        Assert.AreEqual(6.0, v.ToVolts());
        Assert.AreEqual(2.0, (v / ElectricResistance.FromOhms(3)).ToAmperes());
        Assert.AreEqual(3.0, (v / ElectricCurrent.FromAmperes(2)).ToOhms());
        Power p = ElectricCurrent.FromAmperes(2) * Voltage.FromVolts(6);
        Assert.AreEqual(12.0, p.ToWatts());
        ElectricCharge q = ElectricCurrent.FromAmperes(2) * Duration.FromSeconds(5);
        Assert.AreEqual(10.0, q.ToCoulombs());
        Capacitance c = ElectricCharge.FromCoulombs(6) / Voltage.FromVolts(3);
        Assert.AreEqual(2.0, c.ToFarads());
        MagneticFlux flux = Voltage.FromVolts(4) * Duration.FromSeconds(2);
        Assert.AreEqual(8.0, flux.ToWebers());
        MagneticFluxDensity b = flux / Area.FromSquareMeters(4);
        Assert.AreEqual(2.0, b.ToTeslas());
        LinearMagneticFluxDensity lmb = flux / Length.FromMeters(32e6);
        Assert.AreEqual(250.0, lmb.ToNanowebersPerMeter(), 1e-13); // 1000 Hz reference fluxivity
    }

    [TestMethod]
    public void Photometry() {
        LuminousFlux lf = LuminousIntensity.FromCandelas(2) * SolidAngle.FromSteradians(3);
        Assert.AreEqual(6.0, lf.ToLumens());
        Illuminance ill = lf / Area.FromSquareMeters(2);
        Assert.AreEqual(3.0, ill.ToLux());
        LuminousIntensity i = lf / SolidAngle.FromSteradians(3);
        Assert.AreEqual(2.0, i.ToCandelas());
    }

    [TestMethod]
    public void MassDerivedAndChemistry() {
        Density d = Mass.FromKilograms(20) / Volume.FromCubicMeters(4);
        Assert.AreEqual(5.0, d.ToKilogramsPerCubicMeter());
        Assert.AreEqual(10.0, (d * Volume.FromCubicMeters(2)).ToKilograms());
        Momentum mom = Mass.FromKilograms(3) * Speed.FromMetersPerSecond(4);
        Assert.AreEqual(12.0, mom.ToKilogramMetersPerSecond());
        Concentration conc = Quantity.FromMoles(6) / Volume.FromCubicMeters(3);
        Assert.AreEqual(2.0, conc.ToMolesPerCubicMeter());
        ReactionRate r = conc / Duration.FromSeconds(4);
        Assert.AreEqual(0.5, r.ToMolesPerCubicMeterSecond());
        DoseRate dr = AbsorbedDose.FromGrays(9) / Duration.FromSeconds(3);
        Assert.AreEqual(3.0, dr.ToGraysPerSecond());
        MomentOfInertia moi = Mass.FromKilograms(2) * Area.FromSquareMeters(3);
        AngularMomentum am = moi * AngularVelocity.FromRadiansPerSecond(5);
        Assert.AreEqual(30.0, am.ToKilogramSquareMetersPerSecond());
    }
}
