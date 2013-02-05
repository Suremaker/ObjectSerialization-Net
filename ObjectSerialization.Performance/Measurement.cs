using System;

namespace ObjectSerialization.Performance
{
    internal class Measurement
    {
        public double Avg { get { return Count > 0 ? (Total / (double)Count) : 0; } }
        public int Count { get; private set; }
        public long Max { get; private set; }
        public long Min { get; private set; }
        public long Total { get; private set; }

        public void Add(long value)
        {
            Total += value;
            ++Count;
            Min = Min > 0 ? Math.Min(value, Min) : value;
            Max = Math.Max(value, Max);
        }

        public override string ToString()
        {
            return string.Format("Min:{0}, Max:{1}, Avg:{2}, Total: {3}", Min, Max, Avg, Total);
        }
    }
}