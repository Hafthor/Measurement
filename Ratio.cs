namespace com.hafthor.Measurement;

[Measurement("")]
public readonly partial struct Ratio {

    // Units
    public static Ratio FromRatio(double ratio) => new(ratio);
    public double ToRatio() => value;
    public static Ratio FromPercent(double percent) => new(percent * (1e-2));
    public double ToPercent() => value / (1e-2);
    public static Ratio FromPerMille(double perMille) => new(perMille * (1e-3));
    public double ToPerMille() => value / (1e-3);
    public static Ratio FromPartsPerMillion(double partsPerMillion) => new(partsPerMillion * (1e-6));
    public double ToPartsPerMillion() => value / (1e-6);
    public static Ratio FromPartsPerBillion(double partsPerBillion) => new(partsPerBillion * (1e-9));
    public double ToPartsPerBillion() => value / (1e-9);

    // Logarithmic (power) decibels
    public static Ratio FromDecibels(double decibels) => new(Math.Pow(10, decibels / 10));
    public double ToDecibels() => 10 * Math.Log10(value);

}
