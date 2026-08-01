using com.hafthor.Measurement.Fluent; // for the double sugar entry (5.0.Joules etc.)

namespace com.hafthor.Measurement;

// Prototype `.Per` denominator-walk builder, hung off the concrete numerator type (Energy, Mass).
// The numerator terminal is unchanged (still returns the concrete type); `.Per` opts into the walk,
// and a completed compound implicitly converts to its measurement type.
[TestClass]
public sealed class PerFluentTests {
    [TestMethod]
    public void NumeratorTerminalUnchanged() {
        // The entry hook still returns the concrete type — no breakage of existing usage.
        Energy e = Measure.Of(5).Kilo.Joules;
        Assert.AreEqual(5000.0, e.ToJoules());
        Mass m = Measure.Of(2).Kilo.Grams;
        Assert.AreEqual(2.0, m.ToKilograms());
    }

    [TestMethod]
    public void Energy_Per_Kelvin_IsHeatCapacity() {
        HeatCapacity hc = Measure.Of(10).Joules.Per.Kelvin;
        Assert.AreEqual(HeatCapacity.FromJoulesPerKelvin(10).ToJoulesPerKelvin(), hc.ToJoulesPerKelvin());
    }

    [TestMethod]
    public void Energy_Per_Gram_Kelvin_IsSpecificHeatCapacity() {
        // The intermediate .Per.Gram is NOT a valid stop; .Kelvin completes it.
        SpecificHeatCapacity shc = Measure.Of(4184).Joules.Per.Kilo.Gram.Kelvin; // water
        Assert.AreEqual(4184.0, shc.ToJoulesPerKilogramKelvin(), 1e-9);
        // gram-denominator variant equals the flat factory
        SpecificHeatCapacity g = Measure.Of(1).Joules.Per.Gram.Kelvin;
        Assert.AreEqual(SpecificHeatCapacity.FromJoulesPerGramKelvin(1).ToJoulesPerKilogramKelvin(),
                        g.ToJoulesPerKilogramKelvin(), 1e-9);
    }

    [TestMethod]
    public void Energy_Per_Mole_Kelvin_IsMolarHeatCapacity() {
        MolarHeatCapacity mhc = Measure.Of(8.314).Joules.Per.Mole.Kelvin;
        Assert.AreEqual(8.314, mhc.ToJoulesPerMoleKelvin(), 1e-9);
    }

    [TestMethod]
    public void Mass_Per_Volume_IsDensity_AndPrefixComposes() {
        // 1 kg per cubic metre = 1 kg/m³; prefix on the numerator composes correctly.
        Density d = Measure.Of(1).Kilo.Grams.Per.Cubic.Meter;
        Assert.AreEqual(1.0, d.ToKilogramsPerCubicMeter(), 1e-12);
        // reached from grams too: 1000 g/m³ == 1 kg/m³
        Density d2 = Measure.Of(1000).Grams.Per.Cubic.Meter;
        Assert.AreEqual(1.0, d2.ToKilogramsPerCubicMeter(), 1e-12);
    }

    [TestMethod]
    public void Mass_Per_Mole_IsMolarMass() {
        MolarMass mm = Measure.Of(18).Grams.Per.Mole; // water ≈ 18 g/mol
        Assert.AreEqual(MolarMass.FromGramsPerMole(18).ToGramsPerMole(), mm.ToGramsPerMole(), 1e-9);
    }

    [TestMethod]
    public void Mass_Per_Meter_IsLinearDensity() {
        LinearDensity ld = Measure.Of(5).Grams.Per.Meter;
        Assert.AreEqual(LinearDensity.FromGramsPerMeter(5).ToGramsPerMeter(), ld.ToGramsPerMeter(), 1e-12);
    }

    [TestMethod]
    public void Mass_Per_Second_IsMassFlowRate() {
        MassFlowRate r = Measure.Of(3).Kilo.Grams.Per.Second;
        Assert.AreEqual(3.0, r.ToKilogramsPerSecond(), 1e-12);
    }

    [TestMethod]
    public void DoubleSugarEntry_Works() {
        Density d = 1.0.Kilo.Grams.Per.Liter; // 1 kg/L
        Assert.AreEqual(1.0, d.ToKilogramsPerLiter(), 1e-12);
    }

