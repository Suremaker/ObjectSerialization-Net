using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using ObjectSerialization.Builders;
using ObjectSerialization.Types;

namespace ObjectSerialization.Factories
{
    internal class TypeSerializerFactory
    {        
        public static Action<BinaryWriter, object> GetSerializer(string typeFullName)
        {
            return TypeInfoRepository.GetTypeInfo(typeFullName).Serializer;
        }

        public static Func<BinaryReader, object> GetDeserializer(string typeFullName)
        {
			return TypeInfoRepository.GetTypeInfo(typeFullName).Deserializer;
        }

        public static Action<BinaryWriter, object> GetSerializer(Type type)
        {
			return TypeInfoRepository.GetTypeInfo(type).Serializer;
        }

        public static Func<BinaryReader, object> GetDeserializer(Type type)
        {
			return TypeInfoRepository.GetTypeInfo(type).Deserializer;
        }        
    }
}