namespace com.hafthor.Measurement;

[Measurement("sr", VariableName = "steradians")]
[SiUnit("Steradians", 0)]
[Unit("Spats", 4 * Math.PI)]
[Unit("SquareDegrees", Math.PI * Math.PI / 32400)]
public readonly partial struct SolidAngle { }
