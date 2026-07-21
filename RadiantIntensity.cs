namespace com.hafthor.Measurement;

public class RadiantIntensity {
    private readonly double wattsPerSteradian;

    private RadiantIntensity(double wattsPerSteradian) => this.wattsPerSteradian = wattsPerSteradian;

    // Arithmetic
    public static RadiantIntensity operator +(RadiantIntensity a, RadiantIntensity b) => new(a.wattsPerSteradian + b.wattsPerSteradian);
    public static RadiantIntensity operator -(RadiantIntensity a, RadiantIntensity b) => new(a.wattsPerSteradian - b.wattsPerSteradian);
    public static RadiantIntensity operator -(RadiantIntensity x) => new(-x.wattsPerSteradian);

    // Units
    public static RadiantIntensity FromWattsPerSteradian(double wattsPerSteradian) => new(wattsPerSteradian);
    public double ToWattsPerSteradian() => wattsPerSteradian;

    // Composite relationships
    public static Power operator *(RadiantIntensity radiantIntensity, SolidAngle solidAngle) => Power.FromWatts(radiantIntensity.ToWattsPerSteradian() * solidAngle.ToSteradians());
    public static Power operator *(SolidAngle solidAngle, RadiantIntensity radiantIntensity) => Power.FromWatts(solidAngle.ToSteradians() * radiantIntensity.ToWattsPerSteradian());

    public override string ToString() => $"{wattsPerSteradian} W/sr";

    public override bool Equals(object obj) => obj is RadiantIntensity other && other.wattsPerSteradian == wattsPerSteradian;
    public override int GetHashCode() => wattsPerSteradian.GetHashCode();
}
