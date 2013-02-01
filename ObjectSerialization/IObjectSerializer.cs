using System.IO;

namespace ObjectSerialization
{
	public interface IObjectSerializer
	{
		T Deserialize<T>(byte[] serialized);

	    T Deserialize<T>(Stream stream);
	    byte[] Serialize(object value);
	    void Serialize(Stream stream, object value);
	}
}
