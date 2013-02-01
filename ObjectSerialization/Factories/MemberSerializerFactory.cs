using System;
using System.IO;
using ObjectSerialization.Builders;

namespace ObjectSerialization.Factories
{
    internal class MemberSerializerFactory:GenericSerializerFactory
    {
        public static Func<BinaryReader, object> GetDeserializer(Type type)
        {
            return GetDeserializer(typeof(MemberSerializerBuilder<>), type);
        }

        public static Action<BinaryWriter, object> GetSerializer(Type type)
        {
            return GetSerializer(typeof(MemberSerializerBuilder<>), type);
        }
    }
}