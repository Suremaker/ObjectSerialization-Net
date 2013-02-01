using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ObjectSerialization.Factories;

namespace ObjectSerialization.Builders.Types
{
    internal class CollectionTypeSerializer : BaseTypeSerializer, ISerializer
    {
        #region ISerializer Members

        public bool IsSupported(Type type)
        {
            return GetCollectionType(type) != null;
        }

        public Expression Write(Expression writerObject, Expression value, Type valueType)
        {
            /*BinaryWriter w;
            TCollection<T> v;
            if (v!= null)
            {
				w.Write(v.Count);
	            int s = TypeSerializerFactory.GetSerializer(typeof(T));
	            IEnumerator<T> e = v.GetEnumerator();

	            try
				{
					while (e.MoveNext())
						s.Invoke(w, e.Current);
				}
				finally
				{
					e.Dispose();
				}
            }
            else
            {
				w.Write(-1);
            }*/

            Expression checkNotNull = CheckNotNull(value, valueType);

            BlockExpression collectionWrite = Expression.Block(
                GetWriteExpression(Expression.Property(value, "Count"), writerObject),
                CreateWriteLoop(writerObject, value, valueType));

            return Expression.IfThenElse(checkNotNull, collectionWrite, GetWriteExpression(Expression.Constant(-1), writerObject));
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            int c = r.ReadInt32();
            if (c == -1)
                return null;
                
            TCollection<T> v=new TCollection<T>();
            int s = TypeSerializerFactory.GetDeserializer(typeof(T));
            for(var i=0 ; i < c; ++i)
                v.Add(s.Invoke(r));
            return v;*/

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

        private static Type GetCollectionItemType(Type type)
        {
            Type collection = GetCollectionType(type);
            return collection != null ? collection.GetGenericArguments()[0] : null;
        }

        private static Type GetCollectionType(Type type)
        {
            return type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        private Expression CreateReadLoop(Expression readerObject, Type expectedValueType, ParameterExpression count)
        {
            ParameterExpression index = Expression.Parameter(typeof(int), "i");
            ParameterExpression deserializer = Expression.Parameter(typeof(Func<BinaryReader, object>), "s");
            ParameterExpression result = Expression.Parameter(expectedValueType, "v");
            LabelTarget loopEndLabel = Expression.Label(expectedValueType);

            BlockExpression forLoop = Expression.Block(
                new[] { index, result, deserializer },
                Expression.Assign(result, Expression.New(expectedValueType)),
                Expression.Assign(index, Expression.Constant(0, typeof(int))),
                Expression.Assign(deserializer, GetDeserializer<TypeSerializerFactory>(Expression.Constant(GetCollectionItemType(expectedValueType)))),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(index, count),
                        Expression.Block(
                            Expression.Call(Expression.TypeAs(result, GetCollectionType(expectedValueType)), "Add", null, CallDeserialize(deserializer, GetCollectionItemType(expectedValueType), readerObject)),
                            Expression.PreIncrementAssign(index)),
                        Expression.Break(loopEndLabel, result)),
                    loopEndLabel));

            return forLoop;
        }

        private Expression CreateWriteLoop(Expression writerObject, Expression value, Type valueType)
        {
            Type enumeratorType = GetEnumeratorType(valueType);
            ParameterExpression enumerator = Expression.Parameter(enumeratorType, "e");
            ParameterExpression serializer = Expression.Parameter(typeof(Action<BinaryWriter, object>), "s");
            LabelTarget loopEndLabel = Expression.Label();

            return Expression.Block(
                new[] { enumerator, serializer },
                Expression.Assign(serializer, GetSerializer<TypeSerializerFactory>(Expression.Constant(GetCollectionItemType(valueType)))),
                Expression.Assign(enumerator, Expression.Convert(Expression.Call(value, "GetEnumerator", null), enumeratorType)),
                Expression.TryFinally(
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.Call(Expression.TypeAs(enumerator, typeof(IEnumerator)), "MoveNext", null),
                            CallSerializeWithConvert(serializer, Expression.Property(enumerator, "Current"), writerObject),
                            Expression.Break(loopEndLabel)),
                        loopEndLabel),
                    Expression.Call(Expression.TypeAs(enumerator, typeof(IDisposable)), "Dispose", null)));
        }

        private Type GetEnumeratorType(Type collectionType)
        {
            return typeof(IEnumerator<>).MakeGenericType(GetCollectionItemType(collectionType));
        }
    }
}