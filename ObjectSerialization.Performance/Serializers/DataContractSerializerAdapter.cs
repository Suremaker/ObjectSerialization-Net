using System.IO;
using System.Runtime.Serialization;

namespace ObjectSerialization.Performance.Serializers
{
    internal class DataContractSerializerAdapter : ISerializerAdapter
    {
        #region ISerializerAdapter Members

        public string Name { get { return "DataContractSerializer"; } }
        public T Deserialize<T>(Stream stream, out long operationTime)
        {
            var serializer = new DataContractSerializer(typeof(T));
            return ExecutionTimer.Measure(() => (T)serializer.ReadObject(stream), out operationTime);
        }

        public void Serialize<T>(Stream stream, T value, out long operationTime)
        {
            var serializer = new DataContractSerializer(typeof(T));
            ExecutionTimer.Measure(() => serializer.WriteObject(stream, value), out operationTime);
        }

        #endregion
    }
}