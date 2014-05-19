using System;
using CodeBuilder;
using CodeBuilder.Expressions;
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
                int s = TypeSerializerFactory.GetSerializer<T>();
                for (var i = 0; i < c; ++i)
                    s.Invoke(w, v[i]);                
            }
            else
                w.Write(-1);*/

            var checkNotNull = CheckNotNull(value, valueType);

            var forLoop = CreateWriteLoop(writerObject, value, valueType);
            var arrayWrite = Expr.Block(GetWriteExpression(Expr.ArrayLength(value), writerObject), forLoop);

            return Expr.IfThenElse(checkNotNull, arrayWrite, GetWriteExpression(Expr.Constant(-1), writerObject));
        }

        public Expression Read(Expression readerObject, Type expectedValueType)
        {
            /*BinaryReader r;
            int c = r.ReadInt32();
	        if (c == -1)
		        return null;
            
            T[]v=new T[c];
            int s = TypeSerializerFactory.GetDeserializer<T>();
            for(var i=0 ; i < c; ++i)
                v[i] = s.Invoke(r);
            return v;
            */

            var count = Expr.LocalVariable(typeof(int), "c");
            var countRead = Expr.DeclareLocal(count, GetReadExpression("ReadInt32", readerObject));

            var expression = Expr.ValueBlock(
                expectedValueType,
                countRead,
                Expr.IfThenElse(
                    Expr.Equal(Expr.ReadLocal(count), Expr.Constant(-1)),
                    Expr.Null(expectedValueType),
                    CreateReadLoop(readerObject, expectedValueType, count)));
            return expression;
        }

        #endregion

        private static Expression CreateWriteLoop(Expression writerObject, Expression value, Type valueType)
        {
            var index = Expr.LocalVariable(typeof(int), "i");
            var count = Expr.LocalVariable(typeof(int), "c");
            var serializer = Expr.LocalVariable(GetWriteSerializerDelegateType(valueType.GetElementType()), "s");

            return Expr.Block(
                Expr.DeclareLocal(index, Expr.Constant(0)),
                Expr.DeclareLocal(count, Expr.ArrayLength(value)),
                Expr.DeclareLocal(serializer, GetSerializer<TypeSerializerFactory>(valueType.GetElementType())),
                Expr.Loop(
                    Expr.IfThenElse(
                        Expr.Less(Expr.ReadLocal(index), Expr.ReadLocal(count)),
                        Expr.Block(
                            CallSerialize(Expr.ReadLocal(serializer), Expr.ReadArray(value, Expr.ReadLocal(index)), writerObject),
                            Expr.WriteLocal(index, Expr.Add(Expr.ReadLocal(index), Expr.Constant(1)))),
                        Expr.LoopBreak())));
        }

        private Expression CreateReadLoop(Expression readerObject, Type expectedValueType, LocalVariable count)
        {
            var index = Expr.LocalVariable(typeof(int), "i");
            var deserializer = Expr.LocalVariable(GetReadSerializerDelegateType(expectedValueType.GetElementType()), "s");
            var result = Expr.LocalVariable(expectedValueType, "r");

            var forLoop = Expr.ValueBlock(
                expectedValueType,
                Expr.DeclareLocal(result, Expr.NewArray(expectedValueType.GetElementType(), Expr.ReadLocal(count))),
                Expr.DeclareLocal(index, Expr.Constant(0)),
                Expr.DeclareLocal(deserializer, GetDeserializer<TypeSerializerFactory>(expectedValueType.GetElementType())),
                Expr.Loop(
                    Expr.IfThenElse(
                        Expr.Less(Expr.ReadLocal(index), Expr.ReadLocal(count)),
                        Expr.Block(
                            Expr.WriteArray(Expr.ReadLocal(result), Expr.ReadLocal(index), CallDeserialize(Expr.ReadLocal(deserializer), readerObject)),
                            Expr.WriteLocal(index, Expr.Add(Expr.ReadLocal(index), Expr.Constant(1)))),
                        Expr.LoopBreak())),
                Expr.ReadLocal(result));

            return forLoop;
        }

    }
}