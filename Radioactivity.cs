namespace com.hafthor.Measurement;

public class Radioactivity {
    private readonly double becquerels;

    private Radioactivity(double becquerels) => this.becquerels = becquerels;

    // Arithmetic
    public static Radioactivity operator +(Radioactivity a, Radioactivity b) => new(a.becquerels + b.becquerels);
    public static Radioactivity operator -(Radioactivity a, Radioactivity b) => new(a.becquerels - b.becquerels);
    public static Radioactivity operator -(Radioactivity x) => new(-x.becquerels);

    // SI units
    public static Radioactivity FromGigabecquerels(double gigabecquerels) => new(gigabecquerels * 1e9);
    public double ToGigabecquerels() => becquerels / 1e9;
    public static Radioactivity FromMegabecquerels(double megabecquerels) => new(megabecquerels * 1e6);
    public double ToMegabecquerels() => becquerels / 1e6;
    public static Radioactivity FromKilobecquerels(double kilobecquerels) => new(kilobecquerels * 1e3);
    public double ToKilobecquerels() => becquerels / 1e3;
    public static Radioactivity FromBecquerels(double becquerels) => new(becquerels);
    public double ToBecquerels() => becquerels;

    // Legacy units
    public static Radioactivity FromCuries(double curies) => new(curies * 3.7e10);
    public double ToCuries() => becquerels / 3.7e10;
    public static Radioactivity FromMillicuries(double millicuries) => new(millicuries * 3.7e7);
    public double ToMillicuries() => becquerels / 3.7e7;
    public static Radioactivity FromMicrocuries(double microcuries) => new(microcuries * 3.7e4);
    public double ToMicrocuries() => becquerels / 3.7e4;
    public static Radioactivity FromRutherfords(double rutherfords) => new(rutherfords * 1e6);
    public double ToRutherfords() => becquerels / 1e6;

    public override string ToString() => $"{becquerels} Bq";

    public override bool Equals(object obj) => obj is Radioactivity other && other.becquerels == becquerels;
    public override int GetHashCode() => becquerels.GetHashCode();
}
