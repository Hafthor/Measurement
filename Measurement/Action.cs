namespace com.hafthor.Measurement;

[Measurement("J·s", VariableName = "nanoJouleSeconds", DisplayFactor = 1e9)]
[SiUnit("JouleSeconds", 9)]
[SiUnit("ErgSeconds", 2)]
[Unit("PlanckConstants", 6.62607015e-25)]
[Product<Energy, Duration>(Primary = true)]
[Product<Length, Momentum>]
public readonly partial struct Action { }
