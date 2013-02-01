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
            typeof (uint), typeof (long), typeof (ulong), typeof (float), typeof (double), typeof (decimal)
        };

        #region ISerializer Members

        public bool IsSupported(Type type)
        {
            return _predefinedTypes.Contains(type);
        }

        public Expression Write(Expression writerObject, Expression value, Type valueType)
        {
            /*BinaryWriter w;   
            T v;
            w.Write(v);*/
            return GetWriteExpression(value, writerObject);
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {            
            /*BinaryReader r;
            return r.ReadTTT();*/
            return GetReadExpression("Read" + expectedValueType.Name, readerObject);
        }

        #endregion
    }
}