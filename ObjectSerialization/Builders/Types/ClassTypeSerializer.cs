using System;
using System.Linq.Expressions;
using ObjectSerialization.Types;

namespace ObjectSerialization.Builders.Types
{
	internal class ClassTypeSerializer : BaseTypeSerializer, ISerializer
	{
		#region ISerializer Members

		public bool IsSupported(Type type)
		{
			return type.IsClass || type.IsAbstract || type.IsInterface;
		}

		public Expression Write(Expression writerObject, Expression value, Type valueType)
		{
			/*BinaryWriter w;
			T v;
			if(v != null)
			{
				if(v.GetType() == typeof(T))
				{
					w.Write((byte)1);
					ClassMembersSerializerBuilder<T>.SerializeFn.Invoke(w, v);
				}
				else
				{
					w.Write((byte)2);
					TypeInfoWriter.WriteInfo(w, v.GetType()).Serializer.Invoke(w, v);
				}
			}
			else
				w.Write((byte)0);*/


			Expression checkNotNull = CheckNotNull(value, valueType);
			BlockExpression serializeClass = Expression.Block(
				GetWriteExpression(Expression.Constant((byte)1), writerObject),
				CallSerialize(GetDirectSerializer(typeof(ClassMembersSerializerBuilder<>), valueType), value, writerObject));

			BlockExpression serializePolymorphicClass = Expression.Block(
				GetWriteExpression(Expression.Constant((byte)2), writerObject),
				CallSerialize(GetSerializerField(WriteTypeInfo(writerObject, value)), value, writerObject));

			ConditionalExpression serializationExpression = Expression.IfThenElse(
				Expression.Equal(GetActualValueType(value), Expression.Constant(valueType)),
				serializeClass,
				serializePolymorphicClass
				);

			return Expression.IfThenElse(checkNotNull,
				serializationExpression,
				GetWriteExpression(Expression.Constant((byte)0), writerObject));

		}

		public Expression Read(Expression readerObject, Type expectedValueType)
		{
			/*BinaryReader r;
			byte b = r.ReadByte();
			return (b == 0)
				? null
				: ((b == 1)
					? ClassMembersSerializerBuilder<T>.DeserializeFn.Invoke(r)
					:TypeInfoWriter.ReadInfo(r).Deserializer.Invoke(r));*/

			ParameterExpression flag = Expression.Variable(typeof(byte), "b");
			BinaryExpression readFlag = Expression.Assign(flag, GetReadExpression("ReadByte", readerObject));
			Expression deserializeClass = CallDeserialize(GetDirectDeserializer(typeof(ClassMembersSerializerBuilder<>), expectedValueType), expectedValueType, readerObject);
			Expression deserializePolymorphic = CallDeserialize(GetDeserializerField(ReadTypeInfo(readerObject)), expectedValueType, readerObject);

			ConditionalExpression deserialization = Expression.Condition(
				Expression.Equal(flag, Expression.Constant((byte)0)),
				Expression.Constant(null, expectedValueType),
				Expression.Condition(
					Expression.Equal(flag, Expression.Constant((byte)1)),
					deserializeClass,
					deserializePolymorphic));

			return Expression.Block(new[] { flag }, readFlag, deserialization);
		}

		private Expression GetSerializerField(Expression typeInfo)
		{
			return Expression.Field(typeInfo, "Serializer");
		}

		private Expression GetDeserializerField(Expression typeInfo)
		{
			return Expression.Field(typeInfo, "Deserializer");
		}

		private Expression WriteTypeInfo(Expression writerObject, Expression value)
		{
			return Expression.Call(typeof(TypeInfoWriter), "WriteInfo", null, writerObject, GetActualValueType(value));
		}

		private MethodCallExpression ReadTypeInfo(Expression readerObject)
		{
			return Expression.Call(typeof(TypeInfoWriter), "ReadInfo", null, readerObject);
		}

		#endregion
	}
}