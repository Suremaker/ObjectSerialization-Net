using System;
using System.IO;
using System.Text;
using ObjectSerialization.Types;

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
			Type type = (value != null) ? value.GetType() : typeof(object);

			var writer = new BinaryWriter(stream, Encoding.UTF8);
			
			TypeInfo typeInfo = TypeInfoWriter.WriteInfo(writer, type);
			typeInfo.Serializer.Invoke(writer, value);
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
			TypeInfo typeInfo = TypeInfoWriter.ReadInfo(reader);
			return (T)typeInfo.Deserializer.Invoke(reader);
		}

	    #endregion
	}
}