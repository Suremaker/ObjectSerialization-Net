using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSerialization.Builders.Types
{
    internal class PredefinedTypeSerializer : BaseTypeSerializer, ISerializer
    {
        private static readonly IEnumerable<Type> _predefinedTypes = new[]
        {
            typeof (bool), typeof (byte), typeof (sbyte), typeof (char), typeof (ushort), typeof (short), typeof (int),
            typeof (uint), typeof (long), typeof (ulong), typeof (float), typeof (double), typeof (decimal),
            typeof(string)
        };

        public bool IsSupported(Type type)
        {
            return _predefinedTypes.Contains(type);
        }

        public Expression Write(Expression writerObject, Expression value,Type expectedValueType)
        {
            /*BinaryWriter w;   
            object o;
            w.Write(((T)o).Prop);*/
            return GetWriteExpression(value, writerObject);
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*T o;
            BinaryReader r;
            o.Prop = r.ReadString();*/
            return GetReadExpression("Read" + expectedValueType.Name, readerObject);
        }
    }
}