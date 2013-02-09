using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ObjectSerialization.Performance.Results
{
    internal class CsvResultPresenter : ResultPresenter
    {
        public CsvResultPresenter(IEnumerable<PerformanceResult> results)
            : base("csv", results) { }

        private static void WriteCell(StringBuilder sb, object value)
        {
            sb.Append(value).Append(';');
        }

        public override string Present(string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine(title);
            WriteHeaders(sb);
            foreach (string testCase in TestCases)
                WriteCase(sb, testCase, GetResultsFor(testCase).ToArray());
            return sb.ToString();
        }

        private void WriteCase(StringBuilder sb, string testCase, PerformanceResult[] results)
        {
            WriteCase(sb, testCase, "Data size", results, c => FormatSize(c.Size));
            WriteCase(sb, testCase, "Serialization time", results, c => FormatTimeSpan(c.SerializeTime.Total));
            WriteCase(sb, testCase, "Deserialization time", results, c => FormatTimeSpan(c.DeserializeTime.Total));
            WriteCase(sb, testCase, "Total time", results, c => FormatTimeSpan(c.SerializeTime.Total + c.DeserializeTime.Total));
        }

        private void WriteCase(StringBuilder sb, string testCase, string category, IEnumerable<PerformanceResult> results, Func<PerformanceResult, string> valueGetter)
        {
            WriteCell(sb, testCase);
            WriteCell(sb, category);
            foreach (PerformanceResult result in results)
                WriteCell(sb, result.Failure == null ? valueGetter(result).ToString(CultureInfo.InvariantCulture) : "error");
            sb.Append("\n");
        }

        private void WriteHeaders(StringBuilder sb)
        {
            WriteCell(sb, "Test Case");
            WriteCell(sb, "Category");
            foreach (string serializer in SerializerNames)
                WriteCell(sb, serializer);
            sb.Append("\n");
        }
    }
}