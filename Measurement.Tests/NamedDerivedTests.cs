namespace com.hafthor.Measurement;

[TestClass]
public sealed class NamedDerivedTests {
    [TestMethod]
    public void Force_BaseConversions() {
        Assert.AreEqual(9.80665, Force.Of(1).Kilo.Grams.Force.To.Newtons);
        Assert.AreEqual(4.4482216152605, Force.Of(1).Pounds.Force.To.Newtons);
        Assert.AreEqual(1e-5, Force.Of(1).Dynes.To.Newtons);
        Assert.AreEqual(0.138254954376, Force.Of(1).Poundals.To.Newtons);
    }

    [TestMethod]
    public void Pressure_BaseConversions() {
        Assert.AreEqual(101325.0, Pressure.Of(1).Atmospheres.To.Pascals);
        Assert.AreEqual(1e5, Pressure.Of(1).Bars.To.Pascals);
        Assert.AreEqual(6894.757293168, Pressure.Of(1).Pounds.Per.Square.Inch.To.Pascals);
        Assert.AreEqual(133.32236842105263, Pressure.Of(1).Torr.To.Pascals);
    }

    [TestMethod]
    public void Energy_BaseConversions() {
        Assert.AreEqual(4.184, Energy.Of(1).Calories.To.Joules);
        Assert.AreEqual(3.6e6, Energy.Of(1).Kilo.Watt.Hours.To.Joules);
        Assert.AreEqual(3600.0, Energy.Of(1).Watt.Hours.To.Joules);
        Assert.AreEqual(1.602176634e-19, Energy.Of(1).Electronvolts.To.Joules);
        Assert.AreEqual(1055.05585262, Energy.Of(1).British.Thermal.Units.To.Joules);
    }

    [TestMethod]
    public void Power_BaseConversions() {
        Assert.AreEqual(745.6998715822702, Power.Of(1).Horsepower.To.Watts);
        Assert.AreEqual(735.49875, Power.Of(1).Metric.Horsepower.To.Watts);
        Assert.AreEqual(1e6, Power.Of(1).Mega.Watts.To.Watts);
    }

    [TestMethod]
    public void Charge_BaseConversions() {
        Assert.AreEqual(3600.0, ElectricCharge.Of(1).Ampere.Hours.To.Coulombs);
        Assert.AreEqual(1.602176634e-10, ElectricCharge.Of(1).Elementary.Charges.To.Nano.Coulombs);
        Assert.AreEqual(96485.33212, ElectricCharge.Of(1).Faradays.To.Coulombs);
    }

    [TestMethod]
    public void Electromagnetism_BaseConversions() {
        Assert.AreEqual(299.792458, Voltage.Of(1).Statvolts.To.Volts);
        Assert.AreEqual(1e-6, Capacitance.Of(1).Micro.Farads.To.Farads);
        Assert.AreEqual(1e6, ElectricResistance.Of(1).Mega.Ohms.To.Ohms);
        Assert.AreEqual(1.0, ElectricConductance.Of(1).Mhos.To.Siemens);
        Assert.AreEqual(1e-8, MagneticFlux.Of(1).Maxwells.To.Webers);
        Assert.AreEqual(1e-4, MagneticFluxDensity.Of(1).Gauss.To.Teslas);
        Assert.AreEqual(1e-3, Inductance.Of(1).Milli.Henries.To.Henries);
    }

    [TestMethod]
    public void Photometry_BaseConversions() {
        Assert.AreEqual(1000.0, LuminousFlux.Of(1).Kilo.Lumens.To.Lumens);
        Assert.AreEqual(10.763910416709722, Illuminance.Of(1).Footcandles.To.Lux);
        Assert.AreEqual(1e4, Illuminance.Of(1).Phots.To.Lux);
    }

    [TestMethod]
    public void RadiationAndCatalysis_BaseConversions() {
        Assert.AreEqual(3.7e10, Radioactivity.Of(1).Curies.To.Becquerels);
        Assert.AreEqual(1e-2, AbsorbedDose.Of(1).Rads.To.Grays);
        Assert.AreEqual(1e-2, EquivalentDose.Of(1).Rems.To.Sieverts);
        Assert.AreEqual(1.6666666666666667e-8, CatalyticActivity.Of(1).Enzyme.Units.To.Katals);
    }

    [TestMethod]
    public void Torque_BaseConversions() {
        Assert.AreEqual(1.3558179483314004, Torque.Of(1).Pound.Feet.To.NewtonMeters);
        Assert.AreEqual(9.80665, Torque.Of(1).Kilo.Gram.Force.Meters.To.NewtonMeters);
    }
}
