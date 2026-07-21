namespace com.hafthor.Measurement;

public class Volume {
    private readonly double cubicMeters;

    private Volume(double cubicMeters) => this.cubicMeters = cubicMeters;

    // Arithmetic
    public static Volume operator +(Volume a, Volume b) => new(a.cubicMeters + b.cubicMeters);
    public static Volume operator -(Volume a, Volume b) => new(a.cubicMeters - b.cubicMeters);
    public static Volume operator -(Volume x) => new(-x.cubicMeters);

    // SI units
    public static Volume FromCubicMeters(double cubicMeters) => new(cubicMeters);
    public double ToCubicMeters() => cubicMeters;
    public static Volume FromCubicCentimeters(double cubicCentimeters) => new(cubicCentimeters * 1e-6);
    public double ToCubicCentimeters() => cubicMeters / 1e-6;
    public static Volume FromCubicMillimeters(double cubicMillimeters) => new(cubicMillimeters * 1e-9);
    public double ToCubicMillimeters() => cubicMeters / 1e-9;
    public static Volume FromKiloliters(double kiloliters) => new(kiloliters);
    public double ToKiloliters() => cubicMeters;
    public static Volume FromLiters(double liters) => new(liters * 1e-3);
    public double ToLiters() => cubicMeters / 1e-3;
    public static Volume FromMilliliters(double milliliters) => new(milliliters * 1e-6);
    public double ToMilliliters() => cubicMeters / 1e-6;
    public static Volume FromMicroliters(double microliters) => new(microliters * 1e-9);
    public double ToMicroliters() => cubicMeters / 1e-9;

    // US customary liquid units
    public static Volume FromGallons(double gallons) => new(gallons * 0.003785411784);
    public double ToGallons() => cubicMeters / 0.003785411784;
    public static Volume FromQuarts(double quarts) => new(quarts * 0.000946352946);
    public double ToQuarts() => cubicMeters / 0.000946352946;
    public static Volume FromPints(double pints) => new(pints * 0.000473176473);
    public double ToPints() => cubicMeters / 0.000473176473;
    public static Volume FromCups(double cups) => new(cups * 0.0002365882365);
    public double ToCups() => cubicMeters / 0.0002365882365;
    public static Volume FromFluidOunces(double fluidOunces) => new(fluidOunces * 2.95735295625e-5);
    public double ToFluidOunces() => cubicMeters / 2.95735295625e-5;
    public static Volume FromTablespoons(double tablespoons) => new(tablespoons * 1.4786764828125e-5);
    public double ToTablespoons() => cubicMeters / 1.4786764828125e-5;
    public static Volume FromTeaspoons(double teaspoons) => new(teaspoons * 4.92892159375e-6);
    public double ToTeaspoons() => cubicMeters / 4.92892159375e-6;

    // Imperial units
    public static Volume FromImperialGallons(double imperialGallons) => new(imperialGallons * 0.00454609);
    public double ToImperialGallons() => cubicMeters / 0.00454609;

    // Other units
    public static Volume FromCubicYards(double cubicYards) => new(cubicYards * 0.764554857984);
    public double ToCubicYards() => cubicMeters / 0.764554857984;
    public static Volume FromCubicFeet(double cubicFeet) => new(cubicFeet * 0.028316846592);
    public double ToCubicFeet() => cubicMeters / 0.028316846592;
    public static Volume FromCubicInches(double cubicInches) => new(cubicInches * 1.6387064e-5);
    public double ToCubicInches() => cubicMeters / 1.6387064e-5;
    public static Volume FromOilBarrels(double oilBarrels) => new(oilBarrels * 0.158987294928);
    public double ToOilBarrels() => cubicMeters / 0.158987294928;

    // Composite relationships
    public static Area operator /(Volume volume, Length length) => Area.FromSquareMeters(volume.cubicMeters / length.ToMeters());
    public static Length operator /(Volume volume, Area area) => Length.FromMeters(volume.cubicMeters / area.ToSquareMeters());

    // Composite relationships (derived)
    public static VolumetricFlowRate operator /(Volume volume, Duration duration) => VolumetricFlowRate.FromCubicMetersPerSecond(volume.ToCubicMeters() / duration.ToSeconds());
    public static SpecificVolume operator /(Volume volume, Mass mass) => SpecificVolume.FromCubicMetersPerKilogram(volume.ToCubicMeters() / mass.ToKilograms());

    // Famous relations
    public static Energy operator *(Volume volume, Pressure pressure) => Energy.FromJoules(volume.ToCubicMeters() * pressure.ToPascals());

    public override string ToString() => $"{cubicMeters} m³";
}
