namespace com.hafthor.Measurement;

[Measurement("C", DisplayFactor = 1e9)]
public readonly partial struct ElectricCharge {

    // Canonical (stored) unit is the nanocoulomb, so nC/µC/mC-scale values land on exact
    // integers in IEEE-754; ToString presents coulombs (DisplayFactor = 1e9).
    public static ElectricCharge FromKilocoulombs(double kilocoulombs) => new(kilocoulombs * 1e12);
    public double ToKilocoulombs() => value / 1e12;
    public static ElectricCharge FromCoulombs(double coulombs) => new(coulombs * 1e9);
    public double ToCoulombs() => value / 1e9;
    public static ElectricCharge FromMillicoulombs(double millicoulombs) => new(millicoulombs * 1e6);
    public double ToMillicoulombs() => value / 1e6;
    public static ElectricCharge FromMicrocoulombs(double microcoulombs) => new(microcoulombs * 1e3);
    public double ToMicrocoulombs() => value / 1e3;
    public static ElectricCharge FromNanocoulombs(double nanocoulombs) => new(nanocoulombs);
    public double ToNanocoulombs() => value;

    // Battery-capacity units
    public static ElectricCharge FromAmpereHours(double ampereHours) => new(ampereHours * 3.6e12);
    public double ToAmpereHours() => value / 3.6e12;
    public static ElectricCharge FromMilliampereHours(double milliampereHours) => new(milliampereHours * 3.6e9);
    public double ToMilliampereHours() => value / 3.6e9;

    // Physical & CGS units
    public static ElectricCharge FromFaradays(double faradays) => new(faradays * 96485.33212e9);
    public double ToFaradays() => value / 96485.33212e9;
    public static ElectricCharge FromElementaryCharges(double elementaryCharges) => new(elementaryCharges * 1.602176634e-10);
    public double ToElementaryCharges() => value / 1.602176634e-10;
    public static ElectricCharge FromAbcoulombs(double abcoulombs) => new(abcoulombs * 1e10);
    public double ToAbcoulombs() => value / 1e10;
    public static ElectricCharge FromStatcoulombs(double statcoulombs) => new(statcoulombs * 3.335641e-1);
    public double ToStatcoulombs() => value / 3.335641e-1;

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
