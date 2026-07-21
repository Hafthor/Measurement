namespace com.hafthor.Measurement;

// Well-known physics relations expressed directly through the type system.
// Results are computed in base units with round-number inputs, so exact equality holds.
[TestClass]
public sealed class PhysicsRelationsTests {
    [TestMethod]
    public void NewtonsSecondLaw() {
        // F = m a
        Force f = Mass.FromKilograms(3) * Acceleration.FromMetersPerSecondSquared(4);
        Assert.AreEqual(12.0, f.ToNewtons());
    }

    [TestMethod]
    public void ImpulseMomentumTheorem() {
        // Impulse J = F Δt equals the change in momentum
        Momentum impulse = Force.FromNewtons(10) * Duration.FromSeconds(3);
        Assert.AreEqual(30.0, impulse.ToKilogramMetersPerSecond());
        Momentum p = Mass.FromKilograms(6) * Speed.FromMetersPerSecond(5);
        Assert.AreEqual(30.0, p.ToKilogramMetersPerSecond());
        // and F = Δp / Δt
        Assert.AreEqual(10.0, (impulse / Duration.FromSeconds(3)).ToNewtons());
    }

    [TestMethod]
    public void WorkEnergyPrinciple() {
        // W = F d
        Energy w = Force.FromNewtons(20) * Length.FromMeters(5);
        Assert.AreEqual(100.0, w.ToJoules());
    }

    [TestMethod]
    public void PowerRelations() {
        // P = F v
        Assert.AreEqual(60.0, (Force.FromNewtons(20) * Speed.FromMetersPerSecond(3)).ToWatts());
        // P = V I
        Assert.AreEqual(24.0, (Voltage.FromVolts(12) * ElectricCurrent.FromAmperes(2)).ToWatts());
        // P = I^2 R
        var i = ElectricCurrent.FromAmperes(3);
        var r = ElectricResistance.FromOhms(4);
        Assert.AreEqual(36.0, (i * (i * r)).ToWatts());
        // P = V^2 / R
        var v = Voltage.FromVolts(12);
        Assert.AreEqual(36.0, (v * (v / r)).ToWatts());
    }

    [TestMethod]
    public void OhmsLaw() {
        // V = I R, I = V / R, R = V / I
        Voltage v = ElectricCurrent.FromAmperes(2) * ElectricResistance.FromOhms(6);
        Assert.AreEqual(12.0, v.ToVolts());
        Assert.AreEqual(2.0, (v / ElectricResistance.FromOhms(6)).ToAmperes());
        Assert.AreEqual(6.0, (v / ElectricCurrent.FromAmperes(2)).ToOhms());
    }

    [TestMethod]
    public void WaveEquation() {
        // c = f λ
        Speed c = Frequency.FromHertz(2) * Length.FromMeters(3);
        Assert.AreEqual(6.0, c.ToMetersPerSecond());
        // for light, f = c / λ
        Frequency f = Speed.FromSpeedOfLight(1) / Length.FromMeters(1);
        Assert.AreEqual(299792458.0, f.ToHertz());
        // wavelength / period is the wave *speed* (v = λ / T), not a frequency
        Speed v = Length.FromMeters(3) / Duration.FromSeconds(2);
        Assert.AreEqual(1.5, v.ToMetersPerSecond());
        // frequency is the reciprocal of the period: f = 1 / T
        Frequency fromPeriod = Frequency.FromPeriod(Duration.FromSeconds(0.5));
        Assert.AreEqual(2.0, fromPeriod.ToHertz());
        Assert.AreEqual(0.5, Frequency.FromHertz(2).ToPeriod().ToSeconds());
        // and the same reciprocal from the Duration side: T = 1 / f
        Assert.AreEqual(0.5, Duration.FromFrequency(Frequency.FromHertz(2)).ToSeconds());
        Assert.AreEqual(2.0, Duration.FromSeconds(0.5).ToFrequency().ToHertz());
    }

    [TestMethod]
    public void DeBroglieWavelength() {
        // λ = h / p
        Action h = Action.FromPlanckConstants(1);
        Momentum p = Momentum.FromKilogramMetersPerSecond(2);
        Length lambda = h / p;
        Assert.AreEqual(6.62607015e-34 / 2, lambda.ToMeters());
        // and p = h / λ recovers the momentum
        Assert.AreEqual(2.0, (h / lambda).ToKilogramMetersPerSecond());
    }

    [TestMethod]
    public void PhotonEnergyAndMomentum() {
        // E = h f, then p = E / c
        Action h = Action.FromPlanckConstants(1);
        Frequency f = Frequency.FromHertz(6e14);
        Energy e = h * f;
        Assert.AreEqual(6.62607015e-34 * 6e14, e.ToJoules());
        Speed cc = Speed.FromSpeedOfLight(1);
        Momentum p = e / cc;
        Assert.AreEqual((6.62607015e-34 * 6e14) / 299792458.0, p.ToKilogramMetersPerSecond());
    }

    [TestMethod]
    public void WorkToMoveCharge() {
        // W = Q V  (also the capacitor energy building block)
        Energy w = ElectricCharge.FromCoulombs(2) * Voltage.FromVolts(5);
        Assert.AreEqual(10.0, w.ToJoules());
        Assert.AreEqual(2.0, (w / Voltage.FromVolts(5)).ToCoulombs());
        Assert.AreEqual(5.0, (w / ElectricCharge.FromCoulombs(2)).ToVolts());
    }

    [TestMethod]
    public void PressureVolumeWork() {
        // W = P V
        Energy w = Pressure.FromPascals(101325) * Volume.FromCubicMeters(2);
        Assert.AreEqual(202650.0, w.ToJoules());
        Assert.AreEqual(2.0, (w / Pressure.FromPascals(101325)).ToCubicMeters());
    }

    [TestMethod]
    public void DensityFromMassAndVolume() {
        // ρ = m / V, and m = ρ V
        Density rho = Mass.FromKilograms(1000) / Volume.FromCubicMeters(1);
        Assert.AreEqual(1000.0, rho.ToKilogramsPerCubicMeter());
        Assert.AreEqual(2000.0, (rho * Volume.FromCubicMeters(2)).ToKilograms());
    }
}
