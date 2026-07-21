namespace com.hafthor.Measurement;

[TestClass]
public sealed class NamedDerivedTests {
    [TestMethod]
    public void Force_BaseConversions() {
        Assert.AreEqual(9.80665, Force.FromKilogramsForce(1).ToNewtons());
        Assert.AreEqual(4.4482216152605, Force.FromPoundsForce(1).ToNewtons());
        Assert.AreEqual(1e-5, Force.FromDynes(1).ToNewtons());
        Assert.AreEqual(0.138254954376, Force.FromPoundals(1).ToNewtons());
    }

    [TestMethod]
    public void Pressure_BaseConversions() {
        Assert.AreEqual(101325.0, Pressure.FromAtmospheres(1).ToPascals());
        Assert.AreEqual(1e5, Pressure.FromBars(1).ToPascals());
        Assert.AreEqual(6894.757293168, Pressure.FromPoundsPerSquareInch(1).ToPascals());
        Assert.AreEqual(133.32236842105263, Pressure.FromTorr(1).ToPascals());
    }

    [TestMethod]
    public void Energy_BaseConversions() {
        Assert.AreEqual(4.184, Energy.FromCalories(1).ToJoules());
        Assert.AreEqual(3.6e6, Energy.FromKilowattHours(1).ToJoules());
        Assert.AreEqual(3600.0, Energy.FromWattHours(1).ToJoules());
        Assert.AreEqual(1.602176634e-19, Energy.FromElectronvolts(1).ToJoules());
        Assert.AreEqual(1055.05585262, Energy.FromBritishThermalUnits(1).ToJoules());
    }

    [TestMethod]
    public void Power_BaseConversions() {
        Assert.AreEqual(745.6998715822702, Power.FromHorsepower(1).ToWatts());
        Assert.AreEqual(735.49875, Power.FromMetricHorsepower(1).ToWatts());
        Assert.AreEqual(1e6, Power.FromMegawatts(1).ToWatts());
    }

    [TestMethod]
    public void Charge_BaseConversions() {
        Assert.AreEqual(3600.0, ElectricCharge.FromAmpereHours(1).ToCoulombs());
        Assert.AreEqual(1.602176634e-19, ElectricCharge.FromElementaryCharges(1).ToCoulombs());
        Assert.AreEqual(96485.33212, ElectricCharge.FromFaradays(1).ToCoulombs());
    }

    [TestMethod]
    public void Electromagnetism_BaseConversions() {
        Assert.AreEqual(299.792458, Voltage.FromStatvolts(1).ToVolts());
        Assert.AreEqual(1e-6, Capacitance.FromMicrofarads(1).ToFarads());
        Assert.AreEqual(1e6, ElectricResistance.FromMegaohms(1).ToOhms());
        Assert.AreEqual(1.0, ElectricConductance.FromMhos(1).ToSiemens());
        Assert.AreEqual(1e-8, MagneticFlux.FromMaxwells(1).ToWebers());
        Assert.AreEqual(1e-4, MagneticFluxDensity.FromGauss(1).ToTeslas());
        Assert.AreEqual(1e-3, Inductance.FromMillihenries(1).ToHenries());
    }

    [TestMethod]
    public void Photometry_BaseConversions() {
        Assert.AreEqual(1000.0, LuminousFlux.FromKilolumens(1).ToLumens());
        Assert.AreEqual(10.763910416709722, Illuminance.FromFootcandles(1).ToLux());
        Assert.AreEqual(1e4, Illuminance.FromPhots(1).ToLux());
    }

    [TestMethod]
    public void RadiationAndCatalysis_BaseConversions() {
        Assert.AreEqual(3.7e10, Radioactivity.FromCuries(1).ToBecquerels());
        Assert.AreEqual(1e-2, AbsorbedDose.FromRads(1).ToGrays());
        Assert.AreEqual(1e-2, EquivalentDose.FromRems(1).ToSieverts());
        Assert.AreEqual(1.6666666666666667e-8, CatalyticActivity.FromEnzymeUnits(1).ToKatals());
    }

    [TestMethod]
    public void Torque_BaseConversions() {
        Assert.AreEqual(1.3558179483314004, Torque.FromPoundFeet(1).ToNewtonMeters());
        Assert.AreEqual(9.80665, Torque.FromKilogramForceMeters(1).ToNewtonMeters());
    }
}