    [TestMethod]
    public void DenominatorPrefixViaChain_MatchesFlatCompound() {
        // .Kilo.Gram reaches the kilogram denominator (was .Kilogram); exact via FromJoulesPerKilogramKelvin.
        SpecificHeatCapacity shc = Measure.Of(1).Joules.Per.Kilo.Gram.Kelvin;
        Assert.AreEqual(SpecificHeatCapacity.FromJoulesPerKilogramKelvin(1).ToJoulesPerKilogramKelvin(),
                        shc.ToJoulesPerKilogramKelvin(), 1e-12);
        // .Centi.Meter (was .Centimeter)
        LinearDensity ld = Measure.Of(2).Grams.Per.Centi.Meter;
        Assert.AreEqual(LinearDensity.FromGramsPerCentimeter(2).ToGramsPerMeter(), ld.ToGramsPerMeter(), 1e-12);
        // .Cubic.Centi.Meter (was .Cubic.Centimeter) — prefix inside a cubic denominator
        Density d = Measure.Of(1).Grams.Per.Cubic.Centi.Meter; // 1 g/cm³ = 1000 kg/m³
        Assert.AreEqual(1000.0, d.ToKilogramsPerCubicMeter(), 1e-9);
    }

    [TestMethod]
    public void Angle_Per_Second_Squared_TheOriginalExample() {
        // Measure.Of(1).Degrees.Per.Second.Squared — the intermediate stops are all valid:
        Angle a = Measure.Of(90).Degrees;                                   // Angle
        Assert.AreEqual(90.0, a.ToDegrees(), 1e-12);
        AngularVelocity w = Measure.Of(90).Degrees.Per.Second;              // AngularVelocity
        Assert.AreEqual(90.0, w.ToDegreesPerSecond(), 1e-9);
        AngularAcceleration alpha = Measure.Of(90).Degrees.Per.Second.Squared; // AngularAcceleration
        Assert.AreEqual(90.0, alpha.ToDegreesPerSecondSquared(), 1e-9);
    }

    [TestMethod]
    public void Power_Per_SquareMeter_IsDualStop() {
        // Power.Per.Square.Meter is BOTH a stop (HeatFluxDensity) and continues (→ Radiance).
        HeatFluxDensity hfd = Measure.Of(50).Watts.Per.Square.Meter;
        Assert.AreEqual(50.0, hfd.ToWattsPerSquareMeter(), 1e-9);
        Radiance rad = Measure.Of(50).Watts.Per.Square.Meter.Steradian;
        Assert.AreEqual(50.0, rad.ToWattsPerSquareMeterSteradian(), 1e-9);
        // Kelvin denominator → ThermalConductivity
        ThermalConductivity tc = Measure.Of(2).Watts.Per.Meter.Kelvin;
        Assert.AreEqual(2.0, tc.ToWattsPerMeterKelvin(), 1e-9);
    }

    [TestMethod]
    public void ElectricCharge_Per_Mass_IsExposure_WithCorrectInversion() {
        // Mass in the denominator: C/g is 1000× C/kg for the same physical exposure.
        Exposure perKg = Measure.Of(1).Coulombs.Per.Kilo.Gram;
        Assert.AreEqual(1.0, perKg.ToCoulombsPerKilogram(), 1e-12);
        Exposure perG = Measure.Of(1).Coulombs.Per.Gram;
        Assert.AreEqual(1000.0, perG.ToCoulombsPerKilogram(), 1e-9);
        // volume/area denominators
        ChargeDensity cd = Measure.Of(3).Coulombs.Per.Cubic.Meter;
        Assert.AreEqual(3.0, cd.ToCoulombsPerCubicMeter(), 1e-12);
        SurfaceChargeDensity scd = Measure.Of(4).Coulombs.Per.Square.Meter;
        Assert.AreEqual(4.0, scd.ToCoulombsPerSquareMeter(), 1e-12);
    }

    [TestMethod]
    public void Speed_Per_Second_IsAcceleration_CompoundNumerator() {
        // 10 m/s per second = 10 m/s²; the numerator is the compound KilometersPerHour internally.
        Acceleration acc = Measure.Of(10).MetersPerSecond.Per.Second;
        Assert.AreEqual(10.0, acc.ToMetersPerSecondSquared(), 1e-9);
    }

