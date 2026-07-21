namespace com.hafthor.Measurement;

public sealed class RadiantIntensity : Measurement<RadiantIntensity> {

    private RadiantIntensity(double value) : base(value) { }

    protected override RadiantIntensity Create(double value) => new(value);
    protected override string Symbol => "W/sr";

    // Units
    public static RadiantIntensity FromWattsPerSteradian(double value) => new(value);
    public double ToWattsPerSteradian() => value;

    // Composite relationships
    public static Power operator *(RadiantIntensity radiantIntensity, SolidAngle solidAngle) => Power.FromWatts(radiantIntensity.ToWattsPerSteradian() * solidAngle.ToSteradians());
    public static Power operator *(SolidAngle solidAngle, RadiantIntensity radiantIntensity) => Power.FromWatts(solidAngle.ToSteradians() * radiantIntensity.ToWattsPerSteradian());

}
