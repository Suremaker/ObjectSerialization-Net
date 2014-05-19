using System;
using System.Collections.Generic;
using System.Linq;
using CodeBuilder;
using CodeBuilder.Expressions;

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

			return Expr.Block(
				GetWriteExpression(notNull, writerObject),
				Expr.IfThen(
					notNull,
					GetWriteExpression(value, writerObject)));
		}

		public Expression Read(Expression readerObject, Type expectedValueType)
		{
			/*
			BinaryReader r;
			return r.ReadBoolean()?r.ReadString():null;*/

			return Expr.IfThenElse(
				GetReadExpression("ReadBoolean", readerObject),
				GetReadExpression("Read" + expectedValueType.Name, readerObject),
				Expr.Null(expectedValueType));
		}

	    #endregion
	}
}