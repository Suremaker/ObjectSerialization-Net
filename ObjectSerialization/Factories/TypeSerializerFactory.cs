using System;
using System.IO;
using ObjectSerialization.Builders;
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

        public static Func<BinaryReader, T> GetDeserializer<T>()
        {
            return TypeSerializerBuilder<T>.DeserializeFn;
        }

        public static Action<BinaryWriter, object> GetSerializer(string typeFullName)
        {
            return TypeInfoRepository.GetTypeInfo(typeFullName).Serializer;
        }

        public static Action<BinaryWriter, object> GetSerializer(Type type)
        {
            return TypeInfoRepository.GetTypeInfo(type).Serializer;
        }

        public static Action<BinaryWriter, T> GetSerializer<T>()
        {
            return TypeSerializerBuilder<T>.SerializeFn;
        }
    }
}