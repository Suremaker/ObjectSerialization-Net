using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public string GetResults()
        {
            string[] tests = _results.Select(r => r.TestCase).Distinct().OrderBy(c => c).ToArray();
            var sb = new StringBuilder();

            WriteHeaders(sb);
            foreach (string test in tests)
            {
                WriteCell(sb, test);
                foreach (ISerializer serializer in _serializers)
                    WriteResult(sb, _results.Single(r => r.TestCase == test && r.SerializerName == serializer.Name));
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private static void WriteCell(StringBuilder sb, object value)
        {
            sb.Append(value).Append(';');
        }

        private void WriteResult(StringBuilder sb, PerformanceResult result)
        {
            WriteCell(sb, result.Size.Avg);
            WriteMeasure(sb, result.SerializeTime);
            WriteMeasure(sb, result.DeserializeTime);
        }

        private void WriteMeasure(StringBuilder sb, Measurement measurement)
        {
            WriteCell(sb, measurement.Min);
            WriteCell(sb, measurement.Max);
            WriteCell(sb, measurement.Avg);
        }

        private void WriteHeaders(StringBuilder sb)
        {
            WriteCell(sb, "Test Name");
            foreach (ISerializer serializer in _serializers)
            {
                WriteCell(sb, serializer.Name + " data avg size");
                WriteCell(sb, "Ser. Min Time");
                WriteCell(sb, "Ser. Max Time");
                WriteCell(sb, "Ser. Avg Time");
                WriteCell(sb, "Des. Min Time");
                WriteCell(sb, "Des. Max Time");
                WriteCell(sb, "Des. Avg Time");
            }
            sb.Append("\n");
        }

        private void Store(PerformanceResult measure)
        {
            Console.WriteLine(measure);
            _results.Add(measure);
        }
    }
}