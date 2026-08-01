namespace com.hafthor.Measurement;

[Measurement("Pa", VariableName = "pascals")]
[SiUnit("Pascals", 0, "None Mega Kilo Hecto")]
[SiUnit("Bars", 5, "None Milli")]
[Unit("Atmospheres", 101325)]
[Unit("Torr", 133.32236842105263)]
[Unit("MillimetersOfMercury", 133.322387415)]
[Unit("InchesOfMercury", 3386.389)]
[Unit("InchesOfWater", 249.08891)]
[Unit("PoundsPerSquareInch", 6894.757293168)]
public readonly partial struct Pressure {
    // Composite relationships
    public static Force operator *(Pressure pressure, Area area) => Force.FromNewtons(pressure.pascals * area.ToSquareMeters());

    // Composite relationships (derived)
    public static DynamicViscosity operator *(Pressure pressure, Duration duration) => DynamicViscosity.FromPascalSeconds(pressure.ToPascals() * duration.ToSeconds());

    // Famous relations
    public static Energy operator *(Pressure pressure, Volume volume) => Energy.FromJoules(pressure.ToPascals() * volume.ToCubicMeters());
}
