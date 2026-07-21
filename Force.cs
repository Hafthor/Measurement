namespace com.hafthor.Measurement;

[Measurement("N")]
public readonly partial struct Force {

    // SI units
    public static Force FromMeganewtons(double meganewtons) => new(meganewtons * 1e6);
    public double ToMeganewtons() => value / 1e6;
    public static Force FromKilonewtons(double kilonewtons) => new(kilonewtons * 1e3);
    public double ToKilonewtons() => value / 1e3;
    public static Force FromNewtons(double newtons) => new(newtons);
    public double ToNewtons() => value;
    public static Force FromMillinewtons(double millinewtons) => new(millinewtons * 1e-3);
    public double ToMillinewtons() => value / 1e-3;
    public static Force FromDynes(double dynes) => new(dynes * 1e-5);
    public double ToDynes() => value / 1e-5;

    // Gravitational & imperial units
    public static Force FromKilogramsForce(double kilogramsForce) => new(kilogramsForce * 9.80665);
    public double ToKilogramsForce() => value / 9.80665;
    public static Force FromPoundsForce(double poundsForce) => new(poundsForce * 4.4482216152605);
    public double ToPoundsForce() => value / 4.4482216152605;
    public static Force FromOuncesForce(double ouncesForce) => new(ouncesForce * 0.27801385095378125);
    public double ToOuncesForce() => value / 0.27801385095378125;
    public static Force FromPoundals(double poundals) => new(poundals * 0.138254954376);
    public double ToPoundals() => value / 0.138254954376;

    // Composite relationships
    public static Acceleration operator /(Force force, Mass mass) => Acceleration.FromMetersPerSecondSquared(force.value / mass.ToKilograms());
    public static Mass operator /(Force force, Acceleration acceleration) => Mass.FromKilograms(force.value / acceleration.ToMetersPerSecondSquared());
    public static Energy operator *(Force force, Length length) => Energy.FromJoules(force.value * length.ToMeters());
    public static Pressure operator /(Force force, Area area) => Pressure.FromPascals(force.value / area.ToSquareMeters());
    public static Power operator *(Force force, Speed speed) => Power.FromWatts(force.value * speed.ToMetersPerSecond());

    // Composite relationships (derived)
    public static SurfaceTension operator /(Force force, Length length) => SurfaceTension.FromNewtonsPerMeter(force.ToNewtons() / length.ToMeters());

    // Famous relations
    public static Momentum operator *(Force force, Duration duration) => Momentum.FromKilogramMetersPerSecond(force.ToNewtons() * duration.ToSeconds());

}
