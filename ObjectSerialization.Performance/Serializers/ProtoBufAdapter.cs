using System.IO;
using ProtoBuf;

namespace ObjectSerialization.Performance.Serializers
{
    internal class ProtoBufAdapter : ISerializerAdapter
    {
        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value, out long operationTime)
        {
            using (var stream = new MemoryStream())
            {
                ExecutionTimer.Measure(() => Serializer.Serialize(stream, value), out operationTime);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data, out long operationTime)
        {
            using (var stream = new MemoryStream(data))
                return ExecutionTimer.Measure(() => Serializer.Deserialize<T>(stream), out operationTime);
        }

        public string Name { get { return "ProtoBuf"; } }

        #endregion
    }
}