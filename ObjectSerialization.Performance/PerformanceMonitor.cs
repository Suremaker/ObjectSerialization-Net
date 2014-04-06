using System;
using System.Collections.Generic;
using System.Linq;
using ObjectSerialization.Performance.Serializers;
using ObjectSerialization.Performance.TestCases;

namespace ObjectSerialization.Performance
{
    internal class PerformanceMonitor
    {
        private readonly List<PerformanceResult> _results = new List<PerformanceResult>();
        private readonly ISerializerAdapter[] _serializers;

        public PerformanceMonitor(params ISerializerAdapter[] serializers)
        {
            _serializers = serializers;
        }

        public IEnumerable<PerformanceResult> GetResults()
        {
            return _results;
        }

        public void MeasureFor(TestCase testCase)
        {
            foreach (ISerializerAdapter serializer in _serializers)
                Store(testCase.Measure(serializer));
        }

        private void Store(PerformanceResult measure)
        {
            Console.WriteLine(measure);
            _results.Add(measure);
        }

        public void Summarize()
        {
            var summaries = _results.GroupBy(r => r.SerializerName).OrderBy(g => g.Key).Select(CreateSummary);
            foreach (var summary in summaries)
            {
                Console.WriteLine(summary);
                _results.Add(summary);
            }
        }

        private PerformanceResult CreateSummary(IGrouping<string, PerformanceResult> group)
        {
            var performanceResult = new PerformanceResult("Summary", group.Key, group.Count());
            performanceResult.Size = group.Sum(g => g.Size);
            foreach (var value in group.Select(g => g.SerializeTime).Select(s => s.Avg))
                performanceResult.SerializeTime.Add(value);
            foreach (var value in group.Select(g => g.DeserializeTime).Select(s => s.Avg))
                performanceResult.DeserializeTime.Add(value);
            return performanceResult;
        }
    }
}