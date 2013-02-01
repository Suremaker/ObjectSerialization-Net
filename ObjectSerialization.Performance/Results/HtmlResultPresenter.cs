using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectSerialization.Performance.Results
{
    internal class HtmlResultPresenter : ResultPresenter
    {
        public HtmlResultPresenter(IEnumerable<PerformanceResult> results)
            : base("html", results)
        {
        }

        private static string GetColor(Func<PerformanceResult, double> valueGetter, PerformanceResult result, double min)
        {
            string color = null;
            if (result.Failure != null)
                color = "red";
            else if (Equals(valueGetter(result), min))
                color = "DarkSeaGreen";
            return color;
        }

        public override string Present()
        {
            var sb = new StringBuilder();

            sb.Append("<html colspan=\"5\" rowspan=\"5\"><head><title>Results</title></head><body><table>");
            WriteHeader(sb);
            WriteCases(sb);

            sb.Append("</table></body></html>");
            return sb.ToString();
        }

        private string GetPercent(double value, double max)
        {
            return string.Format("{0:0} %", value * 100 / max);
        }

        private void WriteCase(StringBuilder sb, string testCase, PerformanceResult[] results)
        {
            WriteCase(sb, testCase, "Data size", "Beige", results, c => c.Size, FormatSize);
            WriteCase(sb, testCase, "Serialization time", "Coral", results, c => c.SerializeTime.Total, FormatTimeSpan);
            WriteCase(sb, testCase, "Deserialization time", "LightSeaGreen", results, c => c.DeserializeTime.Total, FormatTimeSpan);
            WriteCase(sb, testCase, "Total time", "RoyalBlue", results, c => c.SerializeTime.Total + c.DeserializeTime.Total, FormatTimeSpan);
        }


        private void WriteCase(StringBuilder sb, string testCase, string category, string categoryColor, PerformanceResult[] results, Func<PerformanceResult, double> valueGetter, Func<double, string> formatter)
        {
            sb.Append("<tr>");
            WriteCell(sb, testCase);
            WriteCell(sb, category, categoryColor);

            double max = results.Where(c => c.Failure == null).Select(valueGetter).Max();
            double min = results.Where(c => c.Failure == null).Select(valueGetter).Min();

            foreach (PerformanceResult result in results)
            {
                string text = result.Failure == null
                    ? formatter(valueGetter(result))
                    : "error";
                string color = GetColor(valueGetter, result, min);
                WriteCell(sb, text, color);
            }

            foreach (PerformanceResult result in results)
            {
                string text = result.Failure == null
                    ? GetPercent(valueGetter(result), max)
                    : "error";
                string color = GetColor(valueGetter, result, min);
                WriteCell(sb, text, color);
            }

            sb.Append("</tr>");
        }

        private void WriteCases(StringBuilder sb)
        {
            foreach (string testCase in TestCases)
                WriteCase(sb, testCase, GetResultsFor(testCase).ToArray());
        }

        private void WriteCell(StringBuilder sb, string text, string color = null)
        {
            if (color == null)
                sb.AppendFormat("<td>{0}</td>", text);
            else
                sb.AppendFormat("<td style=\"background-color: {1}\">{0}</td>", text, color);
        }

        private void WriteHead(StringBuilder sb, string name)
        {
            sb.AppendFormat("<th>{0}</th>", name);
        }

        private void WriteHeader(StringBuilder sb)
        {
            sb.Append("<tr>");
            WriteHead(sb, "Case");
            WriteHead(sb, "Category");
            foreach (string name in SerializerNames)
                WriteHead(sb, name);
            foreach (string name in SerializerNames)
                WriteHead(sb, name + " %");
            sb.Append("</tr>");
        }
    }
}