using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectSerialization.Performance.Serializers
{
    internal class BinaryFormatterAdapter : ISerializerAdapter
    {
        readonly BinaryFormatter _formatter = new BinaryFormatter();

        #region ISerializerAdapter Members

        public string Name { get { return "BinaryFormatter"; } }
        public T Deserialize<T>(Stream stream, out long operationTime)
        {
            return ExecutionTimer.Measure(() => (T)_formatter.Deserialize(stream), out operationTime);
        }

        public void Serialize<T>(Stream stream, T value, out long operationTime)
        {
            ExecutionTimer.Measure(() => _formatter.Serialize(stream, value), out operationTime);
        }

        #endregion
    }
}