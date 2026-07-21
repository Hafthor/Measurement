namespace com.hafthor.Measurement;

public class Pressure {
    private readonly double pascals;

    private Pressure(double pascals) => this.pascals = pascals;

    // Arithmetic
    public static Pressure operator +(Pressure a, Pressure b) => new(a.pascals + b.pascals);
    public static Pressure operator -(Pressure a, Pressure b) => new(a.pascals - b.pascals);
    public static Pressure operator -(Pressure x) => new(-x.pascals);

    // SI units
    public static Pressure FromMegapascals(double megapascals) => new(megapascals * 1e6);
    public double ToMegapascals() => pascals / 1e6;
    public static Pressure FromKilopascals(double kilopascals) => new(kilopascals * 1e3);
    public double ToKilopascals() => pascals / 1e3;
    public static Pressure FromHectopascals(double hectopascals) => new(hectopascals * 1e2);
    public double ToHectopascals() => pascals / 1e2;
    public static Pressure FromPascals(double pascals) => new(pascals);
    public double ToPascals() => pascals;

    // Bar units
    public static Pressure FromBars(double bars) => new(bars * 1e5);
    public double ToBars() => pascals / 1e5;
    public static Pressure FromMillibars(double millibars) => new(millibars * 1e2);
    public double ToMillibars() => pascals / 1e2;

    // Atmospheric & column units
    public static Pressure FromAtmospheres(double atmospheres) => new(atmospheres * 101325);
    public double ToAtmospheres() => pascals / 101325;
    public static Pressure FromTorr(double torr) => new(torr * 133.32236842105263);
    public double ToTorr() => pascals / 133.32236842105263;
    public static Pressure FromMillimetersOfMercury(double millimetersOfMercury) => new(millimetersOfMercury * 133.322387415);
    public double ToMillimetersOfMercury() => pascals / 133.322387415;
    public static Pressure FromInchesOfMercury(double inchesOfMercury) => new(inchesOfMercury * 3386.389);
    public double ToInchesOfMercury() => pascals / 3386.389;
    public static Pressure FromInchesOfWater(double inchesOfWater) => new(inchesOfWater * 249.08891);
    public double ToInchesOfWater() => pascals / 249.08891;

    // Imperial / US units
    public static Pressure FromPoundsPerSquareInch(double poundsPerSquareInch) => new(poundsPerSquareInch * 6894.757293168);
    public double ToPoundsPerSquareInch() => pascals / 6894.757293168;

    // Composite relationships
    public static Force operator *(Pressure pressure, Area area) => Force.FromNewtons(pressure.pascals * area.ToSquareMeters());

    // Composite relationships (derived)
    public static DynamicViscosity operator *(Pressure pressure, Duration duration) => DynamicViscosity.FromPascalSeconds(pressure.ToPascals() * duration.ToSeconds());

    // Famous relations
    public static Energy operator *(Pressure pressure, Volume volume) => Energy.FromJoules(pressure.ToPascals() * volume.ToCubicMeters());
}
