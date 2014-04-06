using System;
using System.Collections;
using System.IO;
using System.Linq;
using ObjectSerialization.Performance.Serializers;

namespace ObjectSerialization.Performance.TestCases
{
    abstract class TestCase
    {
        private const int _measurementCount = 500000;
        public abstract string Name { get; }
        public PerformanceResult Measure(ISerializerAdapter serializer)
        {
            var result = new PerformanceResult(Name, serializer.Name, _measurementCount);
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

        protected abstract object Deserialize(ISerializerAdapter serializer, Stream stream, out long operationTime);
        protected abstract object GetValue();
        protected abstract void Serialize(ISerializerAdapter serializer, Stream stream, out long operationTime);

        private void Measure(ISerializerAdapter serializer, PerformanceResult result)
        {
            GC.Collect();
            using (var stream = new MemoryStream())
            {
                ValidateSerializer(serializer, stream, result);

                for (int i = 0; i < _measurementCount; ++i)
                {
                    long operationTime;
                    Serialize(serializer, stream, out operationTime);
                    result.SerializeTime.Add(operationTime);

                    Deserialize(serializer, stream, out operationTime);
                    result.DeserializeTime.Add(operationTime);
                }
            }
        }

        private void ValidateSerializer(ISerializerAdapter serializer, MemoryStream stream, PerformanceResult result)
        {
            long operationTime;
            Serialize(serializer, stream, out operationTime);
            result.Size = stream.Length;
            object deserialized = Deserialize(serializer, stream, out operationTime);
            var deserializedEnumerable = deserialized as IEnumerable;

            if (deserializedEnumerable != null)
            {
                if (!deserializedEnumerable.OfType<object>().SequenceEqual(((IEnumerable)GetValue()).OfType<object>()))
                    throw new Exception("Deserialized object does not equal expected one");
            }
            else if (!Equals(deserialized, GetValue()))
                throw new Exception("Deserialized object does not equal expected one");

        }
    }
}