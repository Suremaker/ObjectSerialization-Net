using System.IO;
using System.Runtime.Serialization;

namespace ObjectSerialization.Performance.Serializers
{
    internal class DataContractSerializerAdapter : ISerializerAdapter
    {
        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value, out long operationTime)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                ExecutionTimer.Measure(() => serializer.WriteObject(stream, value), out operationTime);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data, out long operationTime)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream(data))
                return ExecutionTimer.Measure(() => (T)serializer.ReadObject(stream), out operationTime);
        }

        public string Name { get { return "DataContractSerializer"; } }

        #endregion
    }
}