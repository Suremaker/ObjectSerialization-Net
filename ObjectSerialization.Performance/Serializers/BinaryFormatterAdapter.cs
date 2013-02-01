using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectSerialization.Performance.Serializers
{
    internal class BinaryFormatterAdapter : ISerializerAdapter
    {
        readonly BinaryFormatter _formatter = new BinaryFormatter();
        
        public byte[] Serialize<T>(T value)
        {
            using (var stream = new MemoryStream())
            {
                _formatter.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return (T)_formatter.Deserialize(stream);
        }

        public string Name { get { return "BinaryFormatter"; } }
    }
}