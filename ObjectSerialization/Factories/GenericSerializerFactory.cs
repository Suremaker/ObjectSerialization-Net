using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ObjectSerialization.Factories
{
    internal class GenericSerializerFactory
    {
        private static readonly ConcurrentDictionary<string, Type> _typeDictionary = new ConcurrentDictionary<string, Type>();

        public static Func<BinaryReader, object> GetDeserializer(Type builderGenericType, Type type)
        {
            Type builderType = builderGenericType.MakeGenericType(type);
            PropertyInfo property = builderType.GetProperty("DeserializeFn", BindingFlags.Static | BindingFlags.Public);
            return (Func<BinaryReader, object>)property.GetValue(null, null);
        }

        public static Action<BinaryWriter, object> GetSerializer(Type builderGenericType, Type type)
        {
            Type builderType = builderGenericType.MakeGenericType(type);
            PropertyInfo property = builderType.GetProperty("SerializeFn", BindingFlags.Static | BindingFlags.Public);
            return (Action<BinaryWriter, object>)property.GetValue(null, null);
        }

        public static Type LoadType(string type)
        {
            Type result;
            if (_typeDictionary.TryGetValue(type, out result))
                return result;
            result = FindType(type);
            _typeDictionary.TryAdd(type, result);
            return result;
        }

        private static Type FindType(string type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(type, false)).First(t => t != null);
        }
    }
}