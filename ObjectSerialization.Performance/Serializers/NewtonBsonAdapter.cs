using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ObjectSerialization.Performance.Serializers
{
    internal class NewtonBsonAdapter : ISerializerAdapter
    {
        readonly JsonSerializer _serializer = new JsonSerializer();
        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BsonWriter(stream))
            {
                _serializer.Serialize(writer, value);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BsonReader(stream))
                return _serializer.Deserialize<T>(reader);
        }

        public string Name { get { return "NewtonBSON"; } }

        #endregion
    }
}