using System;
using System.Linq.Expressions;
using ObjectSerialization.Factories;

namespace ObjectSerialization.Builders.Types
{
    internal class ClassTypeSerializer : BaseTypeSerializer, ISerializer
    {
        #region ISerializer Members

        public bool IsSupported(Type type)
        {
            return type.IsClass || type.IsAbstract || type.IsInterface;
        }

        public Expression Write(Expression writerObject, Expression value, Type valueType)
        {
            /*BinaryWriter w;
            object o;
            if(((T)o).Prop!=null)
            {
                if(((T)o).Prop.GetType() == typeof(T))
                {
                    w.Write((byte)1);
                    ClassMembersSerializerFactory.GetSerializer(type).Invoke(w, ((T)o).Prop);
                }
                else
                {
                    w.Write((byte)2);
                    w.Write(((T)o).Prop.GetType().FullName);
                    TypeSerializerFactory.GetSerializer(((T)o).Prop.GetType()).Invoke(w, ((T)o).Prop);
                }
            }
            else
                w.Write((byte)0);
             */

            Expression checkNotNull = CheckNotNull(value, valueType);
            BlockExpression serializeClass = Expression.Block(
                GetWriteExpression(Expression.Constant((byte)1), writerObject),
                CallSerialize(GetDirectSerializer(typeof(ClassMembersSerializerBuilder<>), valueType), value, writerObject));

            BlockExpression serializePolymorphicClass = Expression.Block(
                GetWriteExpression(Expression.Constant((byte)2), writerObject),
                WriteObjectType(value, writerObject),
                CallSerialize(GetSerializer<TypeSerializerFactory>(GetActualValueType(value)), value, writerObject));

            ConditionalExpression serializationExpression = Expression.IfThenElse(
                Expression.Equal(GetActualValueType(value), Expression.Constant(valueType)),
                serializeClass,
                serializePolymorphicClass
                );

            return Expression.IfThenElse(checkNotNull,
                serializationExpression,
                GetWriteExpression(Expression.Constant((byte)0), writerObject));

        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            T o;
            byte b = r.ReadByte();
            return (b == 0)
                ? null
                : ((b == 1)
                    ? ClassMembersSerializerFactory.GetDeserializer(typeof(T)).Invoke(r)
                    : TypeSerializerFactory.GetDeserializer(TypeSerializer.LoadType(r.ReadString())).Invoke(r));
            */
            ParameterExpression flag = Expression.Variable(typeof(byte), "b");
            BinaryExpression readFlag = Expression.Assign(flag, GetReadExpression("ReadByte", readerObject));
            Expression deserializeClass = CallDeserialize(GetDirectDeserializer(typeof(ClassMembersSerializerBuilder<>), expectedValueType), expectedValueType, readerObject);
            Expression deserializePolymorphic = CallDeserialize(GetDeserializer<TypeSerializerFactory>(ReloadType(readerObject)), expectedValueType, readerObject);

            ConditionalExpression deserialization = Expression.Condition(
                Expression.Equal(flag, Expression.Constant((byte)0)),
                Expression.Constant(null, expectedValueType),
                Expression.Condition(
                    Expression.Equal(flag, Expression.Constant((byte)1)),
                    deserializeClass,
                    deserializePolymorphic));

            return Expression.Block(new[] { flag }, readFlag, deserialization);
        }

        #endregion
    }
}