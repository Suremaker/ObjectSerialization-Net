using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ObjectSerialization.Builders.Types
{
	internal class BaseTypeSerializer
	{
		public static Expression InstantiateNew(Type type)
		{
			if (type.IsClass && type.GetConstructor(new Type[0]) == null)
				return Expression.TypeAs(Expression.Call(typeof(FormatterServices), "GetUninitializedObject", null, Expression.Constant(type)), type);
			return Expression.New(type);
		}

		protected static Expression CallDeserialize(Expression deserializer, Expression readerObject)
		{
			return Expression.Call(deserializer, "Invoke", null, readerObject);
		}

		protected static Expression CallDeserializeWithConvert(Expression deserializer, Type propertyType, Expression readerObject)
		{
			return Expression.Convert(Expression.Call(deserializer, "Invoke", null, readerObject), propertyType);
		}

		protected static Expression CallSerialize(Expression serializer, Expression value, Expression writerObject)
		{
			return Expression.Call(serializer, "Invoke", null, writerObject, value);
		}

		protected static Expression CallSerializeWithConvert(Expression serializer, Expression value, Type valueType, Expression writerObject)
		{
			return Expression.Call(serializer, "Invoke", null, writerObject, Expression.Convert(value, valueType));
		}

		protected static Expression CheckNotNull(Expression value, Type valueType)
		{
			return Expression.ReferenceNotEqual(value, Expression.Constant(null, valueType));
		}

		protected static Expression GetActualValueType(Expression value)
		{
			return Expression.Call(Expression.TypeAs(value, typeof(object)), "GetType", null);
		}

		protected static Expression GetDeserializer<TSerializerFactory>(Expression type)
		{
			return Expression.Call(typeof(TSerializerFactory), "GetDeserializer", null, type);
		}

		protected static Expression GetDeserializer<TSerializerFactory>(Type type)
		{
			return Expression.Call(typeof(TSerializerFactory), "GetDeserializer", new[] { type });
		}

		protected static Expression GetReadExpression(string method, Expression reader)
		{
			return Expression.Call(reader, method, new Type[0]);
		}

		protected static Expression GetSerializer<TSerializerFactory>(Expression type)
		{
			return Expression.Call(typeof(TSerializerFactory), "GetSerializer", null, type);
		}

		protected static Expression GetSerializer<TSerializerFactory>(Type type)
		{
			return Expression.Call(typeof(TSerializerFactory), "GetSerializer", new[] { type });
		}

		protected static Expression GetWriteExpression(Expression valueExpression, Expression writer)
		{
			return Expression.Call(writer, "Write", null, valueExpression);
		}

		protected static Expression ReloadType(Expression readerObject)
		{
			return GetReadExpression("ReadString", readerObject);
		}

		protected static Expression WriteObjectType(Expression value, Expression objectWriter)
		{
			Expression valueType = GetActualValueType(value);
			MemberExpression typeFullName = Expression.Property(valueType, "FullName");
			return GetWriteExpression(typeFullName, objectWriter);
		}

		protected Expression GetDirectDeserializer(Type builderType, Type valueType)
		{
			return Expression.Property(null, builderType.MakeGenericType(valueType), "DeserializeFn");
		}

		protected Expression GetDirectSerializer(Type builderType, Type valueType)
		{
			return Expression.Property(null, builderType.MakeGenericType(valueType), "SerializeFn");
		}

		protected static Type GetWriteSerializerDelegateType(Type type)
		{
			return typeof(Action<,>).MakeGenericType(typeof(BinaryWriter), type);
		}

		protected static Type GetReadSerializerDelegateType(Type type)
		{
			return typeof(Func<,>).MakeGenericType(typeof(BinaryReader), type);
		}
	}
}