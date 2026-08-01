namespace com.hafthor.Measurement;

[Measurement("A", VariableName = "microamperes", DisplayFactor = 1e6)]
[SiUnit("Amperes", 6, "None Kilo Milli Micro Nano Pico")]
[SiUnit("Abamperes", 7)]
[Unit("Statamperes", 3.335641e-4)]
[Product<Area, CurrentDensity>]
[Product<Length, MagneticFieldStrength>]
public readonly partial struct ElectricCurrent { }
