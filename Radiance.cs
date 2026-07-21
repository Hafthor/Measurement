namespace com.hafthor.Measurement;

[Measurement("W/(m²·sr)")]
public readonly partial struct Radiance {

    // Units
    public static Radiance FromWattsPerSquareMeterSteradian(double wattsPerSquareMeterSteradian) => new(wattsPerSquareMeterSteradian);
    public double ToWattsPerSquareMeterSteradian() => value;

}
