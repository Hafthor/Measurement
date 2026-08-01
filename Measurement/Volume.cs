namespace com.hafthor.Measurement;

[Measurement("m³", VariableName = "microliters", DisplayFactor = 1e9)]
[SiUnit("CubicMeters", 9)]
[SiUnit("CubicCentimeters", 3)]
[SiUnit("CubicMillimeters", 0)]
[SiUnit("Liters", 6, "None Kilo Milli Micro")]
[Unit("Gallons", 0.003785411784e9)]
[Unit("Quarts", 0.000946352946e9)]
[Unit("Pints", 0.000473176473e9)]
[Unit("Cups", 0.0002365882365e9)]
[Unit("FluidOunces", 2.95735295625e4)]
[Unit("Tablespoons", 1.4786764828125e4)]
[Unit("Teaspoons", 4.92892159375e3)]
[Unit("ImperialGallons", 0.00454609e9)]
[Unit("CubicYards", 0.764554857984e9)]
[Unit("CubicFeet", 0.028316846592e9)]
[Unit("CubicInches", 1.6387064e4)]
[Unit("OilBarrels", 0.158987294928e9)]
public readonly partial struct Volume {
    // Composite relationships
    public static Area operator /(Volume volume, Length length) => Area.FromSquareMeters(volume.ToCubicMeters() / length.ToMeters());
    public static Length operator /(Volume volume, Area area) => Length.FromMeters(volume.ToCubicMeters() / area.ToSquareMeters());

    // Composite relationships (derived)
    public static VolumetricFlowRate operator /(Volume volume, Duration duration) => VolumetricFlowRate.FromCubicMetersPerSecond(volume.ToCubicMeters() / duration.ToSeconds());
    public static SpecificVolume operator /(Volume volume, Mass mass) => SpecificVolume.FromCubicMetersPerKilogram(volume.ToCubicMeters() / mass.ToKilograms());

    // Famous relations
    public static Energy operator *(Volume volume, Pressure pressure) => Energy.FromJoules(volume.ToCubicMeters() * pressure.ToPascals());
}
