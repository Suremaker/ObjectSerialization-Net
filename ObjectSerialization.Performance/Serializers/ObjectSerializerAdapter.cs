using System.IO;

namespace ObjectSerialization.Performance.Serializers
{
    internal class ObjectSerializerAdapter : ISerializerAdapter
    {
        private readonly IObjectSerializer _serializer = new ObjectSerializer();

        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value, out long operationTime)
        {
            using (var stream = new MemoryStream())
            {
                ExecutionTimer.Measure(() => _serializer.Serialize(stream, value), out operationTime);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data, out long operationTime)
        {
            using (var stream = new MemoryStream(data))
                return ExecutionTimer.Measure(() => _serializer.Deserialize<T>(stream), out operationTime);
        }

        public string Name { get { return "ObjectSerializer"; } }

        #endregion
    }
}