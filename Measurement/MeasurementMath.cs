namespace com.hafthor.Measurement;

// Shared numeric helpers used by the generated measurement structs, so the per-type generated
// code stays a thin delegation rather than duplicating each algorithm.
internal static class MeasurementMath {

    // Approximate equality within a number of representable steps (ULPs). Relies on the
    // IEEE-754 property that reinterpreting a double's bits as a sign-magnitude integer makes
    // adjacent representable values adjacent integers — including across exponent boundaries
    // (0.9999… → 1.0) and the subnormal/normal boundary — so the integer distance is the exact
    // count of representable doubles between a and b.
    public static bool NearlyEqual(double a, double b, int ulps) {
        if (a == b) return true;                                  // handles +0.0 == -0.0
        if (double.IsNaN(a) || double.IsNaN(b)) return false;
        if (double.IsInfinity(a) || double.IsInfinity(b)) return false;
        long ai = BitConverter.DoubleToInt64Bits(a);
        long bi = BitConverter.DoubleToInt64Bits(b);
        if ((ai < 0) != (bi < 0)) return false;                  // opposite signs (straddle 0)
        return Math.Abs(ai - bi) <= ulps;
    }
}
