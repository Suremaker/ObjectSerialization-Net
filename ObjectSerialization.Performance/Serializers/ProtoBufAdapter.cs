using System.IO;
using ProtoBuf;

namespace ObjectSerialization.Performance.Serializers
{
    internal class ProtoBufAdapter : ISerializerAdapter
    {
        #region ISerializerAdapter Members

        public string Name { get { return "ProtoBuf 3.0"; } }
        public T Deserialize<T>(Stream stream, out long operationTime)
        {
            return ExecutionTimer.Measure(() => Serializer.Deserialize<T>(stream), out operationTime);
        }

        public void Serialize<T>(Stream stream, T value, out long operationTime)
        {
            ExecutionTimer.Measure(() => Serializer.Serialize(stream, value), out operationTime);
        }

        #endregion
    }
}