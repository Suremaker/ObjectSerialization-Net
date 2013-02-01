using System;
using System.IO;
using ObjectSerialization.Builders;

namespace ObjectSerialization.Factories
{
    internal class ClassMembersSerializerFactory : GenericSerializerFactory
    {
        public static Func<BinaryReader, object> GetDeserializer(Type type)
        {
            return GetDeserializer(typeof(ClassMembersSerializerBuilder<>), type);
        }

        public static Action<BinaryWriter, object> GetSerializer(Type type)
        {
            return GetSerializer(typeof(ClassMembersSerializerBuilder<>), type);
        }
    }
}