using System;
using System.IO;
using System.Linq.Expressions;
using ObjectSerialization.Factories;

namespace ObjectSerialization.Builders.Types
{
    internal class ArrayTypeSerializer : BaseTypeSerializer, ISerializer
    {
        #region ISerializer Members

        public bool IsSupported(Type type)
        {
            return type.IsArray;
        }

        public Expression Write(Expression writerObject, Expression value, Type valueType)
        {
			/*BinaryWriter w;
            T[] v;
            if (v != null)
            {
				w.Write(v.Length);
				int c = v.Length;
				int s = TypeSerializerFactory.GetSerializer(typeof(T));
				for (var i = 0; i < c; ++i)
					s.Invoke(w, v[i]);                
            }
            else
				w.Write(-1);*/

            Expression checkNotNull = CheckNotNull(value, valueType);

            BlockExpression forLoop = CreateWriteLoop(writerObject, value, valueType);
            BlockExpression arrayWrite = Expression.Block(GetWriteExpression(Expression.Property(value, "Length"), writerObject), forLoop);

            return Expression.IfThenElse(checkNotNull, arrayWrite, GetWriteExpression(Expression.Constant(-1), writerObject));
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            int c = r.ReadInt32();
	        if (c == -1)
		        return null;
            
            T[]v=new T[c];
            int s = TypeSerializerFactory.GetDeserializer(typeof (T));
            for(var i=0 ; i < c; ++i)
                v[i] = s.Invoke(r);
            return v;
            */

            ParameterExpression count = Expression.Variable(typeof(int), "c");
            BinaryExpression countRead = Expression.Assign(count, GetReadExpression("ReadInt32", readerObject));

            BlockExpression expression = Expression.Block(new[] { count },
                countRead,
                Expression.Condition(
                    Expression.Equal(count, Expression.Constant(-1)),
                    Expression.Constant(null, expectedValueType),
                    CreateReadLoop(readerObject, expectedValueType, count)));
            return expression;
        }

        #endregion

        private static BlockExpression CreateWriteLoop(Expression writerObject, Expression value, Type valueType)
        {
            ParameterExpression index = Expression.Parameter(typeof(int), "i");
            ParameterExpression count = Expression.Parameter(typeof(int), "c");
            ParameterExpression serializer = Expression.Parameter(typeof(Action<BinaryWriter, object>), "s");
            LabelTarget loopEndLabel = Expression.Label();

            return Expression.Block(
                new[] { index, count, serializer },
                Expression.Assign(index, Expression.Constant(0, typeof(int))),
                Expression.Assign(count, Expression.Property(value, "Length")),
                Expression.Assign(serializer, GetSerializer<TypeSerializerFactory>(Expression.Constant(valueType.GetElementType()))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(index, count),
                        Expression.Block(
                            CallSerializeWithConvert(serializer, Expression.ArrayAccess(value, index), writerObject),
                            Expression.PreIncrementAssign(index)),
                        Expression.Break(loopEndLabel)),
                    loopEndLabel));
        }

        private Expression CreateReadLoop(Expression readerObject, Type expectedValueType, ParameterExpression count)
        {
            ParameterExpression index = Expression.Parameter(typeof(int), "i");
            ParameterExpression deserializer = Expression.Parameter(typeof(Func<BinaryReader, object>), "s");
            ParameterExpression result = Expression.Parameter(expectedValueType, "r");
            LabelTarget loopEndLabel = Expression.Label(expectedValueType);

            BlockExpression forLoop = Expression.Block(
                new[] { index, result, deserializer },
                Expression.Assign(result, Expression.NewArrayBounds(expectedValueType.GetElementType(), count)),
                Expression.Assign(index, Expression.Constant(0, typeof(int))),
                Expression.Assign(deserializer, GetDeserializer<TypeSerializerFactory>(Expression.Constant(expectedValueType.GetElementType()))),
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
    }
}