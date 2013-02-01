using System.IO;
using System.Text;
using ObjectSerialization.Factories;

namespace ObjectSerialization
{
	public class ObjectSerializer : IObjectSerializer
	{
		#region IObjectSerializer Members

		public byte[] Serialize(object value)
		{
			using (var stream = new MemoryStream())
			{
				Serialize(stream, value);
				return stream.ToArray();
			}
		}

		public void Serialize(Stream stream, object value)
		{
			var type = value != null ? value.GetType() : typeof(object);
			var writer = new BinaryWriter(stream, Encoding.UTF8);
			writer.Write(type.FullName);
			TypeSerializerFactory.GetSerializer(type).Invoke(writer, value);
			writer.Flush();
		}

		public T Deserialize<T>(byte[] serialized)
		{
			using (var stream = new MemoryStream(serialized))
				return Deserialize<T>(stream);
		}

		public T Deserialize<T>(Stream stream)
		{
			var reader = new BinaryReader(stream, Encoding.UTF8);
			string typeFullName = reader.ReadString();
			return (T)TypeSerializerFactory.GetDeserializer(typeFullName).Invoke(reader);
		}

		#endregion
	}
}