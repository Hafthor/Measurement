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
        Assert.AreEqual(5000.0, e.To.Joules);
        Mass m = Measure.Of(2).Kilo.Grams;
        Assert.AreEqual(2.0, m.To.Kilo.Grams);
    }

    [TestMethod]
    public void Energy_Per_Kelvin_IsHeatCapacity() {
        HeatCapacity hc = Measure.Of(10).Joules.Per.Kelvin;
        Assert.AreEqual(HeatCapacity.Of(10).Joules.Per.Kelvin.To.Joules.Per.Kelvin, hc.To.Joules.Per.Kelvin);
    }

    [TestMethod]
    public void Energy_Per_Gram_Kelvin_IsSpecificHeatCapacity() {
        // The intermediate .Per.Gram is NOT a valid stop; .Kelvin completes it.
        SpecificHeatCapacity shc = Measure.Of(4184).Joules.Per.Kilo.Gram.Kelvin; // water
        Assert.AreEqual(4184.0, shc.To.Joules.Per.Kilo.Gram.Kelvin, 1e-9);
        // gram-denominator variant equals the flat factory
        SpecificHeatCapacity g = Measure.Of(1).Joules.Per.Gram.Kelvin;
        Assert.AreEqual(SpecificHeatCapacity.Of(1).Joules.Per.Gram.Kelvin.To.Joules.Per.Kilo.Gram.Kelvin,
                        g.To.Joules.Per.Kilo.Gram.Kelvin, 1e-9);
    }

    [TestMethod]
    public void Energy_Per_Mole_Kelvin_IsMolarHeatCapacity() {
        MolarHeatCapacity mhc = Measure.Of(8.314).Joules.Per.Mole.Kelvin;
        Assert.AreEqual(8.314, mhc.To.Joules.Per.Mole.Kelvin, 1e-9);
    }

    [TestMethod]
    public void Mass_Per_Volume_IsDensity_AndPrefixComposes() {
        // 1 kg per cubic metre = 1 kg/m³; prefix on the numerator composes correctly.
        Density d = Measure.Of(1).Kilo.Grams.Per.Cubic.Meter;
        Assert.AreEqual(1.0, d.To.Kilo.Grams.Per.Cubic.Meter, 1e-12);
        // reached from grams too: 1000 g/m³ == 1 kg/m³
        Density d2 = Measure.Of(1000).Grams.Per.Cubic.Meter;
        Assert.AreEqual(1.0, d2.To.Kilo.Grams.Per.Cubic.Meter, 1e-12);
    }

    [TestMethod]
    public void Mass_Per_Mole_IsMolarMass() {
        MolarMass mm = Measure.Of(18).Grams.Per.Mole; // water ≈ 18 g/mol
        Assert.AreEqual(MolarMass.Of(18).Grams.Per.Mole.To.Grams.Per.Mole, mm.To.Grams.Per.Mole, 1e-9);
    }

    [TestMethod]
    public void Mass_Per_Meter_IsLinearDensity() {
        LinearDensity ld = Measure.Of(5).Grams.Per.Meter;
        Assert.AreEqual(LinearDensity.Of(5).Grams.Per.Meter.To.Grams.Per.Meter, ld.To.Grams.Per.Meter, 1e-12);
    }

    [TestMethod]
    public void Mass_Per_Second_IsMassFlowRate() {
        MassFlowRate r = Measure.Of(3).Kilo.Grams.Per.Second;
        Assert.AreEqual(3.0, r.To.Kilo.Grams.Per.Second, 1e-12);
    }

    [TestMethod]
    public void DenominatorPrefixes_AreUniversal() {
        // The .Per denominator now accepts the full SI prefix range (folded into the running factor),
        // not just each unit's declared set — Duration declares no Kilo/Mega, yet per-kilosecond works.
        // 2000 m/s = 2000 m per second = 2e6 m per kilosecond.
        Speed s = Measure.Of(2000).Meters.Per.Second;
        Assert.AreEqual(2e6, s.To.Meters.Per.Kilo.Second, 1e-3);
        Assert.AreEqual(2e9, s.To.Meters.Per.Mega.Second, 1);
        // construction with an undeclared denominator prefix round-trips
        Speed s2 = Measure.Of(5).Meters.Per.Kilo.Second;     // 5 m per kilosecond = 0.005 m/s
        Assert.AreEqual(0.005, s2.To.Meters.Per.Second, 1e-12);
        Assert.AreEqual(5.0, s2.To.Meters.Per.Kilo.Second, 1e-9);
        // a multi-factor denominator with an undeclared prefix (per teragram·kelvin)
        SpecificHeatCapacity shc = Measure.Of(1).Joules.Per.Tera.Gram.Kelvin;
        Assert.AreEqual(1.0, shc.To.Joules.Per.Tera.Gram.Kelvin, 1e-9);
    }

    [TestMethod]
    public void DoubleSugarEntry_Works() {
        Density d = 1.0.Kilo.Grams.Per.Liter; // 1 kg/L
        Assert.AreEqual(1.0, d.To.Kilo.Grams.Per.Liter, 1e-12);
    }

    [TestMethod]
    public void DenominatorPrefixViaChain_MatchesFlatCompound() {
        // .Kilo.Gram reaches the kilogram denominator (was .Kilogram); exact via FromJoulesPerKilogramKelvin.
        SpecificHeatCapacity shc = Measure.Of(1).Joules.Per.Kilo.Gram.Kelvin;
        Assert.AreEqual(SpecificHeatCapacity.Of(1).Joules.Per.Kilo.Gram.Kelvin.To.Joules.Per.Kilo.Gram.Kelvin,
                        shc.To.Joules.Per.Kilo.Gram.Kelvin, 1e-12);
        // .Centi.Meter (was .Centimeter)
        LinearDensity ld = Measure.Of(2).Grams.Per.Centi.Meter;
        Assert.AreEqual(LinearDensity.Of(2).Grams.Per.Centi.Meter.To.Grams.Per.Meter, ld.To.Grams.Per.Meter, 1e-12);
        // .Cubic.Centi.Meter (was .Cubic.Centimeter) — prefix inside a cubic denominator
        Density d = Measure.Of(1).Grams.Per.Cubic.Centi.Meter; // 1 g/cm³ = 1000 kg/m³
        Assert.AreEqual(1000.0, d.To.Kilo.Grams.Per.Cubic.Meter, 1e-9);
        // areal/cubic denominators are universal too: prefixes never declared for Square/Cubic units.
        // 1 g/dm³ (deci) = 1 g / 1e-3 m³ = 1 kg/m³. deca/deci were never declared for cubic units.
        Density dd = Measure.Of(1).Grams.Per.Cubic.Deci.Meter;
        Assert.AreEqual(1.0, dd.To.Kilo.Grams.Per.Cubic.Meter, 1e-12);
        // read side: HeatFluxDensity per square deka-meter (undeclared) round-trips
        HeatFluxDensity hfd = Measure.Of(100).Watts.Per.Square.Meter;   // 100 W/m² = 1e4 W per (10 m)²
        Assert.AreEqual(1e4, hfd.To.Watts.Per.Square.Deca.Meter, 1e-6);
    }

    [TestMethod]
    public void Angle_Per_Second_Squared_TheOriginalExample() {
        // Measure.Of(1).Degrees.Per.Second.Squared — the intermediate stops are all valid:
        Angle a = Measure.Of(90).Degrees;                                   // Angle
        Assert.AreEqual(90.0, a.To.Degrees, 1e-12);
        AngularVelocity w = Measure.Of(90).Degrees.Per.Second;              // AngularVelocity
        Assert.AreEqual(90.0, w.To.Degrees.Per.Second, 1e-9);
        AngularAcceleration alpha = Measure.Of(90).Degrees.Per.Second.Squared; // AngularAcceleration
        Assert.AreEqual(90.0, alpha.To.Degrees.Per.Second.Squared, 1e-9);
    }

    [TestMethod]
    public void Power_Per_SquareMeter_IsDualStop() {
        // Power.Per.Square.Meter is BOTH a stop (HeatFluxDensity) and continues (→ Radiance).
        HeatFluxDensity hfd = Measure.Of(50).Watts.Per.Square.Meter;
        Assert.AreEqual(50.0, hfd.To.Watts.Per.Square.Meter, 1e-9);
        Radiance rad = Measure.Of(50).Watts.Per.Square.Meter.Steradian;
        Assert.AreEqual(50.0, rad.To.Watts.Per.Square.Meter.Steradian, 1e-9);
        // Kelvin denominator → ThermalConductivity
        ThermalConductivity tc = Measure.Of(2).Watts.Per.Meter.Kelvin;
        Assert.AreEqual(2.0, tc.To.Watts.Per.Meter.Kelvin, 1e-9);
    }

    [TestMethod]
    public void ElectricCharge_Per_Mass_IsExposure_WithCorrectInversion() {
        // Mass in the denominator: C/g is 1000× C/kg for the same physical exposure.
        Exposure perKg = Measure.Of(1).Coulombs.Per.Kilo.Gram;
        Assert.AreEqual(1.0, perKg.To.Coulombs.Per.Kilo.Gram, 1e-12);
        Exposure perG = Measure.Of(1).Coulombs.Per.Gram;
        Assert.AreEqual(1000.0, perG.To.Coulombs.Per.Kilo.Gram, 1e-9);
        // volume/area denominators
        ChargeDensity cd = Measure.Of(3).Coulombs.Per.Cubic.Meter;
        Assert.AreEqual(3.0, cd.To.Coulombs.Per.Cubic.Meter, 1e-12);
        SurfaceChargeDensity scd = Measure.Of(4).Coulombs.Per.Square.Meter;
        Assert.AreEqual(4.0, scd.To.Coulombs.Per.Square.Meter, 1e-12);
    }

    [TestMethod]
    public void Speed_Per_Second_IsAcceleration_CompoundNumerator() {
        // 10 m/s per second = 10 m/s²; the numerator is the compound KilometersPerHour internally.
        Acceleration acc = Measure.Of(10).MetersPerSecond.Per.Second;
        Assert.AreEqual(10.0, acc.To.Meters.Per.Second.Squared, 1e-9);
    }

    [TestMethod]
    public void Length_Per_Chains_BothWaysToAcceleration() {
        // Chained division: .Meters.Per.Second.Per.Second walks Length →(÷s) Speed →(÷s) Acceleration.
        Speed s = Measure.Of(10).Meters.Per.Second;
        Assert.AreEqual(10.0, s.To.Meters.Per.Second, 1e-9);
        Acceleration chained = Measure.Of(10).Meters.Per.Second.Per.Second;
        Assert.AreEqual(10.0, chained.To.Meters.Per.Second.Squared, 1e-9);
        // The trailing-Squared spelling reaches the same result.
        Acceleration squared = Measure.Of(10).Meters.Per.Second.Squared;
        Assert.AreEqual(chained.To.Meters.Per.Second.Squared, squared.To.Meters.Per.Second.Squared, 1e-9);
        // One level deeper: Length →(÷s) Speed →(÷s) Acceleration → (÷s) is only Jerk via the
        // trailing-Cubed spelling, since Acceleration isn't a .Per gateway yet.
        Jerk jerk = Measure.Of(10).Meters.Per.Second.Cubed;
        Assert.AreEqual(10.0, jerk.To.Meters.Per.Second.Cubed, 1e-9);
    }

    [TestMethod]
    public void Speed_Per_Second_ChainAndDirectAgree() {
        // Direct Speed numerator and the Length→Speed chain give the same Acceleration.
        Acceleration direct = Measure.Of(36).KilometersPerHour.Per.Second; // 36 km/h per s = 10 m/s²
        Assert.AreEqual(10.0, direct.To.Meters.Per.Second.Squared, 1e-9);
    }

    [TestMethod]
    public void FullRollout_NewlyEnabledSpines() {
        // Area ÷ time = kinematic viscosity
        KinematicViscosity kv = Measure.Of(3).SquareMeters.Per.Second;
        Assert.AreEqual(3.0, kv.To.SquareMetersPerSecond, 1e-12);
        // Electric current ÷ length = magnetic field strength (A/m)
        MagneticFieldStrength h = Measure.Of(2).Amperes.Per.Meter;
        Assert.AreEqual(2.0, h.To.Amperes.Per.Meter, 1e-12);
        // Voltage ÷ length = electric field strength (V/m)
        ElectricFieldStrength e = Measure.Of(5).Volts.Per.Meter;
        Assert.AreEqual(5.0, e.To.Volts.Per.Meter, 1e-12);
        // Force ÷ length = surface tension (N/m)
        SurfaceTension st = Measure.Of(4).Newtons.Per.Meter;
        Assert.AreEqual(4.0, st.To.Newtons.Per.Meter, 1e-12);
    }

    [TestMethod]
    public void DeepChain_ThreeDivisions_IsJerk() {
        // Length →(÷s) Speed →(÷s) Acceleration →(÷s) Jerk, all via chained .Per.
        Jerk j = Measure.Of(10).Meters.Per.Second.Per.Second.Per.Second;
        Assert.AreEqual(10.0, j.To.Meters.Per.Second.Cubed, 1e-9);
    }

    [TestMethod]
    public void PolymorphicDenominator_AnyUnitOfTheDimension() {        // A denominator slot now accepts ANY unit of its dimension, not only the ones spelled in a
        // compound unit. `.Minute`/`.Hour`/`.Day` are dimensionally computed from the coherent basis.
        AngularVelocity dpm = Measure.Of(90).Degrees.Per.Minute;     // 90°/min = 1.5°/s
        Assert.AreEqual(1.5, dpm.To.Degrees.Per.Second, 1e-9);
        Speed mph = Measure.Of(10).Meters.Per.Hour;                  // 10 m/h
        Assert.AreEqual(10.0 / 3600.0, mph.To.Meters.Per.Second, 1e-12);
        Speed mpd = Measure.Of(86400).Meters.Per.Day;                // 86400 m/day = 1 m/s
        Assert.AreEqual(1.0, mpd.To.Meters.Per.Second, 1e-9);
        // Squared works with any duration too: 90°/min² in rad/s².
        AngularAcceleration a = Measure.Of(90).Degrees.Per.Minute.Squared;
        Assert.AreEqual((System.Math.PI / 2) / 3600.0, a.To.Radians.Per.Second.Squared, 1e-12);
    }

    [TestMethod]
    public void Revolutions_DecomposeToAngularRates() {
        // A revolution is an angle (one full turn), so .Revolutions.Per.<time> is an angular rate —
        // no dedicated RevolutionsPerMinute hook needed.
        AngularVelocity w = Measure.Of(1).Revolutions.Per.Minute;
        Assert.AreEqual(AngularVelocity.Of(1).Revolutions.Per.Minute.To.Radians.Per.Second,
            w.To.Radians.Per.Second, 1e-12);
        AngularAcceleration a = Measure.Of(1).Revolutions.Per.Minute.Per.Second;
        Assert.AreEqual(AngularAcceleration.Of(1).Revolutions.Per.Minute.Per.Second.To.Radians.Per.Second.Squared,
            a.To.Radians.Per.Second.Squared, 1e-12);
        AngularVelocity rps = Measure.Of(2).Revolutions.Per.Second;
        Assert.AreEqual(AngularVelocity.Of(2).Revolutions.Per.Second.To.Radians.Per.Second,
            rps.To.Radians.Per.Second, 1e-12);
        // read-out mirrors it
        double rpm = AngularVelocity.Of(3).Revolutions.Per.Minute.To.Revolutions.Per.Minute;
        Assert.AreEqual(3.0, rpm, 1e-12);
    }

    // ---- Read-out side: reader.To.<Numerator>.Per.<denominator…> mirrors the input walk. ----
    [TestMethod]
    public void ReadOut_Speed_Meters_Per_Second() {
        Speed s = Speed.Of(15).Meters.Per.Second;
        double v = s.To.Meters.Per.Second; // implicit conversion to double
        Assert.AreEqual(15.0, v, 1e-9);
    }

    [TestMethod]
    public void ReadOut_Acceleration_Meters_Per_Second_Squared() {
        Acceleration a = Acceleration.Of(9.81).Meters.Per.Second.Squared;
        double v = a.To.Meters.Per.Second.Squared;
        Assert.AreEqual(9.81, v, 1e-9);
    }

    [TestMethod]
    public void ReadOut_SpecificHeatCapacity_PrefixDecomposedDenominator() {
        var water = SpecificHeatCapacity.Of(4184).Joules.Per.Kilo.Gram.Kelvin;
        double perKg = water.To.Joules.Per.Kilo.Gram.Kelvin;   // 4184 J/(kg·K)
        Assert.AreEqual(4184.0, perKg, 1e-9);
        double perG = water.To.Joules.Per.Gram.Kelvin;         // 4.184 J/(g·K)
        Assert.AreEqual(4.184, perG, 1e-9);
    }

    [TestMethod]
    public void ReadOut_MatchesFlatReadout() {
        // The walked read-out equals the existing flat ToXxx()/hook for a range of types.
        Density d = Density.Of(1000).Kilo.Grams.Per.Cubic.Meter;
        Assert.AreEqual(d.To.Grams.Per.Cubic.Centi.Meter, d.To.Grams.Per.Cubic.Centi.Meter, 1e-12);
        Assert.AreEqual(d.To.Kilo.Grams.Per.Cubic.Meter, d.To.Kilo.Grams.Per.Cubic.Meter, 1e-12);
        HeatFluxDensity h = HeatFluxDensity.Of(500).Watts.Per.Square.Meter;
        Assert.AreEqual(h.To.Watts.Per.Square.Centi.Meter, h.To.Watts.Per.Square.Centi.Meter, 1e-12);
        Radiance r = Radiance.Of(7).Watts.Per.Square.Meter.Steradian;
        Assert.AreEqual(7.0, r.To.Watts.Per.Square.Meter.Steradian, 1e-12);
    }

    [TestMethod]
    public void ReadOut_NumeratorPrefixViaFactorChain() {
        // The existing Reader prefix chain composes with the numerator start: .Kilo scales the factor.
        Speed s = Speed.Of(3000).Meters.Per.Second;
        Assert.AreEqual(3.0, s.To.Kilo.Meters.Per.Second, 1e-9); // 3000 m/s = 3 km/s
    }

    [TestMethod]
    public void ReadOut_PolymorphicDenominator() {
        // Read-out denominators are dimension-polymorphic too: any duration works, not just Second.
        Speed s = Speed.Of(10).Meters.Per.Second;
        Assert.AreEqual(600.0, s.To.Meters.Per.Minute, 1e-9);   // 10 m/s = 600 m/min
        Assert.AreEqual(36000.0, s.To.Meters.Per.Hour, 1e-9);   // 10 m/s = 36000 m/h
        AngularVelocity w = AngularVelocity.Of(1.5).Degrees.Per.Second;
        Assert.AreEqual(90.0, w.To.Degrees.Per.Minute, 1e-9);   // 1.5°/s = 90°/min
    }
}
