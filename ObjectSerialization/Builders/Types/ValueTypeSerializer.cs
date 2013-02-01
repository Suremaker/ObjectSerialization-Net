using System;
using System.Linq.Expressions;

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

            return CallSerializeWithConvert(GetDirectSerializer(typeof(StructMembersSerializerBuilder<>),valueType), value, writerObject);
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            StructMembersSerializerFactory.GetDeserializer(typeof(T)).Invoke(r)
            */
            return CallDeserialize(GetDirectDeserializer(typeof(StructMembersSerializerBuilder<>), expectedValueType), expectedValueType, readerObject);
        }

        #endregion
    }
}