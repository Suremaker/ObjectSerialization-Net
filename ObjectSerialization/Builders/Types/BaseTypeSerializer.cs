using System;
using System.IO;
using System.Runtime.Serialization;
using CodeBuilder;
using CodeBuilder.Expressions;

namespace ObjectSerialization.Builders.Types
{
    internal class BaseTypeSerializer
    {
        public static Expression InstantiateNew(Type type)
        {
            if (type.IsClass && type.GetConstructor(new Type[0]) == null)
                return Expr.Convert(Expr.Call(typeof(FormatterServices), "GetUninitializedObject", Expr.Constant(type)), type);
            return Expr.New(type);
        }

        protected static Expression CallDeserialize(Expression deserializer, Expression readerObject)
        {
            return Expr.Call(deserializer, "Invoke", readerObject);
        }

        protected static Expression CallDeserializeWithConvert(Expression deserializer, Type propertyType, Expression readerObject)
        {
            return Expr.Convert(Expr.Call(deserializer, "Invoke", readerObject), propertyType);
        }

        protected static Expression CallSerialize(Expression serializer, Expression value, Expression writerObject)
        {
            return Expr.Call(serializer, "Invoke", writerObject, value);
        }

        protected static Expression CallSerializeWithConvert(Expression serializer, Expression value, Type valueType, Expression writerObject)
        {
            return Expr.Call(serializer, "Invoke", writerObject, Expr.Convert(value, valueType));
        }

        protected static Expression CheckNotNull(Expression value, Type valueType)
        {
            //return Expr.ReferenceNotEqual(value, Expression.Constant(null, valueType));
            if (!valueType.IsClass && !valueType.IsInterface) throw new ArgumentException("Expected class type, got: " + valueType);
            return Expr.IfThenElse(value, Expr.Constant(true), Expr.Constant(false));
        }

        protected static Expression GetActualValueType(Expression value)
        {
            return Expr.Call(Expr.Convert(value, typeof(object)), "GetType"); //TODO: optimize?
        }

        protected static Expression GetDeserializer<TSerializerFactory>(Expression type)
        {
            return Expr.Call(typeof(TSerializerFactory), "GetDeserializer", type);
        }

        protected static Expression GetDeserializer<TSerializerFactory>(Type type)
        {
            return Expr.Call(typeof(TSerializerFactory), "GetDeserializer", new[] { type });
        }

        protected static Expression GetReadExpression(string method, Expression reader)
        {
            return Expr.Call(reader, method);
        }

        protected static Expression GetSerializer<TSerializerFactory>(Expression type)
        {
            return Expr.Call(typeof(TSerializerFactory), "GetSerializer", type);
        }

        protected static Expression GetSerializer<TSerializerFactory>(Type type)
        {
            return Expr.Call(typeof(TSerializerFactory), "GetSerializer", new[] { type });
        }

        protected static Expression GetWriteExpression(Expression valueExpression, Expression writer)
        {
            return Expr.Call(writer, "Write", valueExpression);
        }

        protected static Expression ReloadType(Expression readerObject)
        {
            return GetReadExpression("ReadString", readerObject);
        }

        protected static Expression WriteObjectType(Expression value, Expression objectWriter)
        {
            Expression valueType = GetActualValueType(value);
            var typeFullName = Expr.ReadProperty(valueType, typeof(Type).GetProperty("FullName"));
            return GetWriteExpression(typeFullName, objectWriter);
        }

        protected Expression GetDirectDeserializer(Type builderType, Type valueType)
        {
            return Expr.ReadProperty(builderType.MakeGenericType(valueType).GetProperty("DeserializeFn"));
        }

        protected Expression GetDirectSerializer(Type builderType, Type valueType)
        {
            return Expr.ReadProperty(builderType.MakeGenericType(valueType).GetProperty("SerializeFn"));
        }

        protected static Type GetWriteSerializerDelegateType(Type type)
        {
            return typeof(Action<,>).MakeGenericType(typeof(BinaryWriter), type);
        }

        protected static Type GetReadSerializerDelegateType(Type type)
        {
            return typeof(Func<,>).MakeGenericType(typeof(BinaryReader), type);
        }
    }
}