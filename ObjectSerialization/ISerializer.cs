using System;
using System.Linq.Expressions;

namespace ObjectSerialization
{
    internal interface ISerializer
    {
        bool IsSupported(Type type);
        Expression Write(Expression writerObject, Expression value, Type propertyType);
        Expression Read(Expression readerObject, Type expectedValueType);
    }
}