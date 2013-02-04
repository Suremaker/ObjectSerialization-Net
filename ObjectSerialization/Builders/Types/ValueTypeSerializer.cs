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
			T v;
			TypeMembersSerializerBuilder<T>.SerializeFn.Invoke(w, v);
			*/

			return CallSerializeWithConvert(GetDirectSerializer(typeof(TypeMembersSerializerBuilder<>),valueType), value, writerObject);
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
			/*BinaryReader r;
			return TypeMembersSerializerBuilder<T>.DeserializeFn.Invoke(r)
			*/
			return CallDeserialize(GetDirectDeserializer(typeof(TypeMembersSerializerBuilder<>), expectedValueType), expectedValueType, readerObject);
        }

        #endregion
    }
}