using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ObjectSerialization.Performance.Serializers
{
    internal class NewtonBsonAdapter : ISerializerAdapter
    {
        readonly JsonSerializer _serializer = new JsonSerializer();
        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value, out long operationTime)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BsonWriter(stream))
            {
                ExecutionTimer.Measure(() => _serializer.Serialize(writer, value), out operationTime);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data, out long operationTime)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BsonReader(stream))
                return ExecutionTimer.Measure(() => _serializer.Deserialize<T>(reader), out operationTime);
        }

        public string Name { get { return "NewtonBSON"; } }

        #endregion
    }
}