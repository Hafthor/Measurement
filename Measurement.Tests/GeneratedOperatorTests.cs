namespace com.hafthor.Measurement;

// Spot-checks a broad sample of operators now generated from [Product<,>] declarations, across
// families the other suites don't exercise. Round-number inputs give exactly representable results.
[TestClass]
public sealed class GeneratedOperatorTests {
    [TestMethod]
    public void Electromagnetism() {
        // P = V·I
        Assert.AreEqual(6.0, (Voltage.Of(2).Volts * ElectricCurrent.Of(3).Amperes).To.Watts);
        // Q = I·t
        Assert.AreEqual(6.0, (ElectricCurrent.Of(2).Amperes * Duration.Of(3).Seconds).To.Coulombs);
        // V = I·R (Ohm's law) and its inverses
        Voltage v = ElectricResistance.Of(2).Ohms * ElectricCurrent.Of(3).Amperes;
        Assert.AreEqual(6.0, v.To.Volts);
        Assert.AreEqual(3.0, (v / ElectricResistance.Of(2).Ohms).To.Amperes);
        Assert.AreEqual(2.0, (v / ElectricCurrent.Of(3).Amperes).To.Ohms);
        // Φ = V·t (magnetic flux)
        Assert.AreEqual(6.0, (Voltage.Of(2).Volts * Duration.Of(3).Seconds).To.Webers);
        // Φ = L·I (inductance)
        Assert.AreEqual(6.0, (Inductance.Of(2).Henries * ElectricCurrent.Of(3).Amperes).To.Webers);
        // Q = C·V (capacitance)
        Assert.AreEqual(6.0, (Capacitance.Of(2).Farads * Voltage.Of(3).Volts).To.Coulombs);
    }

    [TestMethod]
    public void Photometry_And_Geometry() {
        // luminous flux Φv = Iv·Ω
        Assert.AreEqual(6.0, (LuminousIntensity.Of(2).Candelas * SolidAngle.Of(3).Steradians).To.Lumens);
        // volume = area · length and inverses
        Volume vol = Area.Of(6).Square.Meters * Length.Of(2).Meters;
        Assert.AreEqual(12.0, vol.To.Cubic.Meters);
        Assert.AreEqual(6.0, (vol / Length.Of(2).Meters).To.Square.Meters);
    }

    [TestMethod]
    public void GramAnchored_Factors_AreExact() {
        // Density = Mass / Volume  (Mass is gram-anchored → coherent kg conversion must be exact)
        Assert.AreEqual(2.0, (Mass.Of(6).Kilo.Grams / Volume.Of(3).Cubic.Meters).To.Kilo.Grams.Per.Cubic.Meter);
        // Momentum = Mass · Speed
        Assert.AreEqual(6.0, (Mass.Of(2).Kilo.Grams * Speed.Of(3).Meters.Per.Second).To.Kilo.GramMetersPerSecond);
        // MassFlowRate = Mass / Duration
        Assert.AreEqual(2.0, (Mass.Of(6).Kilo.Grams / Duration.Of(3).Seconds).To.Kilo.Grams.Per.Hour / 3600.0, 1e-9);
    }

    [TestMethod]
    public void TemperatureResidualNine_IsExact() {
        // T = P·Rθ  (Temperature DisplayFactor 9 → exact /9 · 9 cancellation)
        Assert.AreEqual(6.0, (Power.Of(2).Watts * ThermalResistance.Of(3).Kelvins.Per.Watt).To.Kelvin);
    }
}
