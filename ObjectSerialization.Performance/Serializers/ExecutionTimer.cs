using System;
using System.Diagnostics;

namespace ObjectSerialization.Performance.Serializers
{
    static class ExecutionTimer
    {
        private static readonly Stopwatch _stopwatch = new Stopwatch();
        public static T Measure<T>(Func<T> fun, out long operationTime)
        {
            _stopwatch.Restart();
            var result = fun();
            _stopwatch.Stop();
            operationTime = _stopwatch.ElapsedTicks;
            return result;
        }

        public static void Measure(Action fun, out long operationTime)
        {
            _stopwatch.Restart();
            fun();
            _stopwatch.Stop();
            operationTime = _stopwatch.ElapsedTicks;
        }
    }
}
