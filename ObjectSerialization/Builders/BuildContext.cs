using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSerialization.Builders
{
    internal class BuildContext<T>
    {
        private readonly List<Expression> _readExpressions = new List<Expression>();

        public readonly ParameterExpression ReadResultObject = Expression.Variable(typeof(T), "o");
        public readonly ParameterExpression ReaderObject = Expression.Parameter(typeof(BinaryReader), "r");
        private readonly List<Expression> _writeExpressions = new List<Expression>();

        private readonly ParameterExpression WriteParameter = Expression.Parameter(typeof(object), "o");
        public readonly ParameterExpression WriteObject = Expression.Variable(typeof(T), "v");
        public readonly ParameterExpression WriterObject = Expression.Parameter(typeof(BinaryWriter), "w");


        public void AddReadExpression(Expression expr)
        {
            _readExpressions.Add(expr);
        }

        public void AddWriteExpression(Expression expr)
        {
            _writeExpressions.Add(expr);
        }

        public BuildContext()
        {
            _writeExpressions.Add(Expression.Assign(WriteObject, Expression.Convert(WriteParameter, typeof(T))));
        }

        public BuildContext(Expression readResultDefaultValue)
            : this()
        {
            _readExpressions.Add(Expression.Assign(ReadResultObject, readResultDefaultValue));
        }

        public Func<BinaryReader, object> GetDeserializeFn()
        {
            LabelTarget label = Expression.Label(typeof(object));
            List<Expression> expressions = _readExpressions.ToList();
            expressions.Add(Expression.Return(label, Expression.Convert(ReadResultObject, typeof(object)), typeof(object)));
            expressions.Add(Expression.Label(label, Expression.Convert(Expression.Default(typeof(T)), typeof(object))));
            Expression body = Expression.Block(new[] { ReadResultObject }, expressions);
            Expression<Func<BinaryReader, object>> expression = Expression.Lambda<Func<BinaryReader, object>>(body, ReaderObject);
#if DEBUG
            DumpExpression("Deserialize", expression);
#endif
            return expression.Compile();
        }

        public Action<BinaryWriter, object> GetSerializeFn()
        {
            BlockExpression blockExpression = Expression.Block(new[] { WriteObject }, _writeExpressions);
            Expression<Action<BinaryWriter, object>> expression = Expression.Lambda<Action<BinaryWriter, object>>(blockExpression, WriterObject, WriteParameter);
#if DEBUG
            DumpExpression("Serialize", expression);
#endif
            return expression.Compile();
        }

        private void DumpExpression(string operation, Expression expression)
        {
            object value = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                      .GetValue(expression, null);
            Console.Write("{0} {1}: {2}\n", typeof(T).Name, operation, value);
        }
    }
}