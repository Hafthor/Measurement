namespace com.hafthor.Measurement;

[Measurement("lx", VariableName = "millilux", DisplayFactor = 1e3)]
[SiUnit("Lux", 3, "None Kilo Milli")]
[SiUnit("Phots", 7)]
[Unit("Footcandles", 10.763910416709722e3)]
public readonly partial struct Illuminance {
    // Composite relationships
    public static LuminousFlux operator *(Illuminance illuminance, Area area) => LuminousFlux.FromLumens(illuminance.ToLux() * area.ToSquareMeters());

    // Composite relationships (derived)
    public static LuminousExposure operator *(Illuminance illuminance, Duration duration) => LuminousExposure.FromLuxSeconds(illuminance.ToLux() * duration.ToSeconds());
}
