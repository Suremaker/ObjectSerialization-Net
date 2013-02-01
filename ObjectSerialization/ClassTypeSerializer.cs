using System;
using System.Linq.Expressions;

namespace ObjectSerialization
{
    internal class ClassTypeSerializer : BaseTypeSerializer, ISerializer
    {
        public bool IsSupported(Type type)
        {
            return type.IsClass|| type.IsAbstract || type.IsInterface;
        }

        public Expression Write(Expression writerObject, Expression value, Type expectedValueType)
        {
            /*BinaryWriter w;
            object o;
            if(((T)o).Prop!=null)
            {
                if(((T)o).Prop.GetType() == typeof(T))
                {
                    w.Write((byte)1);
                    TypeSerializer.GetSerializer(type).Invoke(w, ((T)o).Prop);
                }
                else
                {
                    w.Write((byte)2);
                    w.Write(((T)o).Prop.GetType().FullName);
                    TypeSerializer.GetSerializer(((T)o).Prop.GetType()).Invoke(w, ((T)o).Prop);
                }
            }
            else
                w.Write((byte)0);
             */


            /*var checkNotNull = CheckNotNull(value, expectedValueType);
            var valueWriteExpression = CallSerialize(GetSerializer(GetActualValueType(value)), value, writerObject);
            var objectTypeWriteExpression = WriteObjectType(value, writerObject);

            return Expression.Block(
                GetWriteExpression(checkNotNull, writerObject),
                Expression.IfThen(checkNotNull, Expression.Block(objectTypeWriteExpression, valueWriteExpression)));*/

            var checkNotNull = CheckNotNull(value, expectedValueType);
            var serializeClass = Expression.Block(
                GetWriteExpression(Expression.Constant((byte)1), writerObject),
                CallSerialize(GetSerializer(Expression.Constant(expectedValueType)), value, writerObject));

            var serializePolymorphicClass = Expression.Block(
                GetWriteExpression(Expression.Constant((byte)2), writerObject),
                WriteObjectType(value, writerObject),
                CallSerialize(GetSerializer(GetActualValueType(value)), value, writerObject));

            var serializationExpression = Expression.IfThenElse(
                Expression.Equal(GetActualValueType(value),Expression.Constant(expectedValueType)),
                serializeClass,
                serializePolymorphicClass
                );

            return Expression.IfThenElse(checkNotNull,
                serializationExpression,
                GetWriteExpression(Expression.Constant((byte) 0),writerObject));

        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            T o;
            byte b = r.ReadByte();
            return (b == 0)
                ? null
                : ((b == 1)
                    ? TypeSerializer.GetDeserializer(typeof(T)).Invoke(r)
                    : TypeSerializer.GetDeserializer(TypeSerializer.LoadType(r.ReadString())).Invoke(r));
            */
            var flag = Expression.Variable(typeof (byte), "b");
            var readFlag = Expression.Assign(flag, GetReadExpression("ReadByte", readerObject));
            var deserializeClass = CallDeserialize(GetDeserializer(Expression.Constant(expectedValueType)), expectedValueType, readerObject);            
            var deserializePolymorphic = CallDeserialize(GetDeserializer(ReloadType(readerObject)), expectedValueType, readerObject);

            var deserialization= Expression.Condition(
                Expression.Equal(flag, Expression.Constant((byte) 0)),
                Expression.Constant(null, expectedValueType),
                Expression.Condition(
                    Expression.Equal(flag, Expression.Constant((byte) 1)),
                    deserializeClass,
                    deserializePolymorphic));

            return Expression.Block(new []{flag},readFlag, deserialization);
        }
    }
}