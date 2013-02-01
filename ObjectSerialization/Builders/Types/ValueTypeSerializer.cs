using System;
using System.Linq.Expressions;
using ObjectSerialization.Factories;

namespace ObjectSerialization.Builders.Types
{
    internal class ValueTypeSerializer : BaseTypeSerializer, ISerializer
    {
        #region ISerializer Members

        public bool IsSupported(Type type)
        {
            return type.IsValueType;
        }

        public Expression Write(Expression writerObject, Expression value, Type valueType)
        {
            /*BinaryWriter w;
            object o;
            StructMembersSerializerFactory.GetSerializer(type).Invoke(w, ((T)o).Prop);
            */

            return CallSerializeWithConvert(GetSerializer<StructMembersSerializerFactory>(Expression.Constant(valueType)), value, writerObject);
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            StructMembersSerializerFactory.GetDeserializer(typeof(T)).Invoke(r)
            */
            return CallDeserialize(GetDeserializer<StructMembersSerializerFactory>(Expression.Constant(expectedValueType)), expectedValueType, readerObject);
        }

        #endregion
    }
}