namespace com.hafthor.Measurement;

public static class IEnumerableExtensions {
    extension<T>(IEnumerable<T> source) where T : struct, IMeasurement<T> {
        public T Sum() => source.Accumulate().sum;

        public T Average() {
            (T sum, long count) = source.Accumulate();
            if (count == 0) _ = Array.Empty<double>().Average(); // throws
            return sum / count;
        }

        private (T sum, long count) Accumulate() {
            ArgumentNullException.ThrowIfNull(source);

            T sum = T.AdditiveIdentity;
            long count = 0;
            foreach (T i in source) {
                sum += i;
                count++;
            }

            return (sum, count);
        }
    }

    extension<T>(IEnumerable<T?> source) where T : struct, IMeasurement<T> {
        public T? Sum() {
            (T sum, long count) = source.Accumulate();
            return count > 0 ? sum : null;
        }

        public T? Average() {
            (T sum, long count) = source.Accumulate();
            return count > 0 ? sum / count : null;
        }

        private (T sum, long count) Accumulate() {
            ArgumentNullException.ThrowIfNull(source);

            T sum = T.AdditiveIdentity;
            long count = 0;
            foreach (T? i in source)
                if (i.HasValue) {
                    sum += i.Value;
                    count++;
                }

            return (sum, count);
        }
    }
}
