using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSerialization.Builders.Types
{
	internal class NullablePredefinedTypeSerializer : BaseTypeSerializer, ISerializer
	{
		private static readonly IEnumerable<Type> _predefinedTypes = new[] { typeof(string) };

		#region ISerializer Members

		public bool IsSupported(Type type)
		{
			return _predefinedTypes.Contains(type);
		}

		public Expression Write(Expression writerObject, Expression value, Type valueType)
		{
			/*BinaryWriter w;   
			stirng v;
			w.Write(v!=null);
			if(v!=null)
				w.Write(v);*/

			Expression notNull = CheckNotNull(value, valueType);

			return Expression.Block(
				GetWriteExpression(notNull, writerObject),
				Expression.IfThen(
					notNull,
					GetWriteExpression(value, writerObject)));
		}

		public Expression Read(Expression readerObject, Type expectedValueType)
		{
			/*
			BinaryReader r;
			return r.ReadBoolean()?r.ReadString():null;*/

			return Expression.Condition(
				GetReadExpression("ReadBoolean", readerObject),
				GetReadExpression("Read" + expectedValueType.Name, readerObject),
				Expression.Constant(null, expectedValueType));
		}

		#endregion
	}
}