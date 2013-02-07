using System.IO;
using System.Runtime.Serialization;

namespace ObjectSerialization.Performance.Serializers
{
    internal class DataContractSerializerAdapter : ISerializerAdapter
    {
        #region ISerializerAdapter Members

        public byte[] Serialize<T>(T value)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream(data))
                return (T)serializer.ReadObject(stream);
        }

        public string Name { get { return "DataContractSerializer"; } }

        #endregion
    }
}