using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using ObjectSerialization.Performance.Serializers;

namespace ObjectSerialization.Performance.TestCases
{
    abstract class TestCase
    {
        public abstract string Name { get; }
        public PerformanceResult Measure(ISerializerAdapter serializer)
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

        private void Measure(ISerializerAdapter serializer, PerformanceResult result)
        {
            ValidateSerializer(serializer, result);

            var watch = new Stopwatch();

            for (int i = 0; i < 10000; ++i)
            {
                watch.Start();
                byte[] data = Serialize(serializer);
                watch.Stop();
                result.SerializeTime.Add(watch.ElapsedTicks);

                watch.Restart();
                Deserialize(serializer, data);
                watch.Stop();
                result.DeserializeTime.Add(watch.ElapsedTicks);

            }
        }

        private void ValidateSerializer(ISerializerAdapter serializer, PerformanceResult result)
        {
            var serializedData = Serialize(serializer);
            result.Size = serializedData.Length;
            object deserialized = Deserialize(serializer, serializedData);
            var deserializedEnumerable = deserialized as IEnumerable;

            if (deserializedEnumerable != null)
            {
                if (!deserializedEnumerable.OfType<object>().SequenceEqual(((IEnumerable)GetValue()).OfType<object>()))
                    throw new Exception("Deserialized object does not equal expected one");
            }
            else if (!Equals(deserialized, GetValue()))
                throw new Exception("Deserialized object does not equal expected one");
        }

        protected abstract object GetValue();
        protected abstract byte[] Serialize(ISerializerAdapter serializer);
        protected abstract object Deserialize(ISerializerAdapter serializer, byte[] data);
    }
}