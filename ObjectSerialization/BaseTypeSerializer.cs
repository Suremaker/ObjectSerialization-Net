using System;
using System.Linq.Expressions;

namespace ObjectSerialization
{
    internal class BaseTypeSerializer
    {
        protected static Expression GetWriteExpression(Expression valueExpression, Expression writer)
        {
            return Expression.Call(writer, "Write", null, valueExpression);
        }

        protected static Expression GetReadExpression(string method, Expression reader)
        {
            return Expression.Call(reader, method, new Type[0]);
        }

        protected static Expression CheckNotNull(Expression value, Type valueType)
        {
            return Expression.ReferenceNotEqual(value, Expression.Constant(null, valueType));
        }

        protected static Expression GetSerializer(Expression type)
        {
            return Expression.Call(typeof(TypeSerializer), "GetSerializer", null, type);
        }

        protected static Expression CallSerialize(Expression serializer, Expression value, Expression writerObject)
        {
            return Expression.Call(serializer, "Invoke", null, writerObject, value);
        }

        protected static Expression GetDeserializer(Expression type)
        {
            return Expression.Call(typeof(TypeSerializer), "GetDeserializer", null, type);
        }

        protected static Expression CallDeserialize(Expression deserializer, Type propertyType, Expression readerObject)
        {
            return Expression.TypeAs(Expression.Call(deserializer, "Invoke", null, readerObject), propertyType);
        }

        protected static Expression GetActualValueType(Expression value)
        {
            return Expression.Call(Expression.TypeAs(value, typeof(object)), "GetType", null);
        }

        protected static Expression WriteObjectType(Expression value, Expression objectWriter)
        {
            var valueType = GetActualValueType(value);
            var typeFullName = Expression.Property(valueType, "FullName");
            return GetWriteExpression(typeFullName, objectWriter);
        }

        protected static Expression ReloadType(Expression readerObject)
        {
            return Expression.Call(typeof(TypeSerializer), "LoadType", null, GetReadExpression("ReadString", readerObject));
        }
    }
}