    [TestMethod]
    public void Length_Per_Chains_BothWaysToAcceleration() {
        // Chained division: .Meters.Per.Second.Per.Second walks Length →(÷s) Speed →(÷s) Acceleration.
        Speed s = Measure.Of(10).Meters.Per.Second;
        Assert.AreEqual(10.0, s.ToMetersPerSecond(), 1e-9);
        Acceleration chained = Measure.Of(10).Meters.Per.Second.Per.Second;
        Assert.AreEqual(10.0, chained.ToMetersPerSecondSquared(), 1e-9);
        // The trailing-Squared spelling reaches the same result.
        Acceleration squared = Measure.Of(10).Meters.Per.Second.Squared;
        Assert.AreEqual(chained.ToMetersPerSecondSquared(), squared.ToMetersPerSecondSquared(), 1e-9);
        // One level deeper: Length →(÷s) Speed →(÷s) Acceleration → (÷s) is only Jerk via the
        // trailing-Cubed spelling, since Acceleration isn't a .Per gateway yet.
        Jerk jerk = Measure.Of(10).Meters.Per.Second.Cubed;
        Assert.AreEqual(10.0, jerk.ToMetersPerSecondCubed(), 1e-9);
    }

    [TestMethod]
    public void Speed_Per_Second_ChainAndDirectAgree() {
        // Direct Speed numerator and the Length→Speed chain give the same Acceleration.
        Acceleration direct = Measure.Of(36).KilometersPerHour.Per.Second; // 36 km/h per s = 10 m/s²
        Assert.AreEqual(10.0, direct.ToMetersPerSecondSquared(), 1e-9);
    }

    [TestMethod]
    public void FullRollout_NewlyEnabledSpines() {
        // Area ÷ time = kinematic viscosity
        KinematicViscosity kv = Measure.Of(3).SquareMeters.Per.Second;
        Assert.AreEqual(3.0, kv.ToSquareMetersPerSecond(), 1e-12);
        // Electric current ÷ length = magnetic field strength (A/m)
        MagneticFieldStrength h = Measure.Of(2).Amperes.Per.Meter;
        Assert.AreEqual(2.0, h.ToAmperesPerMeter(), 1e-12);
        // Voltage ÷ length = electric field strength (V/m)
        ElectricFieldStrength e = Measure.Of(5).Volts.Per.Meter;
        Assert.AreEqual(5.0, e.ToVoltsPerMeter(), 1e-12);
        // Force ÷ length = surface tension (N/m)
        SurfaceTension st = Measure.Of(4).Newtons.Per.Meter;
        Assert.AreEqual(4.0, st.ToNewtonsPerMeter(), 1e-12);
    }

    [TestMethod]
    public void DeepChain_ThreeDivisions_IsJerk() {
        // Length →(÷s) Speed →(÷s) Acceleration →(÷s) Jerk, all via chained .Per.
        Jerk j = Measure.Of(10).Meters.Per.Second.Per.Second.Per.Second;
        Assert.AreEqual(10.0, j.ToMetersPerSecondCubed(), 1e-9);
    }

    [TestMethod]
    public void PolymorphicDenominator_AnyUnitOfTheDimension() {        // A denominator slot now accepts ANY unit of its dimension, not only the ones spelled in a
        // compound unit. `.Minute`/`.Hour`/`.Day` are dimensionally computed from the coherent basis.
        AngularVelocity dpm = Measure.Of(90).Degrees.Per.Minute;     // 90°/min = 1.5°/s
        Assert.AreEqual(1.5, dpm.ToDegreesPerSecond(), 1e-9);
        Speed mph = Measure.Of(10).Meters.Per.Hour;                  // 10 m/h
        Assert.AreEqual(10.0 / 3600.0, mph.ToMetersPerSecond(), 1e-12);
        Speed mpd = Measure.Of(86400).Meters.Per.Day;                // 86400 m/day = 1 m/s
        Assert.AreEqual(1.0, mpd.ToMetersPerSecond(), 1e-9);
        // Squared works with any duration too: 90°/min² in rad/s².
        AngularAcceleration a = Measure.Of(90).Degrees.Per.Minute.Squared;
        Assert.AreEqual((System.Math.PI / 2) / 3600.0, a.ToRadiansPerSecondSquared(), 1e-12);
    }

