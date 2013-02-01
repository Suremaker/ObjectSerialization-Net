namespace ObjectSerialization.Performance.Serializers
{
    internal interface ISerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] data);
        string Name { get; }
    }
}