namespace com.hafthor.Measurement;

[TestClass]
public sealed class GeometryKinematicsTests {
    [TestMethod]
    public void Area_BaseConversions() {
        Assert.AreEqual(1e4, Area.Of(1).Hectares.To.Square.Meters);
        Assert.AreEqual(4046.8564224, Area.Of(1).Acres.To.Square.Meters);
        Assert.AreEqual(1e6, Area.Of(1).Square.Kilo.Meters.To.Square.Meters);
        Assert.AreEqual(1e-28, Area.Of(1).Barns.To.Square.Meters, 1e-43);
    }

    [TestMethod]
    public void Volume_BaseConversions() {
        Assert.AreEqual(1e-3, Volume.Of(1).Liters.To.Cubic.Meters);
        Assert.AreEqual(0.003785411784, Volume.Of(1).Gallons.To.Cubic.Meters);
        Assert.AreEqual(1.0, Volume.Of(1).Kilo.Liters.To.Cubic.Meters);
        Assert.AreEqual(0.158987294928, Volume.Of(1).Oil.Barrels.To.Cubic.Meters);
    }

    [TestMethod]
    public void Speed_BaseConversions() {
        Assert.AreEqual(1.0 / 3.6, Speed.Of(1).Kilo.Meters.Per.Hour.To.Meters.Per.Second);
        Assert.AreEqual(0.44704, Speed.Of(1).Miles.Per.Hour.To.Meters.Per.Second);
        Assert.AreEqual(0.514444444444, Speed.Of(1).Knots.To.Meters.Per.Second);
        Assert.AreEqual(299792458.0, Speed.Of(1).Speed.Of.Light.To.Meters.Per.Second);
    }

    [TestMethod]
    public void Acceleration_BaseConversions() {
        Assert.AreEqual(9.80665, Acceleration.Of(1).Standard.Gravity.To.Meters.Per.Second.Squared);
        Assert.AreEqual(1e-2, Acceleration.Of(1).Gals.To.Meters.Per.Second.Squared);
        Assert.AreEqual(0.3048, Acceleration.Of(1).Feet.Per.Second.Squared.To.Meters.Per.Second.Squared);
    }

    [TestMethod]
    public void Angle_BaseConversions() {
        Assert.AreEqual(Math.PI / 180, Angle.Of(1).Degrees.To.Radians);
        Assert.AreEqual(2 * Math.PI, Angle.Of(1).Turns.To.Radians);
        Assert.AreEqual(Math.PI / 200, Angle.Of(1).Gradians.To.Radians, 1e-17);
        Assert.AreEqual(Math.PI / 648000, Angle.Of(1).Arcseconds.To.Radians);
    }

    [TestMethod]
    public void SolidAngle_BaseConversions() {
        Assert.AreEqual(4 * Math.PI, SolidAngle.Of(1).Spats.To.Steradians);
        Assert.AreEqual(Math.PI * Math.PI / 32400, SolidAngle.Of(1).Square.Degrees.To.Steradians);
    }

    [TestMethod]
    public void Frequency_BaseConversions() {
        Assert.AreEqual(1000.0, Frequency.Of(1).Kilo.Hertz.To.Hertz);
        Assert.AreEqual(1e9, Frequency.Of(1).Giga.Hertz.To.Hertz);
    }

    [TestMethod]
    public void AngularKinematics_BaseConversions() {
        Assert.AreEqual(2 * Math.PI, AngularVelocity.Of(1).Revolutions.Per.Second.To.Radians.Per.Second);
        Assert.AreEqual(Math.PI / 180, AngularVelocity.Of(1).Degrees.Per.Second.To.Radians.Per.Second);
        Assert.AreEqual(Math.PI / 180, AngularAcceleration.Of(1).Degrees.Per.Second.Squared.To.Radians.Per.Second.Squared);
    }

    [TestMethod]
    public void FlowAndWavenumber() {
        Assert.AreEqual(1e-3, VolumetricFlowRate.Of(1).Liters.Per.Second.To.CubicMetersPerSecond);
        Assert.AreEqual(0.5, Length.Of(2).Meters.To.Wavenumber.To.PerMeter);
        Assert.AreEqual(2.0, Wavenumber.Of(0.5).Per.Meter.To.Wavelength.To.Meters);
    }
}
