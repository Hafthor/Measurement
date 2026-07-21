namespace com.hafthor.Measurement;

[Measurement("C")]
public readonly partial struct ElectricCharge {

    // SI units
    public static ElectricCharge FromKilocoulombs(double kilocoulombs) => new(kilocoulombs * 1e3);
    public double ToKilocoulombs() => value / 1e3;
    public static ElectricCharge FromCoulombs(double coulombs) => new(coulombs);
    public double ToCoulombs() => value;
    public static ElectricCharge FromMillicoulombs(double millicoulombs) => new(millicoulombs * 1e-3);
    public double ToMillicoulombs() => value / 1e-3;
    public static ElectricCharge FromMicrocoulombs(double microcoulombs) => new(microcoulombs * 1e-6);
    public double ToMicrocoulombs() => value / 1e-6;
    public static ElectricCharge FromNanocoulombs(double nanocoulombs) => new(nanocoulombs * 1e-9);
    public double ToNanocoulombs() => value / 1e-9;

    // Battery-capacity units
    public static ElectricCharge FromAmpereHours(double ampereHours) => new(ampereHours * 3600);
    public double ToAmpereHours() => value / 3600;
    public static ElectricCharge FromMilliampereHours(double milliampereHours) => new(milliampereHours * 3.6);
    public double ToMilliampereHours() => value / 3.6;

    // Physical & CGS units
    public static ElectricCharge FromFaradays(double faradays) => new(faradays * 96485.33212);
    public double ToFaradays() => value / 96485.33212;
    public static ElectricCharge FromElementaryCharges(double elementaryCharges) => new(elementaryCharges * 1.602176634e-19);
    public double ToElementaryCharges() => value / 1.602176634e-19;
    public static ElectricCharge FromAbcoulombs(double abcoulombs) => new(abcoulombs * 10);
    public double ToAbcoulombs() => value / 10;
    public static ElectricCharge FromStatcoulombs(double statcoulombs) => new(statcoulombs * 3.335641e-10);
    public double ToStatcoulombs() => value / 3.335641e-10;

    // Composite relationships
    public static ElectricCurrent operator /(ElectricCharge charge, Duration duration) => ElectricCurrent.FromAmperes(charge.value / duration.ToSeconds());
    public static Duration operator /(ElectricCharge charge, ElectricCurrent current) => Duration.FromSeconds(charge.value / current.ToAmperes());
    public static Capacitance operator /(ElectricCharge charge, Voltage voltage) => Capacitance.FromFarads(charge.value / voltage.ToVolts());
    public static Voltage operator /(ElectricCharge charge, Capacitance capacitance) => Voltage.FromVolts(charge.value / capacitance.ToFarads());

    // Composite relationships (derived)
    public static ChargeDensity operator /(ElectricCharge electricCharge, Volume volume) => ChargeDensity.FromCoulombsPerCubicMeter(electricCharge.ToCoulombs() / volume.ToCubicMeters());
    public static SurfaceChargeDensity operator /(ElectricCharge electricCharge, Area area) => SurfaceChargeDensity.FromCoulombsPerSquareMeter(electricCharge.ToCoulombs() / area.ToSquareMeters());
    public static ElectricDipoleMoment operator *(ElectricCharge electricCharge, Length length) => ElectricDipoleMoment.FromCoulombMeters(electricCharge.ToCoulombs() * length.ToMeters());
    public static Exposure operator /(ElectricCharge electricCharge, Mass mass) => Exposure.FromCoulombsPerKilogram(electricCharge.ToCoulombs() / mass.ToKilograms());

    // Famous relations
    public static Energy operator *(ElectricCharge charge, Voltage voltage) => Energy.FromJoules(charge.ToCoulombs() * voltage.ToVolts());

}
