using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectSerialization.Performance.Results
{
    internal class TextResultPresenter : ResultPresenter
    {
        public TextResultPresenter(IEnumerable<PerformanceResult> results)
            : base("txt", results) { }

        public override string Present(string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine(title);
            sb.AppendLine();
            foreach (var result in TestCases.SelectMany(testCase => GetResultsFor(testCase).ToArray()))
                sb.AppendLine(result.ToString());
            return sb.ToString();
        }
    }
}