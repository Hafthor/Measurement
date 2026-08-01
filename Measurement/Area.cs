namespace com.hafthor.Measurement;

[Measurement("m²", VariableName = "squareMillimeters", DisplayFactor = 1e6)]
[SiUnit("SquareKilometers", 12)]
[SiUnit("Hectares", 10)]
[SiUnit("Ares", 8)]
[SiUnit("SquareMeters", 6)]
[SiUnit("SquareCentimeters", 2)]
[SiUnit("SquareMillimeters", 0)]
[Unit("SquareMiles", 2589988.110336e6)]
[Unit("Acres", 4046.8564224e6)]
[Unit("SquareYards", 0.83612736e6)]
[Unit("SquareFeet", 0.09290304e6)]
[Unit("SquareInches", 0.00064516e6)]
[SiUnit("Barns", -22)]
[Product<Length, Length>]
[Product<Duration, KinematicViscosity>]
public readonly partial struct Area { }
