namespace com.hafthor.Measurement;

public sealed class Radioactivity : Measurement<Radioactivity> {

    private Radioactivity(double value) : base(value) { }

    protected override Radioactivity Create(double value) => new(value);
    protected override string Symbol => "Bq";

    // SI units
    public static Radioactivity FromGigabecquerels(double gigabecquerels) => new(gigabecquerels * 1e9);
    public double ToGigabecquerels() => value / 1e9;
    public static Radioactivity FromMegabecquerels(double megabecquerels) => new(megabecquerels * 1e6);
    public double ToMegabecquerels() => value / 1e6;
    public static Radioactivity FromKilobecquerels(double kilobecquerels) => new(kilobecquerels * 1e3);
    public double ToKilobecquerels() => value / 1e3;
    public static Radioactivity FromBecquerels(double value) => new(value);
    public double ToBecquerels() => value;

    // Legacy units
    public static Radioactivity FromCuries(double curies) => new(curies * 3.7e10);
    public double ToCuries() => value / 3.7e10;
    public static Radioactivity FromMillicuries(double millicuries) => new(millicuries * 3.7e7);
    public double ToMillicuries() => value / 3.7e7;
    public static Radioactivity FromMicrocuries(double microcuries) => new(microcuries * 3.7e4);
    public double ToMicrocuries() => value / 3.7e4;
    public static Radioactivity FromRutherfords(double rutherfords) => new(rutherfords * 1e6);
    public double ToRutherfords() => value / 1e6;

}
