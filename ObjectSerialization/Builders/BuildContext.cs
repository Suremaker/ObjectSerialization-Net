using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using CodeBuilder;
using CodeBuilder.Expressions;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
    internal class BuildContext<T>
    {
        private readonly string _suffix;
        public readonly Expression ReaderObject = Expr.Parameter(0, typeof(BinaryReader));

        public readonly Expression WriteObject = Expr.Parameter(1, typeof(T));
        public readonly Expression WriterObject = Expr.Parameter(0, typeof(BinaryWriter));
        private readonly List<Expression> _readExpressions = new List<Expression>();
        private readonly List<Expression> _writeExpressions = new List<Expression>();
        public LocalVariable ReadResultObject { get; private set; }

        public BuildContext(string suffix)
        {
            _suffix = suffix;
        }

        public BuildContext(LocalVariable readResult, string suffix)
            : this(suffix)
        {
            ReadResultObject = readResult;
            _readExpressions.Add(Expr.DeclareLocal(ReadResultObject, BaseTypeSerializer.InstantiateNew(typeof(T))));
        }

        public void AddReadExpression(Expression expr)
        {
            _readExpressions.Add(expr);
        }

        public void AddWriteExpression(Expression expr)
        {
            _writeExpressions.Add(expr);
        }

        public Func<BinaryReader, T> GetDeserializeFn()
        {
            if (ReadResultObject != null)
                _readExpressions.Add(Expr.Return(Expr.ReadLocal(ReadResultObject)));

            return DefineMethod<Func<BinaryReader, T>>("Deserialize", typeof(T), new[] { typeof(BinaryReader) }, _readExpressions.ToArray());

        }

        public Action<BinaryWriter, T> GetSerializeFn()
        {
            return DefineMethod<Action<BinaryWriter, T>>("Serialize", typeof(void), new[] { typeof(BinaryWriter), typeof(T) }, _writeExpressions.ToArray());
        }

        public Expression ReturnValue(Expression result)
        {
            return Expr.Return(result);
        }

        private void DumpExpression(DynamicMethod method, MethodBodyBuilder builder)
        {
            Console.Write("{0} {1}:\n{2}\n\n", typeof(T).Name, method.Name, builder);
        }

        private T DefineMethod<T>(string name, Type returnType, Type[] parameters, params Expression[] body)
        {
            var method = new DynamicMethod(name + _suffix, returnType, parameters, true);

            var bodyBuilder = new MethodBodyBuilder(method, parameters);
            bodyBuilder.AddStatements(body);
#if DEBUG
            DumpExpression(method, bodyBuilder);
#endif
            bodyBuilder.Compile();

            return (T)(object)method.CreateDelegate(typeof(T));
        }
    }
}