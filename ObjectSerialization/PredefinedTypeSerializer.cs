using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSerialization
{
    internal class PredefinedTypeSerializer : BaseTypeSerializer
    {
        private static readonly IEnumerable<Type> _predefinedTypes = new[]
        {
            typeof (bool), typeof (byte), typeof (sbyte), typeof (char), typeof (ushort), typeof (short), typeof (int),
            typeof (uint), typeof (long), typeof (ulong), typeof (float), typeof (double), typeof (decimal),
            typeof(string)
        };

        public static bool IsSupported(Type type)
        {
            return _predefinedTypes.Contains(type);
        }

        public static Expression Write(Expression writerObject, Expression value)
        {
            /*BinaryWriter w;   
            object o;
            w.Write(((T)o).Prop);*/
            return GetWriteExpression(value, writerObject);
        }

        public static Expression Read(ParameterExpression readerObject, Type expectedValueType)
        {
            /*T o;
            BinaryReader r;
            o.Prop = r.ReadString();*/
            return GetReadExpression("Read" + expectedValueType.Name, readerObject);
        }
    }
}