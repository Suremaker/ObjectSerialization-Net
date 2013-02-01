using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using ObjectSerialization.Builders;

namespace ObjectSerialization.Factories
{
    internal class TypeSerializerFactory
    {
        class TypeInfo
        {
            public Func<BinaryReader, object> Deserializer;
            public Action<BinaryWriter, object> Serializer;
        }

        private static readonly ConcurrentDictionary<string, TypeInfo> _typeDictionary = new ConcurrentDictionary<string, TypeInfo>();

        public static Action<BinaryWriter, object> GetSerializer(string typeFullName)
        {
            return GetTypeInfo(typeFullName).Serializer;
        }

        public static Func<BinaryReader, object> GetDeserializer(string typeFullName)
        {
            return GetTypeInfo(typeFullName).Deserializer;
        }

        public static Action<BinaryWriter, object> GetSerializer(Type type)
        {
            return GetTypeInfo(type).Serializer;
        }

        public static Func<BinaryReader, object> GetDeserializer(Type type)
        {
            return GetTypeInfo(type).Deserializer;
        }

        private static TypeInfo GetTypeInfo(string typeFullName)
        {
            TypeInfo info;
            if (_typeDictionary.TryGetValue(typeFullName, out info))
                return info;

            info = LoadTypeInfo(typeFullName);
            _typeDictionary.TryAdd(typeFullName, info);
            return info;
        }

        private static TypeInfo GetTypeInfo(Type type)
        {
            TypeInfo info;
            if (_typeDictionary.TryGetValue(type.FullName, out info))
                return info;

            info = LoadTypeInfo(type);
            _typeDictionary.TryAdd(type.FullName, info);
            return info;
        }

        private static Type FindType(string type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(type, false)).First(t => t != null);
        }

        private static TypeInfo LoadTypeInfo(string typeFullName)
        {
            return LoadTypeInfo(FindType(typeFullName));
        }

        private static TypeInfo LoadTypeInfo(Type type)
        {
            Type builderType = typeof(TypeSerializerBuilder<>).MakeGenericType(type);
            return new TypeInfo
            {
                Serializer = (Action<BinaryWriter, object>)builderType.GetProperty("SerializeFn", BindingFlags.Static | BindingFlags.Public).GetValue(null, null),
                Deserializer = (Func<BinaryReader, object>)builderType.GetProperty("DeserializeFn", BindingFlags.Static | BindingFlags.Public).GetValue(null, null)
            };
        }
    }
}