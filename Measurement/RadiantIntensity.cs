namespace com.hafthor.Measurement;

[Measurement("W/sr", VariableName = "wattsPerSteradian")]
[SiUnit("WattsPerSteradian", 0)]
public readonly partial struct RadiantIntensity {
    // Composite relationships
    public static Power operator *(RadiantIntensity radiantIntensity, SolidAngle solidAngle) => Power.FromWatts(radiantIntensity.ToWattsPerSteradian() * solidAngle.ToSteradians());
    public static Power operator *(SolidAngle solidAngle, RadiantIntensity radiantIntensity) => Power.FromWatts(solidAngle.ToSteradians() * radiantIntensity.ToWattsPerSteradian());
}
