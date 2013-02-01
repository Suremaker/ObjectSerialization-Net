using System;
using System.Linq.Expressions;

namespace ObjectSerialization
{
    internal class PolymorphicClassTypeSerializer : BaseTypeSerializer
    {
        public static bool IsSupported(Type type)
        {
            return type == typeof(object) || type.IsAbstract || type.IsInterface;
        }

        public static Expression Write(Expression writerObject, Expression value, Type expectedValueType)
        {
            /*BinaryWriter w;
            object o;
            w.Write(((T)o).Prop!=null);
            if(((T)o).Prop!=null)
            {
                w.Write(((T)o).Prop.GetType().FullName);
                TypeSerializer.GetSerializer(((T)o).Prop.GetType()).Invoke(w, ((T)o).Prop);
            }*/
            var isNullExpression = CheckNotNull(value, expectedValueType);
            var valueWriteExpression = CallSerialize(GetSerializer(GetActualValueType(value)), value, writerObject);
            var objectTypeWriteExpression = WriteObjectType(value, writerObject);

            return Expression.Block(
                GetWriteExpression(isNullExpression, writerObject),
                Expression.IfThen(isNullExpression, Expression.Block(objectTypeWriteExpression, valueWriteExpression)));

        }

        public static Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            T o;
            if (r.ReadBoolean())            
                o.Prop = TypeSerializer.GetDeserializer(TypeSerializer.LoadType(r.ReadString())).Invoke(r);*/
            var readBool = GetReadExpression("ReadBoolean", readerObject);
            var readValue = CallDeserialize(GetDeserializer(ReloadType(readerObject)), expectedValueType, readerObject);
            return Expression.Condition(readBool, readValue, Expression.Constant(null, expectedValueType));
        }
    }
}