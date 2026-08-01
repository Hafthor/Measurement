namespace com.hafthor.Measurement;

[Measurement("Gy", VariableName = "micrograys", DisplayFactor = 1e6)]
[SiUnit("Grays", 6, "None Kilo Milli Micro")]
[SiUnit("Rads", 4, "None Milli")]
public readonly partial struct AbsorbedDose {
    // Composite relationships (derived)
    public static DoseRate operator /(AbsorbedDose absorbedDose, Duration duration) => DoseRate.FromGraysPerSecond(absorbedDose.ToGrays() / duration.ToSeconds());
}
