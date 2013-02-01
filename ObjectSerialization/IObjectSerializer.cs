using System;

namespace ObjectSerialization
{
    public interface IObjectSerializer
    {
        object Deserialize(byte[] serialized);
        T Deserialize<T>(byte[] serialized);
        byte[] Serialize(object value);
        byte[] SerializeAs(object value, Type type);
    }
}
