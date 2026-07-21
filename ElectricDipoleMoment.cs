namespace com.hafthor.Measurement;

public sealed class ElectricDipoleMoment : Measurement<ElectricDipoleMoment> {

    private ElectricDipoleMoment(double value) : base(value) { }

    protected override ElectricDipoleMoment Create(double value) => new(value);
    protected override string Symbol => "C·m";

    // Units
    public static ElectricDipoleMoment FromCoulombMeters(double value) => new(value);
    public double ToCoulombMeters() => value;
    public static ElectricDipoleMoment FromDebyes(double debyes) => new(debyes * (3.33564095198e-30));
    public double ToDebyes() => value / (3.33564095198e-30);

    // Composite relationships
    public static ElectricCharge operator /(ElectricDipoleMoment electricDipoleMoment, Length length) => ElectricCharge.FromCoulombs(electricDipoleMoment.ToCoulombMeters() / length.ToMeters());
    public static Length operator /(ElectricDipoleMoment electricDipoleMoment, ElectricCharge electricCharge) => Length.FromMeters(electricDipoleMoment.ToCoulombMeters() / electricCharge.ToCoulombs());

}
