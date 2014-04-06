using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectSerialization.Builders.Types;

namespace ObjectSerialization.Builders
{
	internal class BuildContext<T>
	{
		public readonly LabelTarget ReadReturnLabel = Expression.Label(typeof(T), "ret");
		public readonly ParameterExpression ReaderObject = Expression.Parameter(typeof(BinaryReader), "r");

		public readonly ParameterExpression WriteObject = Expression.Parameter(typeof(T), "o");
		public readonly ParameterExpression WriterObject = Expression.Parameter(typeof(BinaryWriter), "w");
		private readonly List<Expression> _readExpressions = new List<Expression>();
		private readonly List<Expression> _writeExpressions = new List<Expression>();
		public ParameterExpression ReadResultObject { get; private set; }

		public BuildContext()
		{
		}

		public BuildContext(ParameterExpression readResult)
			: this()
		{
			ReadResultObject = readResult;
			_readExpressions.Add(Expression.Assign(ReadResultObject, BaseTypeSerializer.InstantiateNew(typeof(T))));
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
				_readExpressions.Add(ReturnValue(ReadResultObject));

			List<Expression> expressions = _readExpressions.ToList();

			expressions.Add(Expression.Label(ReadReturnLabel, Expression.Default(typeof(T))));
			Expression body = Expression.Block(ReadResultObject == null ? null : new[] { ReadResultObject }, expressions);
			Expression<Func<BinaryReader, T>> expression = Expression.Lambda<Func<BinaryReader, T>>(body, ReaderObject);
#if DEBUG
			DumpExpression("Deserialize", expression);
#endif
			return expression.Compile();
		}

		public Action<BinaryWriter, T> GetSerializeFn()
		{
			Expression body = !_writeExpressions.Any()
								  ? Expression.Empty()
								  : (_writeExpressions.Count == 1)
									? _writeExpressions[0]
									: Expression.Block(_writeExpressions);
			Expression<Action<BinaryWriter, T>> expression = Expression.Lambda<Action<BinaryWriter, T>>(body, WriterObject, WriteObject);
#if DEBUG
			DumpExpression("Serialize", expression);
#endif
			return expression.Compile();
		}

		public Expression ReturnValue(Expression result)
		{
			return Expression.Return(ReadReturnLabel, result, typeof(T));
		}

		private void DumpExpression(string operation, Expression expression)
		{
			object value = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
					  .GetValue(expression, null);
			Console.Write("{0} {1}: {2}\n", typeof(T).Name, operation, value);
		}
	}
}