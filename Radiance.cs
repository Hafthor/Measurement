namespace com.hafthor.Measurement;

public class Radiance {
    private readonly double wattsPerSquareMeterSteradian;

    private Radiance(double wattsPerSquareMeterSteradian) => this.wattsPerSquareMeterSteradian = wattsPerSquareMeterSteradian;

    // Arithmetic
    public static Radiance operator +(Radiance a, Radiance b) => new(a.wattsPerSquareMeterSteradian + b.wattsPerSquareMeterSteradian);
    public static Radiance operator -(Radiance a, Radiance b) => new(a.wattsPerSquareMeterSteradian - b.wattsPerSquareMeterSteradian);
    public static Radiance operator -(Radiance x) => new(-x.wattsPerSquareMeterSteradian);

    // Units
    public static Radiance FromWattsPerSquareMeterSteradian(double wattsPerSquareMeterSteradian) => new(wattsPerSquareMeterSteradian);
    public double ToWattsPerSquareMeterSteradian() => wattsPerSquareMeterSteradian;

    public override string ToString() => $"{wattsPerSquareMeterSteradian} W/(m²·sr)";

    public override bool Equals(object obj) => obj is Radiance other && other.wattsPerSquareMeterSteradian == wattsPerSquareMeterSteradian;
    public override int GetHashCode() => wattsPerSquareMeterSteradian.GetHashCode();
}
