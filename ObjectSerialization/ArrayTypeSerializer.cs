using System;
using System.Linq.Expressions;

namespace ObjectSerialization
{
    internal class ArrayTypeSerializer : BaseTypeSerializer
    {
        public static bool IsSupported(Type type)
        {
            return type.IsArray;
        }

        public static Expression Write(Expression writerObject, Expression value, Type propertyType)
        {
            throw new NotImplementedException();
        }

        public static Expression Read(ParameterExpression readerObject, Type expectedValueType)
        {
            throw new NotImplementedException();
        }
    }
}