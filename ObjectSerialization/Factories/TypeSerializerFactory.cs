using System;
using System.IO;
using ObjectSerialization.Types;

namespace ObjectSerialization.Factories
{
    internal class TypeSerializerFactory
    {
        public static Func<BinaryReader, object> GetDeserializer(string typeFullName)
        {
			return TypeInfoRepository.GetTypeInfo(typeFullName).Deserializer;
        }

        public static Func<BinaryReader, object> GetDeserializer(Type type)
        {
			return TypeInfoRepository.GetTypeInfo(type).Deserializer;
        }

        public static Action<BinaryWriter, object> GetSerializer(string typeFullName)
        {
            return TypeInfoRepository.GetTypeInfo(typeFullName).Serializer;
        }

        public static Action<BinaryWriter, object> GetSerializer(Type type)
        {
            return TypeInfoRepository.GetTypeInfo(type).Serializer;
        }
    }
}