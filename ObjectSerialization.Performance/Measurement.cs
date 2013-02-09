using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ObjectSerialization.Performance
{
    internal class Measurement
    {
        private readonly List<long> _values;

        public double Avg { get { return CalculateAvg(); } }

        public int Count { get { return _values.Count; } }
        public long Max { get { return _values.DefaultIfEmpty().Max(); } }
        public long Min { get { return _values.DefaultIfEmpty().Min(); } }
        public long Total { get { return _values.Sum(); } }

        public Measurement(int measurementCount)
        {
            _values = new List<long>(measurementCount);
        }

        public void Add(long value)
        {
            _values.Add(value);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Min:{0}, Max:{1}, Avg:{2:0.000}, Total: {3}", Min, Max, Avg, Total);
        }

        private double CalculateAvg()
        {
            int valuesCountToSkip = Math.Max(Count / 100, 1);
            return _values.OrderBy(v => v)
                          .Skip(valuesCountToSkip)
                          .Take(Count - 2 * valuesCountToSkip)
                          .Average(v => (double)v);
        }
    }
}