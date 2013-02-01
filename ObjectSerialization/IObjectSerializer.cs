using System.IO;

namespace ObjectSerialization
{
	public interface IObjectSerializer
	{
		T Deserialize<T>(byte[] serialized);
		byte[] Serialize(object value);

		T Deserialize<T>(Stream stream);
		void Serialize(Stream stream, object value);
	}
}
