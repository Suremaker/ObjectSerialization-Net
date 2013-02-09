namespace ObjectSerialization.Performance.Serializers
{
    internal interface ISerializerAdapter
    {
        string Name { get; }
        T Deserialize<T>(byte[] data, out long operationTime);
        byte[] Serialize<T>(T value, out long operationTime);
    }
}