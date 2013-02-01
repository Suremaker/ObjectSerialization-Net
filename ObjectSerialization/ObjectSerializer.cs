using System;
using System.IO;
using System.Linq;

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
            using (var writer = new BinaryWriter(stream))
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
            using (var reader = new BinaryReader(stream))
            {
                string type = reader.ReadString();
                return TypeSerializer.GetDeserializer(LoadType(type)).Invoke(reader);
            }
        }

        #endregion

        private static Type LoadType(string type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(type, false)).First(t => t != null);
        }
    }
}