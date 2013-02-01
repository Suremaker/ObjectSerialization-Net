﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectSerialization.Performance.Results
{
    internal abstract class ResultPresenter
    {
        protected ResultPresenter(string ext, IEnumerable<PerformanceResult> results)
        {
            Extension = ext;
            Results = results.ToArray();
            TestCases = Results.Select(r => r.TestCase).Distinct().OrderBy(c => c).ToArray();
            SerializerNames = Results.Select(r => r.SerializerName).Distinct().OrderBy(c => c).ToArray();
        }

        public string Extension { get; private set; }
        public abstract string Present();
        protected string[] SerializerNames { get; private set; }
        protected string[] TestCases { get; private set; }
        protected PerformanceResult[] Results { get; private set; }

        protected IEnumerable<PerformanceResult> GetResultsFor(string testCase)
        {
            return Results.Where(r => r.TestCase == testCase).OrderBy(r => r.SerializerName);
        }

        protected string FormatTimeSpan(double value)
        {
            return string.Format("{0:0.00} ms", new TimeSpan((long)value).TotalMilliseconds);
        }

        protected static string FormatSize(double size)
        {
            return string.Format("{0} b", size);
        }

    }
}
