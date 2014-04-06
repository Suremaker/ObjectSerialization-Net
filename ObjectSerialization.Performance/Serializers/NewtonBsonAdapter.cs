using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ObjectSerialization.Performance.Serializers
{
    internal class NewtonBsonAdapter : ISerializerAdapter
    {
        readonly JsonSerializer _serializer = new JsonSerializer();

        #region ISerializerAdapter Members

        public string Name { get { return "NewtonBSON"; } }
        public T Deserialize<T>(Stream stream, out long operationTime)
        {
            using (var reader = new BsonReader(stream) { CloseInput = false })
                return ExecutionTimer.Measure(() => _serializer.Deserialize<T>(reader), out operationTime);
        }

        public void Serialize<T>(Stream stream, T value, out long operationTime)
        {
            using (var writer = new BsonWriter(stream) { CloseOutput = false })
            {
                ExecutionTimer.Measure(() => _serializer.Serialize(writer, value), out operationTime);
            }
        }

        #endregion
    }
}