namespace com.hafthor.Measurement;

[TestClass]
public sealed class GeometryKinematicsTests {
    [TestMethod]
    public void Area_BaseConversions() {
        Assert.AreEqual(1e4, Area.FromHectares(1).ToSquareMeters());
        Assert.AreEqual(4046.8564224, Area.FromAcres(1).ToSquareMeters());
        Assert.AreEqual(1e6, Area.FromSquareKilometers(1).ToSquareMeters());
        Assert.AreEqual(1e-28, Area.FromBarns(1).ToSquareMeters());
    }

    [TestMethod]
    public void Volume_BaseConversions() {
        Assert.AreEqual(1e-3, Volume.FromLiters(1).ToCubicMeters());
        Assert.AreEqual(0.003785411784, Volume.FromGallons(1).ToCubicMeters());
        Assert.AreEqual(1.0, Volume.FromKiloliters(1).ToCubicMeters());
        Assert.AreEqual(0.158987294928, Volume.FromOilBarrels(1).ToCubicMeters());
    }

    [TestMethod]
    public void Speed_BaseConversions() {
        Assert.AreEqual(1.0 / 3.6, Speed.FromKilometersPerHour(1).ToMetersPerSecond());
        Assert.AreEqual(0.44704, Speed.FromMilesPerHour(1).ToMetersPerSecond());
        Assert.AreEqual(0.514444444444, Speed.FromKnots(1).ToMetersPerSecond());
        Assert.AreEqual(299792458.0, Speed.FromSpeedOfLight(1).ToMetersPerSecond());
    }

    [TestMethod]
    public void Acceleration_BaseConversions() {
        Assert.AreEqual(9.80665, Acceleration.FromStandardGravity(1).ToMetersPerSecondSquared());
        Assert.AreEqual(1e-2, Acceleration.FromGals(1).ToMetersPerSecondSquared());
        Assert.AreEqual(0.3048, Acceleration.FromFeetPerSecondSquared(1).ToMetersPerSecondSquared());
    }

    [TestMethod]
    public void Angle_BaseConversions() {
        Assert.AreEqual(Math.PI / 180, Angle.FromDegrees(1).ToRadians());
        Assert.AreEqual(2 * Math.PI, Angle.FromTurns(1).ToRadians());
        Assert.AreEqual(Math.PI / 200, Angle.FromGradians(1).ToRadians());
        Assert.AreEqual(Math.PI / 648000, Angle.FromArcseconds(1).ToRadians());
    }

    [TestMethod]
    public void SolidAngle_BaseConversions() {
        Assert.AreEqual(4 * Math.PI, SolidAngle.FromSpats(1).ToSteradians());
        Assert.AreEqual(Math.PI * Math.PI / 32400, SolidAngle.FromSquareDegrees(1).ToSteradians());
    }

    [TestMethod]
    public void Frequency_BaseConversions() {
        Assert.AreEqual(1000.0, Frequency.FromKilohertz(1).ToHertz());
        Assert.AreEqual(1e9, Frequency.FromGigahertz(1).ToHertz());
        Assert.AreEqual(1.0 / 60, Frequency.FromRevolutionsPerMinute(1).ToHertz());
    }

    [TestMethod]
    public void AngularKinematics_BaseConversions() {
        Assert.AreEqual(2 * Math.PI, AngularVelocity.FromRevolutionsPerSecond(1).ToRadiansPerSecond());
        Assert.AreEqual(Math.PI / 180, AngularVelocity.FromDegreesPerSecond(1).ToRadiansPerSecond());
        Assert.AreEqual(Math.PI / 180, AngularAcceleration.FromDegreesPerSecondSquared(1).ToRadiansPerSecondSquared());
    }

    [TestMethod]
    public void FlowAndWavenumber() {
        Assert.AreEqual(1e-3, VolumetricFlowRate.FromLitersPerSecond(1).ToCubicMetersPerSecond());
        Assert.AreEqual(0.5, Wavenumber.FromWavelength(Length.FromMeters(2)).ToPerMeter());
        Assert.AreEqual(2.0, Wavenumber.FromPerMeter(0.5).ToWavelength().ToMeters());
    }
}
