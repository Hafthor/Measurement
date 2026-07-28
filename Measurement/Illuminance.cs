namespace com.hafthor.Measurement;

[Measurement("lx", VariableName = "millilux", DisplayFactor = 1e3)]
public readonly partial struct Illuminance {
    // SI units
    public static Illuminance FromKilolux(double kilolux) => new(kilolux * 1e6);
    public double ToKilolux() => millilux / 1e6;
    public static Illuminance FromLux(double lux) => new(lux * 1e3);
    public double ToLux() => millilux / 1e3;
    public static Illuminance FromMillilux(double millilux) => new(millilux);
    public double ToMillilux() => millilux;

    // CGS units
    public static Illuminance FromPhots(double phots) => new(phots * 1e7);
    public double ToPhots() => millilux / 1e7;

    // Imperial / US units
    public static Illuminance FromFootcandles(double footcandles) => new(footcandles * 10.763910416709722e3);
    public double ToFootcandles() => millilux / 10.763910416709722e3;

    // Composite relationships
    public static LuminousFlux operator *(Illuminance illuminance, Area area) => LuminousFlux.FromLumens(illuminance.ToLux() * area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousExposure operator *(Illuminance illuminance, Duration duration) => LuminousExposure.FromLuxSeconds(illuminance.ToLux() * duration.ToSeconds());
}
