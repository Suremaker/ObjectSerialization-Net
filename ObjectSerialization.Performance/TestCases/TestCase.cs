using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ObjectSerialization.Performance.Serializers;

namespace ObjectSerialization.Performance.TestCases
{
    abstract class TestCase
    {
        public abstract string Name { get; }
        public PerformanceResult Measure(ISerializer serializer)
        {
            var result = new PerformanceResult(Name, serializer.Name);
            try
            {
                Measure(serializer, result);
            }
            catch (Exception e)
            {
                result.Failure = e.Message;
            }
            return result;
        }

        private void Measure(ISerializer serializer, PerformanceResult result)
        {
            ValidateSerializer(serializer);

            var watch = new Stopwatch();

            for (int i = 0; i < 10000; ++i)
            {
                watch.Start();
                var data = Serialize(serializer);
                watch.Stop();
                result.SerializeTime.Add(watch.ElapsedTicks);

                watch.Restart();
                Deserialize(serializer, data);
                watch.Stop();
                result.DeserializeTime.Add(watch.ElapsedTicks);

                result.Size.Add(data.Length);
            }
        }

        private void ValidateSerializer(ISerializer serializer)
        {
            var deserialized = Deserialize(serializer, Serialize(serializer));
            var deserializedEnumerable = deserialized as IEnumerable<object>;

            if (deserializedEnumerable != null && !deserializedEnumerable.SequenceEqual((IEnumerable<object>)GetValue()) && !Equals(deserialized, GetValue()))
                throw new Exception("Deserialized object does not equal expected one");
        }

        protected abstract object GetValue();
        protected abstract byte[] Serialize(ISerializer serializer);
        protected abstract object Deserialize(ISerializer serializer, byte[] data);
    }
}