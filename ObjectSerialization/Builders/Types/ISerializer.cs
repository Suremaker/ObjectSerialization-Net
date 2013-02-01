using System;
using System.Linq.Expressions;

namespace ObjectSerialization.Builders.Types
{
    internal interface ISerializer
    {
        bool IsSupported(Type type);
        Expression Write(Expression writerObject, Expression value, Type valueType);
        Expression Read(Expression readerObject, Type expectedValueType);
    }
}