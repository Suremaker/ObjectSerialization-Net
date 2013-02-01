using System;
using System.IO;
using System.Text;

namespace ObjectSerialization
{
    public class ObjectSerializer : IObjectSerializer
    {
        #region IObjectSerializer Members

        public byte[] Serialize(object value)
        {
            return SerializeAs(value, value != null ? value.GetType() : typeof(object));
        }

        public T Deserialize<T>(byte[] serialized)
        {
            return (T)Deserialize(serialized);
        }

        public byte[] SerializeAs(object value, Type type)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(type.FullName);
                TypeSerializer.GetSerializer(type).Invoke(writer, value);
                writer.Flush();
                return stream.ToArray();
            }
        }

        public object Deserialize(byte[] serialized)
        {
            using (var stream = new MemoryStream(serialized))
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                string type = reader.ReadString();
                return TypeSerializer.GetDeserializer(TypeSerializer.LoadType(type)).Invoke(reader);
            }
        }

        #endregion        
    }
}