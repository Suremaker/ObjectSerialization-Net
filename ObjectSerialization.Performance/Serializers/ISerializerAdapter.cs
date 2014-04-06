using System.IO;

namespace ObjectSerialization.Performance.Serializers
{
    internal interface ISerializerAdapter
    {
        string Name { get; }
        T Deserialize<T>(Stream stream, out long operationTime);
        void Serialize<T>(Stream stream,T value, out long operationTime);
    }
}