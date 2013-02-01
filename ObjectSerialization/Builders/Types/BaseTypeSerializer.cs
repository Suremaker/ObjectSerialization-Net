using System;
using System.Linq.Expressions;
using ObjectSerialization.Factories;

namespace ObjectSerialization.Builders.Types
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
            return Expression.Call(typeof(TypeSerializerFactory), "LoadType", null, GetReadExpression("ReadString", readerObject));
        }
    }
}