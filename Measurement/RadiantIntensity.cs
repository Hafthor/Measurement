namespace com.hafthor.Measurement;

[Measurement("W/sr")]
public readonly partial struct RadiantIntensity {

    // Units
    public static RadiantIntensity FromWattsPerSteradian(double wattsPerSteradian) => new(wattsPerSteradian);
    public double ToWattsPerSteradian() => value;

    // Composite relationships
    public static Power operator *(RadiantIntensity radiantIntensity, SolidAngle solidAngle) => Power.FromWatts(radiantIntensity.ToWattsPerSteradian() * solidAngle.ToSteradians());
    public static Power operator *(SolidAngle solidAngle, RadiantIntensity radiantIntensity) => Power.FromWatts(solidAngle.ToSteradians() * radiantIntensity.ToWattsPerSteradian());

}
