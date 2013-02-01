using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSerialization.Builders
{
    internal class BuildContext<T>
    {
        public readonly List<Expression> ReadExpressions = new List<Expression>();

        public readonly ParameterExpression ReadResultObject = Expression.Variable(typeof(T), "o");
        public readonly ParameterExpression ReaderObject = Expression.Parameter(typeof(BinaryReader), "r");
        public readonly List<Expression> WriteExpressions = new List<Expression>();
        public readonly ParameterExpression WriteObject = Expression.Parameter(typeof(object), "o");
        public readonly ParameterExpression WriterObject = Expression.Parameter(typeof(BinaryWriter), "w");


        public BuildContext(Expression readResultDefaultValue)
        {
            ReadExpressions.Add(Expression.Assign(ReadResultObject, readResultDefaultValue));
        }

        public Func<BinaryReader, object> GetDeserializeFn()
        {
            LabelTarget label = Expression.Label(typeof(object));
            List<Expression> expressions = ReadExpressions.ToList();
            expressions.Add(Expression.Return(label, Expression.Convert(ReadResultObject, typeof(object)), typeof(object)));
            expressions.Add(Expression.Label(label, Expression.Convert(Expression.Default(typeof(T)), typeof(object))));
            BlockExpression body = Expression.Block(new[] { ReadResultObject }, expressions);
            Expression<Func<BinaryReader, object>> expression = Expression.Lambda<Func<BinaryReader, object>>(body, ReaderObject);
            return expression.Compile();
        }

        public Action<BinaryWriter, object> GetSerializeFn()
        {
            Expression<Action<BinaryWriter, object>> expression = Expression.Lambda<Action<BinaryWriter, object>>(Expression.Block(WriteExpressions), WriterObject, WriteObject);
            return expression.Compile();
        }
    }
}