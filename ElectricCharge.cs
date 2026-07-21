namespace com.hafthor.Measurement;

public class ElectricCharge {
    private readonly double coulombs;

    private ElectricCharge(double coulombs) => this.coulombs = coulombs;

    // Arithmetic
    public static ElectricCharge operator +(ElectricCharge a, ElectricCharge b) => new(a.coulombs + b.coulombs);
    public static ElectricCharge operator -(ElectricCharge a, ElectricCharge b) => new(a.coulombs - b.coulombs);
    public static ElectricCharge operator -(ElectricCharge x) => new(-x.coulombs);

    // SI units
    public static ElectricCharge FromKilocoulombs(double kilocoulombs) => new(kilocoulombs * 1e3);
    public double ToKilocoulombs() => coulombs / 1e3;
    public static ElectricCharge FromCoulombs(double coulombs) => new(coulombs);
    public double ToCoulombs() => coulombs;
    public static ElectricCharge FromMillicoulombs(double millicoulombs) => new(millicoulombs * 1e-3);
    public double ToMillicoulombs() => coulombs / 1e-3;
    public static ElectricCharge FromMicrocoulombs(double microcoulombs) => new(microcoulombs * 1e-6);
    public double ToMicrocoulombs() => coulombs / 1e-6;
    public static ElectricCharge FromNanocoulombs(double nanocoulombs) => new(nanocoulombs * 1e-9);
    public double ToNanocoulombs() => coulombs / 1e-9;

    // Battery-capacity units
    public static ElectricCharge FromAmpereHours(double ampereHours) => new(ampereHours * 3600);
    public double ToAmpereHours() => coulombs / 3600;
    public static ElectricCharge FromMilliampereHours(double milliampereHours) => new(milliampereHours * 3.6);
    public double ToMilliampereHours() => coulombs / 3.6;

    // Physical & CGS units
    public static ElectricCharge FromFaradays(double faradays) => new(faradays * 96485.33212);
    public double ToFaradays() => coulombs / 96485.33212;
    public static ElectricCharge FromElementaryCharges(double elementaryCharges) => new(elementaryCharges * 1.602176634e-19);
    public double ToElementaryCharges() => coulombs / 1.602176634e-19;
    public static ElectricCharge FromAbcoulombs(double abcoulombs) => new(abcoulombs * 10);
    public double ToAbcoulombs() => coulombs / 10;
    public static ElectricCharge FromStatcoulombs(double statcoulombs) => new(statcoulombs * 3.335641e-10);
    public double ToStatcoulombs() => coulombs / 3.335641e-10;

    // Composite relationships
    public static ElectricCurrent operator /(ElectricCharge charge, Duration duration) => ElectricCurrent.FromAmperes(charge.coulombs / duration.ToSeconds());
    public static Duration operator /(ElectricCharge charge, ElectricCurrent current) => Duration.FromSeconds(charge.coulombs / current.ToAmperes());
    public static Capacitance operator /(ElectricCharge charge, Voltage voltage) => Capacitance.FromFarads(charge.coulombs / voltage.ToVolts());
    public static Voltage operator /(ElectricCharge charge, Capacitance capacitance) => Voltage.FromVolts(charge.coulombs / capacitance.ToFarads());

    // Composite relationships (derived)
    public static ChargeDensity operator /(ElectricCharge electricCharge, Volume volume) => ChargeDensity.FromCoulombsPerCubicMeter(electricCharge.ToCoulombs() / volume.ToCubicMeters());
    public static SurfaceChargeDensity operator /(ElectricCharge electricCharge, Area area) => SurfaceChargeDensity.FromCoulombsPerSquareMeter(electricCharge.ToCoulombs() / area.ToSquareMeters());
    public static ElectricDipoleMoment operator *(ElectricCharge electricCharge, Length length) => ElectricDipoleMoment.FromCoulombMeters(electricCharge.ToCoulombs() * length.ToMeters());
    public static Exposure operator /(ElectricCharge electricCharge, Mass mass) => Exposure.FromCoulombsPerKilogram(electricCharge.ToCoulombs() / mass.ToKilograms());

    // Famous relations
    public static Energy operator *(ElectricCharge charge, Voltage voltage) => Energy.FromJoules(charge.ToCoulombs() * voltage.ToVolts());

    public override string ToString() => $"{coulombs} C";
}
