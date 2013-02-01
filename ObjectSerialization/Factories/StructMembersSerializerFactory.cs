using System;
using System.IO;
using ObjectSerialization.Builders;

namespace ObjectSerialization.Factories
{
    internal class StructMembersSerializerFactory : GenericSerializerFactory
    {
        public static Func<BinaryReader, object> GetDeserializer(Type type)
        {
            return GetDeserializer(typeof(StructMembersSerializerBuilder<>), type);
        }

        public static Action<BinaryWriter, object> GetSerializer(Type type)
        {
            return GetSerializer(typeof(StructMembersSerializerBuilder<>), type);
        }
    }
}