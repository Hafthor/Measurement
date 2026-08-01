namespace com.hafthor.Measurement;

[Measurement("", VariableName = "picoratio", DisplayFactor = 1e12)]
[SiUnit("Ratio", 12)]
[SiUnit("Percent", 10)]
[SiUnit("PerMille", 9)]
[SiUnit("PartsPerMillion", 6)]
[SiUnit("PartsPerBillion", 3)]
[SiUnit("PartsPerTrillion", 0)]
[UnitHook("Decibels")]
public readonly partial struct Ratio {
    // Logarithmic (power) decibels
    public static Ratio FromDecibels(double decibels) => new(Math.Pow(10, decibels / 10) * 1e12);
    public double ToDecibels() => 10 * Math.Log10(picoratio / 1e12);
}
