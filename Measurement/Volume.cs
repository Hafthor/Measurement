namespace com.hafthor.Measurement;

[Measurement("m³", VariableName = "microliters", DisplayFactor = 1e9)]
public readonly partial struct Volume {
    // The canonical (stored) unit is the microlitre, so microlitre/millilitre/litre-scale
    // values land on exact integers in IEEE-754; ToString presents cubic metres (DisplayFactor = 1e9).
    public static Volume FromCubicMeters(double cubicMeters) => new(cubicMeters * 1e9);
    public double ToCubicMeters() => microliters / 1e9;
    public static Volume FromCubicCentimeters(double cubicCentimeters) => new(cubicCentimeters * 1e3);
    public double ToCubicCentimeters() => microliters / 1e3;
    public static Volume FromCubicMillimeters(double cubicMillimeters) => new(cubicMillimeters);
    public double ToCubicMillimeters() => microliters;
    public static Volume FromKiloliters(double kiloliters) => new(kiloliters * 1e9);
    public double ToKiloliters() => microliters / 1e9;
    public static Volume FromLiters(double liters) => new(liters * 1e6);
    public double ToLiters() => microliters / 1e6;
    public static Volume FromMilliliters(double milliliters) => new(milliliters * 1e3);
    public double ToMilliliters() => microliters / 1e3;
    public static Volume FromMicroliters(double microliters) => new(microliters);
    public double ToMicroliters() => microliters;

    // US customary liquid units
    public static Volume FromGallons(double gallons) => new(gallons * 0.003785411784e9);
    public double ToGallons() => microliters / 0.003785411784e9;
    public static Volume FromQuarts(double quarts) => new(quarts * 0.000946352946e9);
    public double ToQuarts() => microliters / 0.000946352946e9;
    public static Volume FromPints(double pints) => new(pints * 0.000473176473e9);
    public double ToPints() => microliters / 0.000473176473e9;
    public static Volume FromCups(double cups) => new(cups * 0.0002365882365e9);
    public double ToCups() => microliters / 0.0002365882365e9;
    public static Volume FromFluidOunces(double fluidOunces) => new(fluidOunces * 2.95735295625e4);
    public double ToFluidOunces() => microliters / 2.95735295625e4;
    public static Volume FromTablespoons(double tablespoons) => new(tablespoons * 1.4786764828125e4);
    public double ToTablespoons() => microliters / 1.4786764828125e4;
    public static Volume FromTeaspoons(double teaspoons) => new(teaspoons * 4.92892159375e3);
    public double ToTeaspoons() => microliters / 4.92892159375e3;

    // Imperial units
    public static Volume FromImperialGallons(double imperialGallons) => new(imperialGallons * 0.00454609e9);
    public double ToImperialGallons() => microliters / 0.00454609e9;

    // Other units
    public static Volume FromCubicYards(double cubicYards) => new(cubicYards * 0.764554857984e9);
    public double ToCubicYards() => microliters / 0.764554857984e9;
    public static Volume FromCubicFeet(double cubicFeet) => new(cubicFeet * 0.028316846592e9);
    public double ToCubicFeet() => microliters / 0.028316846592e9;
    public static Volume FromCubicInches(double cubicInches) => new(cubicInches * 1.6387064e4);
    public double ToCubicInches() => microliters / 1.6387064e4;
    public static Volume FromOilBarrels(double oilBarrels) => new(oilBarrels * 0.158987294928e9);
    public double ToOilBarrels() => microliters / 0.158987294928e9;

    // Composite relationships
    public static Area operator /(Volume volume, Length length) => Area.FromSquareMeters(volume.ToCubicMeters() / length.ToMeters());
    public static Length operator /(Volume volume, Area area) => Length.FromMeters(volume.ToCubicMeters() / area.ToSquareMeters());

    // Composite relationships (derived)
    public static VolumetricFlowRate operator /(Volume volume, Duration duration) => VolumetricFlowRate.FromCubicMetersPerSecond(volume.ToCubicMeters() / duration.ToSeconds());
    public static SpecificVolume operator /(Volume volume, Mass mass) => SpecificVolume.FromCubicMetersPerKilogram(volume.ToCubicMeters() / mass.ToKilograms());

    // Famous relations
    public static Energy operator *(Volume volume, Pressure pressure) => Energy.FromJoules(volume.ToCubicMeters() * pressure.ToPascals());
}
