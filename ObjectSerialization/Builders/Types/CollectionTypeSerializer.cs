using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBuilder;
using CodeBuilder.Expressions;
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
	            int s = TypeSerializerFactory.GetSerializer<T>();
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

            BlockExpression collectionWrite = Expr.Block(
                GetWriteExpression(Expr.ReadProperty(value, "Count"), writerObject),
                CreateWriteLoop(writerObject, value, valueType));

            return Expr.IfThenElse(checkNotNull, collectionWrite, GetWriteExpression(Expr.Constant(-1), writerObject));
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            int c = r.ReadInt32();
            if (c == -1)
                return null;
                
            TCollection<T> v=new TCollection<T>();
            int s = TypeSerializerFactory.GetDeserializer<T>();
            for(var i=0 ; i < c; ++i)
                v.Add(s.Invoke(r));
            return v;*/

            var count = Expr.LocalVariable(typeof(int), "c");
            var countRead = Expr.DeclareLocal(count, GetReadExpression("ReadInt32", readerObject));

            var expression = Expr.ValueBlock(
                expectedValueType,
                countRead,
                Expr.IfThenElse(
                    Expr.Equal(Expr.ReadLocal(count), Expr.Constant(-1)),
                    Expr.Null(expectedValueType),
                    CreateReadLoop(readerObject, expectedValueType, Expr.ReadLocal(count))));
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

        private Expression CreateReadLoop(Expression readerObject, Type expectedValueType, Expression count)
        {
            var index = Expr.LocalVariable(typeof(int), "i");
            var deserializer = Expr.LocalVariable(GetReadSerializerDelegateType(GetCollectionItemType(expectedValueType)), "s");
            var result = Expr.LocalVariable(expectedValueType, "v");

            var forLoop = Expr.ValueBlock(
                expectedValueType,
                Expr.DeclareLocal(result, InstantiateNew(expectedValueType)),
                Expr.DeclareLocal(index, Expr.Constant(0)),
                Expr.DeclareLocal(deserializer, GetDeserializer<TypeSerializerFactory>(GetCollectionItemType(expectedValueType))),
                Expr.Loop(
                    Expr.IfThenElse(
                        Expr.Less(Expr.ReadLocal(index), count),
                        Expr.Block(
                            Expr.Call(Expr.Convert(Expr.ReadLocal(result), GetCollectionType(expectedValueType)), "Add", CallDeserialize(Expr.ReadLocal(deserializer), readerObject)),
                            Expr.WriteLocal(index,Expr.Add(Expr.ReadLocal(index),Expr.Constant(1)))),
                        Expr.LoopBreak())),
                Expr.ReadLocal(result));

            return forLoop;
        }

        private Expression CreateWriteLoop(Expression writerObject, Expression value, Type valueType)
        {
            Type enumeratorType = GetEnumeratorType(valueType);
            var enumerator = Expr.LocalVariable(enumeratorType, "e");
            var serializer = Expr.LocalVariable(GetWriteSerializerDelegateType(GetCollectionItemType(valueType)), "s");

            return Expr.Block(
                Expr.DeclareLocal(serializer, GetSerializer<TypeSerializerFactory>(GetCollectionItemType(valueType))),
                Expr.DeclareLocal(enumerator, Expr.Convert(Expr.Call(value, "GetEnumerator"), enumeratorType)),
                Expr.TryFinally(
                    Expr.Loop(
                        Expr.IfThenElse(
                            Expr.Call(Expr.Convert(Expr.ReadLocal(enumerator), typeof(IEnumerator)), "MoveNext"),
                            CallSerialize(Expr.ReadLocal(serializer), Expr.ReadProperty(Expr.ReadLocal(enumerator), "Current"), writerObject),
                            Expr.LoopBreak())),
                    Expr.Call(Expr.Convert(Expr.ReadLocal(enumerator), typeof(IDisposable)), "Dispose")));
        }

        private Type GetEnumeratorType(Type collectionType)
        {
            return typeof(IEnumerator<>).MakeGenericType(GetCollectionItemType(collectionType));
        }
    }
}