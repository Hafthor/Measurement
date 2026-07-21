namespace com.hafthor.Measurement;

public class Radiance {
    private readonly double wattsPerSquareMeterSteradian;

    private Radiance(double wattsPerSquareMeterSteradian) => this.wattsPerSquareMeterSteradian = wattsPerSquareMeterSteradian;

    // Arithmetic
    public static Radiance operator +(Radiance a, Radiance b) => new Radiance(a.wattsPerSquareMeterSteradian + b.wattsPerSquareMeterSteradian);
    public static Radiance operator -(Radiance a, Radiance b) => new Radiance(a.wattsPerSquareMeterSteradian - b.wattsPerSquareMeterSteradian);
    public static Radiance operator -(Radiance x) => new Radiance(-x.wattsPerSquareMeterSteradian);

    // Units
    public static Radiance FromWattsPerSquareMeterSteradian(double wattsPerSquareMeterSteradian) => new Radiance(wattsPerSquareMeterSteradian);
    public double ToWattsPerSquareMeterSteradian() => wattsPerSquareMeterSteradian;
}
