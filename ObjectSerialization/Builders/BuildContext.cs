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
		public ParameterExpression ReadResultObject { get; private set; }
		public readonly ParameterExpression ReaderObject = Expression.Parameter(typeof(BinaryReader), "r");
		public readonly LabelTarget ReadReturnLabel = Expression.Label(typeof(object), "ret");

		private readonly List<Expression> _writeExpressions = new List<Expression>();
		private readonly ParameterExpression _writeParameter = Expression.Parameter(typeof(object), "o");
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
			_writeExpressions.Add(Expression.Assign(WriteObject, Expression.Convert(_writeParameter, typeof(T))));
		}

		public BuildContext(ParameterExpression readResult)
			: this()
		{
			ReadResultObject = readResult;
			_readExpressions.Add(Expression.Assign(ReadResultObject, Expression.New(typeof(T))));
		}

		public Func<BinaryReader, object> GetDeserializeFn()
		{
			if (ReadResultObject != null)
				_readExpressions.Add(ReturnValue(ReadResultObject));

			List<Expression> expressions = _readExpressions.ToList();

			expressions.Add(Expression.Label(ReadReturnLabel, Expression.Convert(Expression.Default(typeof(T)), typeof(object))));
			Expression body = Expression.Block(ReadResultObject == null ? null : new[] { ReadResultObject }, expressions);
			Expression<Func<BinaryReader, object>> expression = Expression.Lambda<Func<BinaryReader, object>>(body, ReaderObject);
#if DEBUG
			DumpExpression("Deserialize", expression);
#endif
			return expression.Compile();
		}

		public Expression ReturnValue(Expression result)
		{
			return Expression.Return(ReadReturnLabel, Expression.Convert(result, typeof(object)), typeof(object));
		}

		public Action<BinaryWriter, object> GetSerializeFn()
		{
			BlockExpression blockExpression = Expression.Block(new[] { WriteObject }, _writeExpressions);
			Expression<Action<BinaryWriter, object>> expression = Expression.Lambda<Action<BinaryWriter, object>>(blockExpression, WriterObject, _writeParameter);
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