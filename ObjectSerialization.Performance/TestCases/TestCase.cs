using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using ObjectSerialization.Performance.Serializers;

namespace ObjectSerialization.Performance.TestCases
{
    abstract class TestCase
    {
        private const int _measurementCount = 10000;
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

        protected abstract object Deserialize(ISerializerAdapter serializer, byte[] data, out long operationTime);
        protected abstract object GetValue();
        protected abstract byte[] Serialize(ISerializerAdapter serializer, out long operationTime);

        private void Measure(ISerializerAdapter serializer, PerformanceResult result)
        {
            ValidateSerializer(serializer, result);

            for (int i = 0; i < _measurementCount; ++i)
            {
                long operationTime;
                byte[] data = Serialize(serializer, out operationTime);
                result.SerializeTime.Add(operationTime);

                Deserialize(serializer, data, out operationTime);
                result.DeserializeTime.Add(operationTime);
            }
        }

        private void ValidateSerializer(ISerializerAdapter serializer, PerformanceResult result)
        {
            long operationTime;
            byte[] serializedData = Serialize(serializer, out operationTime);
            result.Size = serializedData.Length;
            object deserialized = Deserialize(serializer, serializedData, out operationTime);
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