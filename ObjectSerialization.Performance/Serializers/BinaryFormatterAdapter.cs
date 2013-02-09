using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectSerialization.Performance.Serializers
{
    internal class BinaryFormatterAdapter : ISerializerAdapter
    {
        readonly BinaryFormatter _formatter = new BinaryFormatter();

        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value, out long operationTime)
        {
            using (var stream = new MemoryStream())
            {
                ExecutionTimer.Measure(() => _formatter.Serialize(stream, value), out operationTime);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data, out long operationTime)
        {
            using (var stream = new MemoryStream(data))
                return ExecutionTimer.Measure(() => (T)_formatter.Deserialize(stream), out operationTime);
        }

        public string Name { get { return "BinaryFormatter"; } }

        #endregion
    }
}