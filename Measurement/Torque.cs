namespace com.hafthor.Measurement;

[Measurement("N·m", VariableName = "newtonMillimeters", DisplayFactor = 1e3)]
[SiUnit("NewtonMeters", 3)]
[SiUnit("NewtonMillimeters", 0)]
[Unit("KilogramForceMeters", 9.80665e3)]
[Unit("PoundFeet", 1.3558179483314004e3)]
[Unit("PoundInches", 0.11298482902762e3)]
public readonly partial struct Torque {
}
