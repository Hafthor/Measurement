namespace com.hafthor.Measurement;

[Measurement("C", VariableName = "nanocoulombs", DisplayFactor = 1e9)]
[SiUnit("Coulombs", 9, "None Kilo Milli Micro Nano")]
[Unit("AmpereHours", 3.6e12)]
[Unit("MilliampereHours", 3.6e9)]
[Unit("Faradays", 96485.33212e9)]
[Unit("ElementaryCharges", 1.602176634e-10)]
[SiUnit("Abcoulombs", 10)]
[Unit("Statcoulombs", 3.335641e-1)]
public readonly partial struct ElectricCharge {
    // Composite relationships
    public static ElectricCurrent operator /(ElectricCharge charge, Duration duration) => ElectricCurrent.FromAmperes(charge.ToCoulombs() / duration.ToSeconds());
    public static Duration operator /(ElectricCharge charge, ElectricCurrent current) => Duration.FromSeconds(charge.ToCoulombs() / current.ToAmperes());
    public static Capacitance operator /(ElectricCharge charge, Voltage voltage) => Capacitance.FromFarads(charge.ToCoulombs() / voltage.ToVolts());
    public static Voltage operator /(ElectricCharge charge, Capacitance capacitance) => Voltage.FromVolts(charge.ToCoulombs() / capacitance.ToFarads());

    // Composite relationships (derived)
    public static ChargeDensity operator /(ElectricCharge electricCharge, Volume volume) => ChargeDensity.FromCoulombsPerCubicMeter(electricCharge.ToCoulombs() / volume.ToCubicMeters());
    public static SurfaceChargeDensity operator /(ElectricCharge electricCharge, Area area) => SurfaceChargeDensity.FromCoulombsPerSquareMeter(electricCharge.ToCoulombs() / area.ToSquareMeters());
    public static ElectricDipoleMoment operator *(ElectricCharge electricCharge, Length length) => ElectricDipoleMoment.FromCoulombMeters(electricCharge.ToCoulombs() * length.ToMeters());
    public static Exposure operator /(ElectricCharge electricCharge, Mass mass) => Exposure.FromCoulombsPerKilogram(electricCharge.ToCoulombs() / mass.ToKilograms());

    // Famous relations
    public static Energy operator *(ElectricCharge charge, Voltage voltage) => Energy.FromJoules(charge.ToCoulombs() * voltage.ToVolts());
}
