using System;
using System.IO;
using System.Linq.Expressions;
using ObjectSerialization.Factories;

namespace ObjectSerialization.Builders.Types
{
    internal class ArrayTypeSerializer : BaseTypeSerializer, ISerializer
    {
        public bool IsSupported(Type type)
        {
            return type.IsArray;
        }

        public Expression Write(Expression writerObject, Expression value, Type valueType)
        {
            /*BinaryWriter w;
            object o;
            if (((T) o) == null)
            {
                w.Write((int) -1);
            }
            else
            {
                w.Write(((T)o).Length);                
                int c = ((T) o).Length;
                int s = TypeSerializerFactory.GetSerializer(typeof (T).GetElementType());
                for(var i=0 ; i < c; ++i)
                    s.Invoke(w, ((T)o)[i]);
            }*/
            var checkNotNull = CheckNotNull(value, valueType);

            var forLoop = CreateWriteLoop(writerObject, value, valueType);
            BlockExpression arrayWrite = Expression.Block(GetWriteExpression(Expression.Property(value, "Length"), writerObject), forLoop);

            ConditionalExpression expression = Expression.IfThenElse(checkNotNull, arrayWrite, GetWriteExpression(Expression.Constant(-1), writerObject));
            return expression;
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            int c = r.ReadInt32();
            return (c == -1)
                ? null
                :
            {
                int s = TypeSerializerFactory.GetDeserializer(typeof (T).GetElementType());
                for(var i=0 ; i < c; ++i)
                    ((T)o)[i] = s.Invoke(r);
            }*/
            var count = Expression.Variable(typeof(int), "c");
            var countRead = Expression.Assign(count, GetReadExpression("ReadInt32", readerObject));

            BlockExpression expression = Expression.Block(new[] { count },
                countRead,
                Expression.Condition(
                    Expression.Equal(count, Expression.Constant(-1)),
                    Expression.Constant(null, expectedValueType),
                    CreateReadLoop(readerObject, expectedValueType, count)));
            return expression;
        }

        private Expression CreateReadLoop(Expression readerObject, Type expectedValueType, ParameterExpression count)
        {
            var index = Expression.Parameter(typeof(int), "i");
            var deserializer = Expression.Parameter(typeof(Func<BinaryReader, object>), "s");
            var result = Expression.Parameter(expectedValueType, "r");
            LabelTarget loopEndLabel = Expression.Label(expectedValueType);

            var forLoop = Expression.Block(
                new[] { index, result, deserializer },
                Expression.Assign(result, Expression.NewArrayBounds(expectedValueType.GetElementType(), count)),
                Expression.Assign(index, Expression.Constant(0, typeof(int))),
                Expression.Assign(deserializer, GetDeserializer(Expression.Constant(expectedValueType.GetElementType()))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(index, count),
                        Expression.Block(
                            Expression.Assign(Expression.ArrayAccess(result, index), CallDeserialize(deserializer, expectedValueType.GetElementType(), readerObject)),
                            Expression.PreIncrementAssign(index)),
                        Expression.Break(loopEndLabel, result)),
                    loopEndLabel));

            return forLoop;
        }

        private static BlockExpression CreateWriteLoop(Expression writerObject, Expression value, Type valueType)
        {
            var index = Expression.Parameter(typeof(int), "i");
            var count = Expression.Parameter(typeof(int), "c");
            var serializer = Expression.Parameter(typeof(Action<BinaryWriter, object>), "s");
            LabelTarget loopEndLabel = Expression.Label();

            return Expression.Block(
                new[] { index, count, serializer },
                Expression.Assign(index, Expression.Constant(0, typeof(int))),
                Expression.Assign(count, Expression.Property(value, "Length")),
                Expression.Assign(serializer, GetSerializer(Expression.Constant(valueType.GetElementType()))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(index, count),
                        Expression.Block(
                            CallSerialize(serializer, Expression.ArrayAccess(value, index), writerObject),
                            Expression.PreIncrementAssign(index)),
                        Expression.Break(loopEndLabel)),
                    loopEndLabel));
        }

        protected static Expression GetSerializer(Expression type)
        {
            return Expression.Call(typeof(TypeSerializerFactory), "GetSerializer", null, type);
        }

        protected static Expression CallSerialize(Expression serializer, Expression value, Expression writerObject)
        {
            return Expression.Call(serializer, "Invoke", null, writerObject, Expression.Convert(value, typeof(object)));
        }

        protected static Expression GetDeserializer(Expression type)
        {
            return Expression.Call(typeof(TypeSerializerFactory), "GetDeserializer", null, type);
        }

        protected static Expression CallDeserialize(Expression deserializer, Type type, Expression readerObject)
        {
            return Expression.Convert(Expression.Call(deserializer, "Invoke", null, readerObject), type);
        }
    }
}