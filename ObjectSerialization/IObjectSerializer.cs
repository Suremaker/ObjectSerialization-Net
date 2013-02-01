namespace ObjectSerialization
{
	public interface IObjectSerializer
	{
		T Deserialize<T>(byte[] serialized);
		byte[] Serialize(object value);
	}
}
