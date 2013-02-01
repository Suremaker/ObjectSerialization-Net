using System.IO;
using ProtoBuf;

namespace ObjectSerialization.Performance.Serializers
{
    internal class ProtoBufAdapter : ISerializerAdapter
    {
        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return Serializer.Deserialize<T>(stream);
        }

        public string Name { get { return "ProtoBuf"; } }

        #endregion
    }
}