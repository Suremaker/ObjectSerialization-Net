namespace ObjectSerialization.Performance.Serializers
{
    internal interface ISerializerAdapter
    {
        string Name { get; }
        T Deserialize<T>(byte[] data);
        byte[] Serialize<T>(T value);
    }
}