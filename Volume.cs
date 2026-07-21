namespace com.hafthor.Measurement;

public sealed class Volume : Measurement<Volume> {

    private Volume(double value) : base(value) { }

    protected override Volume Create(double value) => new(value);
    protected override string Symbol => "m³";

    // SI units
    public static Volume FromCubicMeters(double value) => new(value);
    public double ToCubicMeters() => value;
    public static Volume FromCubicCentimeters(double cubicCentimeters) => new(cubicCentimeters * 1e-6);
    public double ToCubicCentimeters() => value / 1e-6;
    public static Volume FromCubicMillimeters(double cubicMillimeters) => new(cubicMillimeters * 1e-9);
    public double ToCubicMillimeters() => value / 1e-9;
    public static Volume FromKiloliters(double kiloliters) => new(kiloliters);
    public double ToKiloliters() => value;
    public static Volume FromLiters(double liters) => new(liters * 1e-3);
    public double ToLiters() => value / 1e-3;
    public static Volume FromMilliliters(double milliliters) => new(milliliters * 1e-6);
    public double ToMilliliters() => value / 1e-6;
    public static Volume FromMicroliters(double microliters) => new(microliters * 1e-9);
    public double ToMicroliters() => value / 1e-9;

    // US customary liquid units
    public static Volume FromGallons(double gallons) => new(gallons * 0.003785411784);
    public double ToGallons() => value / 0.003785411784;
    public static Volume FromQuarts(double quarts) => new(quarts * 0.000946352946);
    public double ToQuarts() => value / 0.000946352946;
    public static Volume FromPints(double pints) => new(pints * 0.000473176473);
    public double ToPints() => value / 0.000473176473;
    public static Volume FromCups(double cups) => new(cups * 0.0002365882365);
    public double ToCups() => value / 0.0002365882365;
    public static Volume FromFluidOunces(double fluidOunces) => new(fluidOunces * 2.95735295625e-5);
    public double ToFluidOunces() => value / 2.95735295625e-5;
    public static Volume FromTablespoons(double tablespoons) => new(tablespoons * 1.4786764828125e-5);
    public double ToTablespoons() => value / 1.4786764828125e-5;
    public static Volume FromTeaspoons(double teaspoons) => new(teaspoons * 4.92892159375e-6);
    public double ToTeaspoons() => value / 4.92892159375e-6;

    // Imperial units
    public static Volume FromImperialGallons(double imperialGallons) => new(imperialGallons * 0.00454609);
    public double ToImperialGallons() => value / 0.00454609;

    // Other units
    public static Volume FromCubicYards(double cubicYards) => new(cubicYards * 0.764554857984);
    public double ToCubicYards() => value / 0.764554857984;
    public static Volume FromCubicFeet(double cubicFeet) => new(cubicFeet * 0.028316846592);
    public double ToCubicFeet() => value / 0.028316846592;
    public static Volume FromCubicInches(double cubicInches) => new(cubicInches * 1.6387064e-5);
    public double ToCubicInches() => value / 1.6387064e-5;
    public static Volume FromOilBarrels(double oilBarrels) => new(oilBarrels * 0.158987294928);
    public double ToOilBarrels() => value / 0.158987294928;

    // Composite relationships
    public static Area operator /(Volume volume, Length length) => Area.FromSquareMeters(volume.value / length.ToMeters());
    public static Length operator /(Volume volume, Area area) => Length.FromMeters(volume.value / area.ToSquareMeters());

    // Composite relationships (derived)
    public static VolumetricFlowRate operator /(Volume volume, Duration duration) => VolumetricFlowRate.FromCubicMetersPerSecond(volume.ToCubicMeters() / duration.ToSeconds());
    public static SpecificVolume operator /(Volume volume, Mass mass) => SpecificVolume.FromCubicMetersPerKilogram(volume.ToCubicMeters() / mass.ToKilograms());

    // Famous relations
    public static Energy operator *(Volume volume, Pressure pressure) => Energy.FromJoules(volume.ToCubicMeters() * pressure.ToPascals());

}
