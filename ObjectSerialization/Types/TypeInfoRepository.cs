using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ObjectSerialization.Builders;

namespace ObjectSerialization.Types
{
	public static class TypeInfoRepository
	{
		private static readonly ConcurrentDictionary<string, TypeInfo> _typeDictionary = new ConcurrentDictionary<string, TypeInfo>();
		private static readonly List<TypeInfo> _predefinedTypeList = new List<TypeInfo>();

		static TypeInfoRepository()
		{
			var predefinedTypes = new[]
			{
				typeof (string),
				typeof (bool),
				typeof (byte),
				typeof (sbyte),
				typeof (ushort),
				typeof (short),
				typeof (uint),
				typeof (int),
				typeof (ulong),
				typeof (long),
				typeof (char),
				typeof (float),
				typeof (double),
				typeof (decimal),
				typeof (object),
				typeof (DateTime),
				typeof (TimeSpan),
				typeof (Guid)
			};

			foreach (Type type in predefinedTypes)
			{
				RegisterPredefined(type);
				RegisterPredefined(type.MakeArrayType());
			}
		}

		public static void RegisterPredefined(Type type)
		{
			TypeInfo info = LoadTypeInfo(type);
			if (!_typeDictionary.TryAdd(type.FullName, info))
				throw new ArgumentException("Type {0} cannot be registered as predefined one, because it already exist in repository", type.FullName);
			lock (_predefinedTypeList)
			{
				info.ShortTypeId = _predefinedTypeList.Count;
				_predefinedTypeList.Add(info);
			}
		}

		public static void RegisterPredefinedUsingSerializableFrom(Assembly asm)
		{
			foreach (var type in asm.GetTypes()
				.Where(t => t.GetCustomAttributes(typeof(SerializableAttribute), true).Any())
				.Where(t => !t.ContainsGenericParameters)
				.OrderBy(t => t.FullName))
			{
				RegisterPredefined(type);
				RegisterPredefined(type.MakeArrayType());
			}
		}

		internal static TypeInfo GetTypeInfo(Type type)
		{
			TypeInfo info;
			if (_typeDictionary.TryGetValue(type.FullName, out info))
				return info;

			info = LoadTypeInfo(type);
			_typeDictionary.TryAdd(type.FullName, info);
			return info;
		}

		internal static TypeInfo GetTypeInfo(string longTypeId)
		{
			TypeInfo info;
			if (_typeDictionary.TryGetValue(longTypeId, out info))
				return info;

			info = LoadTypeInfo(longTypeId);
			_typeDictionary.TryAdd(longTypeId, info);
			return info;
		}

		internal static TypeInfo GetTypeInfo(int shortTypeId)
		{
			if (shortTypeId < 0 || shortTypeId >= _predefinedTypeList.Count)
				throw new ArgumentException("Unknown type id: " + shortTypeId);
			return _predefinedTypeList[shortTypeId];
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
				Type = type,
				LongTypeId = type.FullName,
				ShortTypeId = null,
				Serializer = (Action<BinaryWriter, object>)builderType.GetProperty("SerializeFn", BindingFlags.Static | BindingFlags.Public).GetValue(null, null),
				Deserializer = (Func<BinaryReader, object>)builderType.GetProperty("DeserializeFn", BindingFlags.Static | BindingFlags.Public).GetValue(null, null)
			};
		}
	}
}
