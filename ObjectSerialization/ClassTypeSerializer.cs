using System;
using System.Linq.Expressions;

namespace ObjectSerialization
{
    internal class ClassTypeSerializer : BaseTypeSerializer, ISerializer
    {
        public bool IsSupported(Type type)
        {
            return type.IsClass && !type.IsArray;
        }

        public Expression Write(Expression writerObject, Expression value, Type expectedValueType)
        {
            /*BinaryWriter w;
            object o;
            w.Write(((T)o).Prop!=null);
            if(((T)o).Prop!=null)
                TypeSerializer.GetSerializer(type).Invoke(w, ((T)o).Prop);*/

            var isNullExpression = CheckNotNull(value, expectedValueType);
            var writeExpression = CallSerialize(GetSerializer(Expression.Constant(expectedValueType)), value, writerObject);

            return Expression.Block(
                GetWriteExpression(isNullExpression, writerObject),
                Expression.IfThen(isNullExpression, writeExpression));
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            T o;
            if (r.ReadBoolean())
                o.Prop = TypeSerializer.GetDeserializer(typeof(T)).Invoke(r);*/

            var readBool = GetReadExpression("ReadBoolean", readerObject);
            var readValue = CallDeserialize(GetDeserializer(Expression.Constant(expectedValueType)), expectedValueType, readerObject);
            return Expression.Condition(readBool, readValue, Expression.Constant(null, expectedValueType));
        }
    }
}