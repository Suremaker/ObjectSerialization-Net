namespace ObjectSerialization.Performance.Serializers
{
    internal interface ISerializerAdapter
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] data);
        string Name { get; }
    }
}