namespace com.hafthor.Measurement;

[Measurement("C·m", VariableName = "coulombMeters")]
[SiUnit("CoulombMeters", 0)]
[Unit("Debyes", 3.33564095198e-30)]
public readonly partial struct ElectricDipoleMoment {
    // Composite relationships
    public static ElectricCharge operator /(ElectricDipoleMoment electricDipoleMoment, Length length) => ElectricCharge.FromCoulombs(electricDipoleMoment.ToCoulombMeters() / length.ToMeters());
    public static Length operator /(ElectricDipoleMoment electricDipoleMoment, ElectricCharge electricCharge) => Length.FromMeters(electricDipoleMoment.ToCoulombMeters() / electricCharge.ToCoulombs());
}
