namespace com.hafthor.Measurement;

public class Illuminance {
    private readonly double lux;

    private Illuminance(double lux) => this.lux = lux;

    // Arithmetic
    public static Illuminance operator +(Illuminance a, Illuminance b) => new Illuminance(a.lux + b.lux);
    public static Illuminance operator -(Illuminance a, Illuminance b) => new Illuminance(a.lux - b.lux);
    public static Illuminance operator -(Illuminance x) => new Illuminance(-x.lux);

    // SI units
    public static Illuminance FromKilolux(double kilolux) => new Illuminance(kilolux * 1e3);
    public double ToKilolux() => lux / 1e3;
    public static Illuminance FromLux(double lux) => new Illuminance(lux);
    public double ToLux() => lux;
    public static Illuminance FromMillilux(double millilux) => new Illuminance(millilux * 1e-3);
    public double ToMillilux() => lux / 1e-3;

    // CGS units
    public static Illuminance FromPhots(double phots) => new Illuminance(phots * 1e4);
    public double ToPhots() => lux / 1e4;

    // Imperial / US units
    public static Illuminance FromFootcandles(double footcandles) => new Illuminance(footcandles * 10.763910416709722);
    public double ToFootcandles() => lux / 10.763910416709722;

    // Composite relationships
    public static LuminousFlux operator *(Illuminance illuminance, Area area) => LuminousFlux.FromLumens(illuminance.lux * area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousExposure operator *(Illuminance illuminance, Duration duration) => LuminousExposure.FromLuxSeconds(illuminance.ToLux() * duration.ToSeconds());
}
