namespace com.hafthor.Measurement;

public class Ratio {
    private readonly double value;

    private Ratio(double value) => this.value = value;

    // Arithmetic
    public static Ratio operator +(Ratio a, Ratio b) => new Ratio(a.value + b.value);
    public static Ratio operator -(Ratio a, Ratio b) => new Ratio(a.value - b.value);
    public static Ratio operator -(Ratio x) => new Ratio(-x.value);

    // Units
    public static Ratio FromRatio(double ratio) => new Ratio(ratio);
    public double ToRatio() => value;
    public static Ratio FromPercent(double percent) => new Ratio(percent * (1e-2));
    public double ToPercent() => value / (1e-2);
    public static Ratio FromPerMille(double perMille) => new Ratio(perMille * (1e-3));
    public double ToPerMille() => value / (1e-3);
    public static Ratio FromPartsPerMillion(double partsPerMillion) => new Ratio(partsPerMillion * (1e-6));
    public double ToPartsPerMillion() => value / (1e-6);
    public static Ratio FromPartsPerBillion(double partsPerBillion) => new Ratio(partsPerBillion * (1e-9));
    public double ToPartsPerBillion() => value / (1e-9);

    // Logarithmic (power) decibels
    public static Ratio FromDecibels(double decibels) => new Ratio(Math.Pow(10, decibels / 10));
    public double ToDecibels() => 10 * Math.Log10(value);
}
