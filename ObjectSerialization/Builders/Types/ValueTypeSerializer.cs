using System;
using System.Linq.Expressions;

namespace ObjectSerialization.Builders.Types
{
    internal class ValueTypeSerializer : BaseTypeSerializer, ISerializer
    {
        public bool IsSupported(Type type)
        {
            return type.IsValueType;
        }

        public Expression Write(Expression writerObject, Expression value, Type propertyType)
        {
            throw new NotImplementedException();
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            throw new NotImplementedException();
        }
    }
}