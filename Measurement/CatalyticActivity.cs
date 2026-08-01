namespace com.hafthor.Measurement;

[Measurement("kat", VariableName = "nanokatals", DisplayFactor = 1e9)]
[SiUnit("Katals", 9, "None Milli Micro Nano")]
[Unit("EnzymeUnits", EnzymeUnitsToNanokatals)]
[Product<Volume, CatalyticConcentration>]
public readonly partial struct CatalyticActivity {
    // Enzyme unit (1 U = 1 micromole per minute)
    private const double EnzymeUnitsToNanokatals = 50.0 / 3;
}
