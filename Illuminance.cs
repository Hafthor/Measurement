namespace com.hafthor.Measurement;

[Measurement("lx")]
public readonly partial struct Illuminance {

    // SI units
    public static Illuminance FromKilolux(double kilolux) => new(kilolux * 1e3);
    public double ToKilolux() => value / 1e3;
    public static Illuminance FromLux(double lux) => new(lux);
    public double ToLux() => value;
    public static Illuminance FromMillilux(double millilux) => new(millilux * 1e-3);
    public double ToMillilux() => value / 1e-3;

    // CGS units
    public static Illuminance FromPhots(double phots) => new(phots * 1e4);
    public double ToPhots() => value / 1e4;

    // Imperial / US units
    public static Illuminance FromFootcandles(double footcandles) => new(footcandles * 10.763910416709722);
    public double ToFootcandles() => value / 10.763910416709722;

    // Composite relationships
    public static LuminousFlux operator *(Illuminance illuminance, Area area) => LuminousFlux.FromLumens(illuminance.value * area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousExposure operator *(Illuminance illuminance, Duration duration) => LuminousExposure.FromLuxSeconds(illuminance.ToLux() * duration.ToSeconds());

}