    [TestMethod]
    public void Revolutions_DecomposeToAngularRates() {
        // A revolution is an angle (one full turn), so .Revolutions.Per.<time> is an angular rate —
        // no dedicated RevolutionsPerMinute hook needed.
        AngularVelocity w = Measure.Of(1).Revolutions.Per.Minute;
        Assert.AreEqual(AngularVelocity.FromRevolutionsPerMinute(1).ToRadiansPerSecond(),
            w.ToRadiansPerSecond(), 1e-12);
        AngularAcceleration a = Measure.Of(1).Revolutions.Per.Minute.Per.Second;
        Assert.AreEqual(AngularAcceleration.FromRevolutionsPerMinutePerSecond(1).ToRadiansPerSecondSquared(),
            a.ToRadiansPerSecondSquared(), 1e-12);
        AngularVelocity rps = Measure.Of(2).Revolutions.Per.Second;
        Assert.AreEqual(AngularVelocity.FromRevolutionsPerSecond(2).ToRadiansPerSecond(),
            rps.ToRadiansPerSecond(), 1e-12);
        // read-out mirrors it
        double rpm = AngularVelocity.FromRevolutionsPerMinute(3).To.Revolutions.Per.Minute;
        Assert.AreEqual(3.0, rpm, 1e-12);
    }

    // ---- Read-out side: reader.To.<Numerator>.Per.<denominator…> mirrors the input walk. ----
    [TestMethod]
    public void ReadOut_Speed_Meters_Per_Second() {
        Speed s = Speed.FromMetersPerSecond(15);
        double v = s.To.Meters.Per.Second; // implicit conversion to double
        Assert.AreEqual(15.0, v, 1e-9);
    }

    [TestMethod]
    public void ReadOut_Acceleration_Meters_Per_Second_Squared() {
        Acceleration a = Acceleration.FromMetersPerSecondSquared(9.81);
        double v = a.To.Meters.Per.Second.Squared;
        Assert.AreEqual(9.81, v, 1e-9);
    }

    [TestMethod]
    public void ReadOut_SpecificHeatCapacity_PrefixDecomposedDenominator() {
        var water = SpecificHeatCapacity.FromJoulesPerKilogramKelvin(4184);
        double perKg = water.To.Joules.Per.Kilo.Gram.Kelvin;   // 4184 J/(kg·K)
        Assert.AreEqual(4184.0, perKg, 1e-9);
        double perG = water.To.Joules.Per.Gram.Kelvin;         // 4.184 J/(g·K)
        Assert.AreEqual(4.184, perG, 1e-9);
    }

    [TestMethod]
    public void ReadOut_MatchesFlatReadout() {
        // The walked read-out equals the existing flat ToXxx()/hook for a range of types.
        Density d = Density.FromKilogramsPerCubicMeter(1000);
        Assert.AreEqual(d.ToGramsPerCubicCentimeter(), d.To.Grams.Per.Cubic.Centi.Meter, 1e-12);
        Assert.AreEqual(d.ToKilogramsPerCubicMeter(), d.To.Kilograms.Per.Cubic.Meter, 1e-12);
        HeatFluxDensity h = HeatFluxDensity.FromWattsPerSquareMeter(500);
        Assert.AreEqual(h.ToWattsPerSquareCentimeter(), h.To.Watts.Per.Square.Centi.Meter, 1e-12);
        Radiance r = Radiance.FromWattsPerSquareMeterSteradian(7);
        Assert.AreEqual(7.0, r.To.Watts.Per.Square.Meter.Steradian, 1e-12);
    }

    [TestMethod]
    public void ReadOut_NumeratorPrefixViaFactorChain() {
        // The existing Reader prefix chain composes with the numerator start: .Kilo scales the factor.
        Speed s = Speed.FromMetersPerSecond(3000);
        Assert.AreEqual(3.0, s.To.Kilo.Meters.Per.Second, 1e-9); // 3000 m/s = 3 km/s
    }

    [TestMethod]
    public void ReadOut_PolymorphicDenominator() {
        // Read-out denominators are dimension-polymorphic too: any duration works, not just Second.
        Speed s = Speed.FromMetersPerSecond(10);
        Assert.AreEqual(600.0, s.To.Meters.Per.Minute, 1e-9);   // 10 m/s = 600 m/min
        Assert.AreEqual(36000.0, s.To.Meters.Per.Hour, 1e-9);   // 10 m/s = 36000 m/h
        AngularVelocity w = AngularVelocity.FromDegreesPerSecond(1.5);
        Assert.AreEqual(90.0, w.To.Degrees.Per.Minute, 1e-9);   // 1.5°/s = 90°/min
    }
}
