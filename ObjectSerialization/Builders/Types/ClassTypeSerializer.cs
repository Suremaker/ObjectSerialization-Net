using System;
using CodeBuilder;
using CodeBuilder.Expressions;
using ObjectSerialization.Types;

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
            T v;
            if(v != null)
            {
                if(v.GetType() == typeof(T))
                {
                    w.Write((byte)1);
                    TypeMembersSerializerBuilder<T>.SerializeFn.Invoke(w, v);
                }
                else
                {
                    w.Write((byte)2);
                    TypeInfoWriter.WriteInfo(w, v.GetType()).Serializer.Invoke(w, v);
                }
            }
            else
                w.Write((byte)0);*/


            Expression checkNotNull = CheckNotNull(value, valueType);
            BlockExpression serializeClass = Expr.Block(
                GetWriteExpression(Expr.Constant((byte)1), writerObject),
                CallSerialize(GetDirectSerializer(typeof(TypeMembersSerializerBuilder<>), valueType), value, writerObject));

            BlockExpression serializePolymorphicClass = Expr.Block(
                GetWriteExpression(Expr.Constant((byte)2), writerObject),
                CallSerialize(GetSerializerField(WriteTypeInfo(writerObject, value)), value, writerObject));

            Expression serializationExpression = GetSerializationExpression(value, valueType, serializeClass, serializePolymorphicClass);

            return Expr.IfThenElse(checkNotNull,
                serializationExpression,
                GetWriteExpression(Expr.Constant((byte)0), writerObject));

        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            byte b = r.ReadByte();
            return (b == 0)
                ? null
                : ((b == 1)
                    ? TypeMembersSerializerBuilder<T>.DeserializeFn.Invoke(r)
                    :TypeInfoWriter.ReadInfo(r).Deserializer.Invoke(r));*/

            var flag = Expr.LocalVariable(typeof(byte), "b");
            var readFlag = Expr.DeclareLocal(flag, GetReadExpression("ReadByte", readerObject));
            Expression deserializeClass = CallDeserialize(GetDirectDeserializer(typeof(TypeMembersSerializerBuilder<>), expectedValueType), readerObject);
            Expression deserializePolymorphic = CallDeserializeWithConvert(GetDeserializerField(ReadTypeInfo(readerObject)), expectedValueType, readerObject);

            var deserialization = Expr.IfThenElse(
                Expr.ReadLocal(flag),
                GetDeserializationExpression(Expr.ReadLocal(flag), expectedValueType, deserializeClass, deserializePolymorphic),
                Expr.Null(expectedValueType));

            return Expr.ValueBlock(expectedValueType, readFlag, deserialization);
        }

        #endregion

        private static Expression GetDeserializationExpression(Expression flag, Type expectedValueType, Expression deserializeClass, Expression deserializePolymorphic)
        {
            if (expectedValueType.IsSealed)
                return deserializeClass;
            if (IsPurePolymorphic(expectedValueType))
                return deserializePolymorphic;

            return Expr.IfThenElse(
                Expr.Equal(flag, Expr.Constant((byte)1)),
                deserializeClass,
                deserializePolymorphic);
        }

        private static Expression GetSerializationExpression(Expression value, Type valueType, BlockExpression serializeClass, BlockExpression serializePolymorphicClass)
        {
            if (valueType.IsSealed)
                return serializeClass;

            if (IsPurePolymorphic(valueType))
                return serializePolymorphicClass;

            return Expr.IfThenElse(
                Expr.Equal(GetActualValueType(value), Expr.Constant(valueType)),
                serializeClass,
                serializePolymorphicClass
                );
        }

        private static bool IsPurePolymorphic(Type valueType)
        {
            return valueType.IsAbstract || valueType.IsInterface || valueType == typeof(object);
        }

        private Expression GetDeserializerField(Expression typeInfo)
        {
            return Expr.ReadField(typeInfo, "Deserializer");
        }

        private Expression GetSerializerField(Expression typeInfo)
        {
            return Expr.ReadField(typeInfo, "Serializer");
        }

        private CallExpression ReadTypeInfo(Expression readerObject)
        {
            return Expr.Call(typeof(TypeInfoWriter), "ReadInfo", readerObject);
        }

        private Expression WriteTypeInfo(Expression writerObject, Expression value)
        {
            return Expr.Call(typeof(TypeInfoWriter), "WriteInfo", writerObject, GetActualValueType(value));
        }
    }
}