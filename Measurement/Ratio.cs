namespace com.hafthor.Measurement;

[Measurement("", VariableName = "picoratio", DisplayFactor = 1e12)]
public readonly partial struct Ratio {
    // Canonical (stored) unit is parts-per-trillion, so integer-ppt (and ppb, ppm, ‰, %)
    // values are exact in IEEE-754; ToString presents the fundamental dimensionless ratio
    // (DisplayFactor = 1e12). Anchoring this fine still keeps ratios exact to ~9000, well
    // beyond the ≤ 1.0 that ratios normally occupy.
    public static Ratio FromRatio(double ratio) => new(ratio * 1e12);
    public double ToRatio() => picoratio / 1e12;
    public static Ratio FromPercent(double percent) => new(percent * 1e10);
    public double ToPercent() => picoratio / 1e10;
    public static Ratio FromPerMille(double perMille) => new(perMille * 1e9);
    public double ToPerMille() => picoratio / 1e9;
    public static Ratio FromPartsPerMillion(double partsPerMillion) => new(partsPerMillion * 1e6);
    public double ToPartsPerMillion() => picoratio / 1e6;
    public static Ratio FromPartsPerBillion(double partsPerBillion) => new(partsPerBillion * 1e3);
    public double ToPartsPerBillion() => picoratio / 1e3;
    public static Ratio FromPartsPerTrillion(double partsPerTrillion) => new(partsPerTrillion);
    public double ToPartsPerTrillion() => picoratio;

    // Logarithmic (power) decibels
    public static Ratio FromDecibels(double decibels) => new(Math.Pow(10, decibels / 10) * 1e12);
    public double ToDecibels() => 10 * Math.Log10(picoratio / 1e12);
}
