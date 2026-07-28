namespace com.hafthor.Measurement;

[Measurement("C·m", VariableName = "coulombMeters")]
public readonly partial struct ElectricDipoleMoment {
    // Units
    public static ElectricDipoleMoment FromCoulombMeters(double coulombMeters) => new(coulombMeters);
    public double ToCoulombMeters() => coulombMeters;
    public static ElectricDipoleMoment FromDebyes(double debyes) => new(debyes * (3.33564095198e-30));
    public double ToDebyes() => coulombMeters / (3.33564095198e-30);

    // Composite relationships
    public static ElectricCharge operator /(ElectricDipoleMoment electricDipoleMoment, Length length) => ElectricCharge.FromCoulombs(electricDipoleMoment.ToCoulombMeters() / length.ToMeters());
    public static Length operator /(ElectricDipoleMoment electricDipoleMoment, ElectricCharge electricCharge) => Length.FromMeters(electricDipoleMoment.ToCoulombMeters() / electricCharge.ToCoulombs());
}
