using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ObjectSerialization.Performance
{
    internal class Measurement
    {
        private List<double> _values;
        public double Avg { get; private set; }
        public int Count { get; private set; }
        public double Max { get; private set; }
        public double Min { get; private set; }
        public double Total { get; private set; }

        public Measurement(int measurementCount)
        {
            _values = new List<double>(measurementCount);
            Min = double.MaxValue;
        }

        public void Add(double value)
        {
            if (_values == null)
                throw new InvalidOperationException("Values are already compacted");
            _values.Add(value);
            ++Count;
            Max = Math.Max(Max, value);
            Min = Math.Min(Min, value);
            Total += value;
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
                          .Average();
        }

        public void Compact()
        {
            Avg = CalculateAvg();
            _values = null;
        }
    }
}