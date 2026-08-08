namespace com.hafthor.Measurement;

// Well-known physics relations expressed directly through the type system.
// Results are computed in base units with round-number inputs, so exact equality holds.
[TestClass]
public sealed class PhysicsRelationsTests {
    [TestMethod]
    public void NewtonsSecondLaw() {
        // F = m a
        Force f = Mass.Of(3).Kilo.Grams * Acceleration.Of(4).Meters.Per.Second.Squared;
        Assert.AreEqual(12.0, f.To.Newtons);
    }

    [TestMethod]
    public void ImpulseMomentumTheorem() {
        // Impulse J = F Δt equals the change in momentum
        Momentum impulse = Force.Of(10).Newtons * Duration.Of(3).Seconds;
        Assert.AreEqual(30.0, impulse.To.Kilo.GramMetersPerSecond);
        Momentum p = Mass.Of(6).Kilo.Grams * Speed.Of(5).Meters.Per.Second;
        Assert.AreEqual(30.0, p.To.Kilo.GramMetersPerSecond);
        // and F = Δp / Δt
        Assert.AreEqual(10.0, (impulse / Duration.Of(3).Seconds).To.Newtons);
    }

    [TestMethod]
    public void WorkEnergyPrinciple() {
        // W = F d
        Energy w = Force.Of(20).Newtons * Length.Of(5).Meters;   // implicit → Energy
        Assert.AreEqual(100.0, w.To.Joules);
    }

    [TestMethod]
    public void PowerRelations() {
        // P = F v
        Assert.AreEqual(60.0, (Force.Of(20).Newtons * Speed.Of(3).Meters.Per.Second).To.Watts);
        // P = V I
        Assert.AreEqual(24.0, (Voltage.Of(12).Volts * ElectricCurrent.Of(2).Amperes).To.Watts);
        // P = I^2 R
        var i = ElectricCurrent.Of(3).Amperes;
        var r = ElectricResistance.Of(4).Ohms;
        Assert.AreEqual(36.0, (i * (i * r)).To.Watts);
        // P = V^2 / R
        var v = Voltage.Of(12).Volts;
        Assert.AreEqual(36.0, (v * (v / r)).To.Watts);
    }

    [TestMethod]
    public void OhmsLaw() {
        // V = I R, I = V / R, R = V / I
        Voltage v = ElectricCurrent.Of(2).Amperes * ElectricResistance.Of(6).Ohms;
        Assert.AreEqual(12.0, v.To.Volts);
        Assert.AreEqual(2.0, (v / ElectricResistance.Of(6).Ohms).To.Amperes);
        Assert.AreEqual(6.0, (v / ElectricCurrent.Of(2).Amperes).To.Ohms);
    }

    [TestMethod]
    public void WaveEquation() {
        // c = f λ
        Speed c = Frequency.Of(2).Hertz * Length.Of(3).Meters;
        Assert.AreEqual(6.0, c.To.Meters.Per.Second);
        // for light, f = c / λ
        Frequency f = Speed.Of(1).Speed.Of.Light / Length.Of(1).Meters;
        Assert.AreEqual(299792458.0, f.To.Hertz);
        // wavelength / period is the wave *speed* (v = λ / T), not a frequency
        Speed v = Length.Of(3).Meters / Duration.Of(2).Seconds;
        Assert.AreEqual(1.5, v.To.Meters.Per.Second);
        // frequency is the reciprocal of the period: f = 1 / T
        Frequency fromPeriod = Duration.Of(0.5).Seconds.To.Frequency;
        Assert.AreEqual(2.0, fromPeriod.To.Hertz);
        Assert.AreEqual(0.5, Frequency.Of(2).Hertz.To.Period.To.Seconds);
        // and the same reciprocal from the Duration side: T = 1 / f
        Assert.AreEqual(0.5, Frequency.Of(2).Hertz.To.Period.To.Seconds);
        Assert.AreEqual(2.0, Duration.Of(0.5).Seconds.To.Frequency.To.Hertz);
    }

    [TestMethod]
    public void DeBroglieWavelength() {
        // λ = h / p
        Action h = Action.Of(1).Planck.Constants;
        Momentum p = Momentum.Of(2).Kilo.Gram.Meters.Per.Second;
        Length lambda = h / p;
        Assert.AreEqual(6.62607015e-34 / 2, lambda.To.Meters);
        // and p = h / λ recovers the momentum
        Assert.AreEqual(2.0, (h / lambda).To.Kilo.GramMetersPerSecond);
    }

    [TestMethod]
    public void PhotonEnergyAndMomentum() {
        // E = h f, then p = E / c
        Action h = Action.Of(1).Planck.Constants;
        Frequency f = Frequency.Of(6e14).Hertz;
        Energy e = h * f;
        Assert.AreEqual(6.62607015e-34 * 6e14, e.To.Joules);
        Speed cc = Speed.Of(1).Speed.Of.Light;
        Momentum p = e / cc;
        Assert.AreEqual((6.62607015e-34 * 6e14) / 299792458.0, p.To.Kilo.GramMetersPerSecond);
    }

    [TestMethod]
    public void WorkToMoveCharge() {
        // W = Q V  (also the capacitor energy building block)
        Energy w = ElectricCharge.Of(2).Coulombs * Voltage.Of(5).Volts;
        Assert.AreEqual(10.0, w.To.Joules);
        Assert.AreEqual(2.0, (w / Voltage.Of(5).Volts).To.Coulombs);
        Assert.AreEqual(5.0, (w / ElectricCharge.Of(2).Coulombs).To.Volts);
    }

    [TestMethod]
    public void PressureVolumeWork() {
        // W = P V
        Energy w = Pressure.Of(101325).Pascals * Volume.Of(2).Cubic.Meters;
        Assert.AreEqual(202650.0, w.To.Joules);
        Assert.AreEqual(2.0, (w / Pressure.Of(101325).Pascals).To.Cubic.Meters);
    }

    [TestMethod]
    public void DensityFromMassAndVolume() {
        // ρ = m / V, and m = ρ V
        Density rho = Mass.Of(1000).Kilo.Grams / Volume.Of(1).Cubic.Meters;
        Assert.AreEqual(1000.0, rho.To.Kilo.Grams.Per.Cubic.Meter);
        Assert.AreEqual(2000.0, (rho * Volume.Of(2).Cubic.Meters).To.Kilo.Grams);
    }
}
