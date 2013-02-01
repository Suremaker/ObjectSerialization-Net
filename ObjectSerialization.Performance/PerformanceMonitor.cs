using System;
using System.Collections.Generic;
using ObjectSerialization.Performance.Serializers;
using ObjectSerialization.Performance.TestCases;

namespace ObjectSerialization.Performance
{
    internal class PerformanceMonitor
    {
        private readonly ISerializer[] _serializers;
        private readonly List<PerformanceResult> _results = new List<PerformanceResult>();

        public PerformanceMonitor(params ISerializer[] serializers)
        {
            _serializers = serializers;
        }

        public void MeasureFor(TestCase testCase)
        {
            foreach (ISerializer serializer in _serializers)
                Store(testCase.Measure(serializer));
        }

        

        private void Store(PerformanceResult measure)
        {
            Console.WriteLine(measure);
            _results.Add(measure);
        }

        public IEnumerable<PerformanceResult> GetResults()
        {
            return _results;
        }
    }
}