namespace com.hafthor.Measurement;

// Spot-checks a broad sample of operators now generated from [Product<,>] declarations, across
// families the other suites don't exercise. Round-number inputs give exactly representable results.
[TestClass]
public sealed class GeneratedOperatorTests {
    [TestMethod]
    public void Electromagnetism() {
        // P = V·I
        Assert.AreEqual(6.0, (Voltage.FromVolts(2) * ElectricCurrent.FromAmperes(3)).ToWatts());
        // Q = I·t
        Assert.AreEqual(6.0, (ElectricCurrent.FromAmperes(2) * Duration.FromSeconds(3)).ToCoulombs());
        // V = I·R (Ohm's law) and its inverses
        Voltage v = ElectricResistance.FromOhms(2) * ElectricCurrent.FromAmperes(3);
        Assert.AreEqual(6.0, v.ToVolts());
        Assert.AreEqual(3.0, (v / ElectricResistance.FromOhms(2)).ToAmperes());
        Assert.AreEqual(2.0, (v / ElectricCurrent.FromAmperes(3)).ToOhms());
        // Φ = V·t (magnetic flux)
        Assert.AreEqual(6.0, (Voltage.FromVolts(2) * Duration.FromSeconds(3)).ToWebers());
        // Φ = L·I (inductance)
        Assert.AreEqual(6.0, (Inductance.FromHenries(2) * ElectricCurrent.FromAmperes(3)).ToWebers());
        // Q = C·V (capacitance)
        Assert.AreEqual(6.0, (Capacitance.FromFarads(2) * Voltage.FromVolts(3)).ToCoulombs());
    }

    [TestMethod]
    public void Photometry_And_Geometry() {
        // luminous flux Φv = Iv·Ω
        Assert.AreEqual(6.0, (LuminousIntensity.FromCandelas(2) * SolidAngle.FromSteradians(3)).ToLumens());
        // volume = area · length and inverses
        Volume vol = Area.FromSquareMeters(6) * Length.FromMeters(2);
        Assert.AreEqual(12.0, vol.ToCubicMeters());
        Assert.AreEqual(6.0, (vol / Length.FromMeters(2)).ToSquareMeters());
    }

    [TestMethod]
    public void GramAnchored_Factors_AreExact() {
        // Density = Mass / Volume  (Mass is gram-anchored → coherent kg conversion must be exact)
        Assert.AreEqual(2.0, (Mass.FromKilograms(6) / Volume.FromCubicMeters(3)).ToKilogramsPerCubicMeter());
        // Momentum = Mass · Speed
        Assert.AreEqual(6.0, (Mass.FromKilograms(2) * Speed.FromMetersPerSecond(3)).ToKilogramMetersPerSecond());
        // MassFlowRate = Mass / Duration
        Assert.AreEqual(2.0, (Mass.FromKilograms(6) / Duration.FromSeconds(3)).ToKilogramsPerHour() / 3600.0, 1e-9);
    }

    [TestMethod]
    public void TemperatureResidualNine_IsExact() {
        // T = P·Rθ  (Temperature DisplayFactor 9 → exact /9 · 9 cancellation)
        Assert.AreEqual(6.0, (Power.FromWatts(2) * ThermalResistance.FromKelvinsPerWatt(3)).ToKelvin());
    }
}
